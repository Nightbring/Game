

using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using System.Net;
using System.Threading.Tasks;

namespace BL
{
    public class GamePlay : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int gameId = dataMap.GetInt("gameId");


            GameLogic.Control(gameId);

            GameLogic.PlayFire(gameId);

            GameLogic.MoveShip(gameId);

            GameLogic.MoveAmmo(gameId);

            GameLogic.MoveOrbital(gameId);


            return Task.CompletedTask;
        }
    }
}
