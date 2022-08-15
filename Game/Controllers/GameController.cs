using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Quartz;
using Quartz.Impl;

namespace Game.Controllers
{
    public class GameController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var query = new Entity.SearchIn.SearchingGame();
            return View(BL.GameBL.Get(query));
        }
        
        ConcurrentDictionary<int, IScheduler> gameFlows = new ConcurrentDictionary<int, IScheduler>();

        StdSchedulerFactory factory = new StdSchedulerFactory();
        
        [Authorize]
        public async Task<IActionResult> CreateNewGame()
        {

            var game = new Entity.Game();
            game.Host = BL.UserBL.Get(User.Identity.Name).Id;
            game.Data = "";
            game.Status = true;
            
            game.Id = BL.GameBL.AddOrUpdate(game);

            var hostSession = new Entity.Session();
            hostSession.User = game.Host;
            hostSession.Game = game.Id;
            hostSession.Status = true;
            BL.SessionBL.AddOrUpdate(hostSession);
            
            BL.GameLogic.Initialization(game);


            ITrigger trigger = TriggerBuilder.Create()      // создаем триггер
            .WithIdentity(game.Id.ToString(), "group1")     // идентифицируем триггер с именем и группой
            .StartNow()                                     // запуск сразу после начала выполнения
            .WithSimpleSchedule(x => x                      // настраиваем выполнение действия
                .WithInterval(new TimeSpan(0, 0, 0, 0, 20)) // каждые 20 мс
                .RepeatForever())                           // бесконечное повторение
            .Build();

            IJobDetail job = JobBuilder.Create<BL.GamePlay>()
                .UsingJobData("gameId", game.Id)
                .Build();

            

            gameFlows.TryAdd(game.Id, await factory.GetScheduler());



            await gameFlows[game.Id].Start();

            await gameFlows[game.Id].ScheduleJob(job, trigger);


            BL.GameLogic.JoinUser(game.Id, game.Host ?? 0);
            
            return RedirectToAction("Start", "Game", new { gameId = game.Id, userId = game.Host });
        }

        [Authorize]
        public IActionResult Join(int gameId)
        {
            var session = new Entity.Session();
            session.User = BL.UserBL.Get(User.Identity.Name).Id;
            session.Game = gameId;
            session.Status = true;

            if (BL.SessionBL.Get(session.User ?? 0, session.Game ?? 0) != null)
            {
                session.Id = BL.SessionBL.Get(session.User ?? 0, session.Game ?? 0).Id;
            }

            BL.SessionBL.AddOrUpdate(session);

            BL.GameLogic.JoinUser(session.Game ?? 0, session.User ?? 0);

            return RedirectToAction("Start", "Game", new { gameId = session.Game, userId = session.User });
        }

        [Authorize]
        public IActionResult Leave(int gameId)
        {
            var session = new Entity.Session();
            session.User = BL.UserBL.Get(User.Identity.Name).Id;
            session.Game = gameId;
            session.Status = false;
            session.Id = BL.SessionBL.Get(session.User ?? 0, session.Game ?? 0).Id;
            BL.SessionBL.AddOrUpdate(session);

            BL.GameLogic.LeaveUser(session.Game ?? 0, session.User ?? 0);

            return RedirectToAction("Index", "Game");
        }

        [Authorize]
        public IActionResult Start(int gameId, int userId)
        {
            return View();
        }

        public IActionResult PressButton(int buttonCode, bool isDown, int gameId, int userId)
        {
            if (isDown)
            {
                if (!BL.GameLogic.allButtons.ContainsKey(gameId)) BL.GameLogic.allButtons.TryAdd(gameId, new ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>());
                if (!BL.GameLogic.allButtons[gameId].ContainsKey(userId)) BL.GameLogic.allButtons[gameId].TryAdd(userId, new ConcurrentDictionary<int, bool>());
                if (!BL.GameLogic.allButtons[gameId][userId].ContainsKey(buttonCode))
                {
                    BL.GameLogic.allButtons[gameId][userId].TryAdd(buttonCode, true);
                }
            } else
            {
                if (BL.GameLogic.allButtons.ContainsKey(gameId) && BL.GameLogic.allButtons[gameId].ContainsKey(userId))
                {
                    if (BL.GameLogic.allButtons[gameId][userId].ContainsKey(buttonCode))
                    {
                        bool success;
                        BL.GameLogic.allButtons[gameId][userId].Remove(buttonCode, out success);
                    }
                }
                
            }
                
            return Ok(Json(true));
        }
        public IActionResult Fire(bool isFire, int gameId, int userId, double fireX = 0, double fireY = 0)
        {
            if (isFire)
            {
                if (!BL.GameLogic.allFire.ContainsKey(gameId)) BL.GameLogic.allFire.TryAdd(gameId, new ConcurrentDictionary<int, (bool, (double, double))>());
                if (!BL.GameLogic.allFire[gameId].ContainsKey(userId)) BL.GameLogic.allFire[gameId].TryAdd(userId, (false, (0,0)));
                BL.GameLogic.allFire[gameId][userId] = (true, (fireX, fireY));
            }
            else
            {
                if (BL.GameLogic.allFire.ContainsKey(gameId) && BL.GameLogic.allFire[gameId].ContainsKey(userId))
                {
                    if (BL.GameLogic.allFire[gameId][userId].Item1 == true)
                    {
                        BL.GameLogic.allFire[gameId][userId] = (false, (0,0));
                    }
                }
            }
            return Ok(Json(true));
        }

        public IActionResult LockTarget(string targetId, bool isLock, int gameId, int userId)
        {
            BL.GameLogic.LockTarget(gameId, userId, targetId, isLock);
            return Ok(Json(true));
        }

        [HttpGet]
        public async Task<IActionResult> GetElements(int gameId, int userId)
        {

            if (BL.GameLogic.allGames.ContainsKey(gameId))
            {
                int count = BL.GameLogic.allGames[gameId]["static"].Values.Count + BL.GameLogic.allGames[gameId]["ships"].Values.Count + BL.GameLogic.allGames[gameId]["ammo"].Values.Count + BL.GameLogic.allGames[gameId]["bg"].Values.Count;
                Entity.Ingame.BaseObject[] temp = new Entity.Ingame.BaseObject[count];
                BL.GameLogic.allGames[gameId]["static"].Values.CopyTo(temp,0);
                BL.GameLogic.allGames[gameId]["ships"].Values.CopyTo(temp, BL.GameLogic.allGames[gameId]["static"].Values.Count);
                BL.GameLogic.allGames[gameId]["ammo"].Values.CopyTo(temp, BL.GameLogic.allGames[gameId]["ships"].Values.Count+BL.GameLogic.allGames[gameId]["static"].Values.Count);
                BL.GameLogic.allGames[gameId]["bg"].Values.CopyTo(temp, BL.GameLogic.allGames[gameId]["ships"].Values.Count+BL.GameLogic.allGames[gameId]["static"].Values.Count+BL.GameLogic.allGames[gameId]["ammo"].Values.Count);

                var json = JsonSerializer.Serialize((object[])temp);

                return Ok(json);
            }
            else return Ok();
        }
    }
}
