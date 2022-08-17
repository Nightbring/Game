var objects = [];

var mouseX;
var mouseY;
var mouseDown = false;

const urlParams = new URLSearchParams(window.location.search);
const gameId = urlParams.get('gameId');
const userId = urlParams.get('userId');

var hero = null;
var minId;
var maxId;

var isAlive = true;


function myRender(serverObject) {
    if (serverObject.ObjectType == "ship") {
        //Ship and visual
        let d = document.createElement('div');
        let v = document.createElement('div');
        d.style.width = serverObject.Width + "px";
        d.style.height = serverObject.Height + "px";
        d.style.bottom = (serverObject.YPosition + parseInt(window.innerHeight) / 2 - hero.YPosition - serverObject.Radius) + "px";
        d.style.left = (serverObject.XPosition + parseInt(window.innerWidth) / 2 - hero.XPosition - serverObject.Radius) + "px";
        d.id = serverObject.Id;
        d.className = serverObject.ObjectType + " " + serverObject.Class;

        v.style.width = serverObject.Width + "px";
        v.style.height = serverObject.Height + "px";
        v.className = "visual";

        let j = document.createElement('div');
        j.className = "jetStream";
        j.style.display = "none";
        v.appendChild(j);
        let i = document.createElement('div');
        i.className = "shipImage " + serverObject.Id;
        i.style.width = serverObject.Width + "px";
        i.style.height = serverObject.Height + "px";
        v.appendChild(i);
        let wa = serverObject.Weapons;

        d.appendChild(v);
        //Weapons
        for (let k = 0; k < wa.length; k++) {
            let w = document.createElement('div');
            w.style.position = "absolute";
            w.style.borderRadius = "50%";
            w.style.backgroundColor = "magenta";
            w.style.left = wa[k].X - 3 + serverObject.Width / 2;
            w.style.bottom = wa[k].Y - 3 + serverObject.Height / 2;
            w.style.width = "6px";
            w.style.height = "6px";
            w.style.zIndex = "9";
            w.className = "weapon " + k;
            d.appendChild(w);
        }

        let t = document.createElement('div');
        t.className = "targetFrame";
        t.style.display = "none";
        d.appendChild(t);

        document.body.appendChild(d);
        //Minimap
        if (serverObject != hero) {
            let mm = document.createElement('div');
            mm.id = "mm" + serverObject.Id;
            mm.className = "MMShip"
            mm.style.height = "4px";
            mm.style.width = "4px";
            mm.style.left = ((serverObject.XPosition - hero.XPosition - 3) / 40 + 100) + "px";
            mm.style.bottom = ((serverObject.YPosition - hero.YPosition - 3) / 40 + 100) + "px";

            document.getElementById("minimap").appendChild(mm);
        } else {
            let mc = document.createElement('div');
            mc.className = "MMCenter";
            mc.style.left = "97px";
            mc.style.bottom = "97px";
            document.getElementById("minimap").appendChild(mc);
        }

    } else if (serverObject.ObjectType == "bgobject") {
        let d = document.createElement('div');
        d.style.width = ((serverObject.Radius * 2) / serverObject.Layer) + "px";
        d.style.height = ((serverObject.Radius * 2) / serverObject.Layer) + "px";

        d.style.bottom = (parseInt(window.innerHeight) / 2 + ((serverObject.YPosition - hero.YPosition) / serverObject.Layer) - (serverObject.Radius / serverObject.Layer)) + "px";
        d.style.left = (parseInt(window.innerWidth) / 2 + ((serverObject.XPosition - hero.XPosition) / serverObject.Layer) - (serverObject.Radius / serverObject.Layer)) + "px";

        d.id = serverObject.Id;
        d.className = serverObject.ObjectType + " " + serverObject.Class;
        document.body.appendChild(d);

        if (serverObject.Class == "star") {
            serverObject.Planets.forEach(function (planet) {
                let p = document.createElement('div');
                p.style.width = planet.Radius * 2 / planet.Layer + "px";
                p.style.height = planet.Radius * 2 / planet.Layer + "px";

                p.style.bottom = (parseInt(window.innerHeight) / 2 + ((planet.YPosition - hero.YPosition) / planet.Layer) - planet.Radius / planet.Layer) + "px";
                p.style.left = (parseInt(window.innerWidth) / 2 + ((planet.XPosition - hero.XPosition) / planet.Layer) - planet.Radius / planet.Layer) + "px";

                p.id = planet.Id;
                p.className = planet.ObjectType + " " + planet.Class;
                document.body.appendChild(p);

                planet.Moons.forEach(function (moon) {
                    let m = document.createElement('div');
                    m.style.width = moon.Radius * 2 / moon.Layer + "px";
                    m.style.height = moon.Radius * 2 / moon.Layer + "px";

                    m.style.bottom = (parseInt(window.innerHeight) / 2 + ((moon.YPosition - hero.YPosition) / moon.Layer) - moon.Radius / moon.Layer) + "px";
                    m.style.left = (parseInt(window.innerWidth) / 2 + ((moon.XPosition - hero.XPosition) / moon.Layer) - moon.Radius / moon.Layer) + "px";

                    m.id = moon.Id;
                    m.className = moon.ObjectType + " " + moon.Class;
                    document.body.appendChild(m);
                });
            });
        }

    } else if (serverObject.ObjectType) {
        let d = document.createElement('div');
        d.style.width = serverObject.Width + "px";
        d.style.height = serverObject.Height + "px";
        switch (serverObject.ObjectType) {
            case "ammo":
                d.className = serverObject.ObjectType + " " + serverObject.AmmoType;
                break;
            default:
                d.className = serverObject.ObjectType;
                break;
        }
        d.style.bottom = (serverObject.YPosition + parseInt(window.innerHeight) / 2 - hero.YPosition - serverObject.Height / 2) + "px";
        d.style.left = (serverObject.XPosition + parseInt(window.innerWidth) / 2 - hero.XPosition - serverObject.Width / 2) + "px";
        d.id = serverObject.Id;
        
        if (serverObject.Rotation) {
            let a = - serverObject.Rotation;
            d.style.webkitTransform = 'rotate(' + a + 'deg)';
        }
        document.body.appendChild(d);
    }
}

function refresh() {
    $.get("https://localhost:44337/Game/GetElements?gameId=" + gameId + "&userId=" + userId, function (data) {
        arr = JSON.parse(data);
        for (let i = 0; i < arr.length; i++) {
            let ex = false;
            for (let j = 0; j < objects.length; j++) {
                if (arr[i].Id == objects[j].Id) {
                    ex = true;
                    let object = document.getElementById(arr[i].Id);
                    if (arr[i].ObjectType != "bgobject") {
                        if (arr[i].Id != hero.Id) {
                            object.style.left = (arr[i].XPosition + parseInt(window.innerWidth) / 2 - hero.XPosition - arr[i].Width / 2) + "px";
                            object.style.bottom = (arr[i].YPosition + parseInt(window.innerHeight) / 2 - hero.YPosition - arr[i].Height / 2) + "px";
                        } else {
                            object.style.left = (parseInt(window.innerWidth) / 2 - hero.Width / 2) + "px";
                            object.style.bottom = (parseInt(window.innerHeight) / 2 - hero.Height / 2) + "px";
                        }
                        if (arr[i].Rotation && arr[i].ObjectType != "ship") {
                            let a = - arr[i].Rotation;
                            object.style.webkitTransform = 'rotate(' + a + 'deg)';
                        }
                    } else {
                        object.style.bottom = (parseInt(window.innerHeight) / 2 + ((arr[i].YPosition - hero.YPosition) / arr[i].Layer) - arr[i].Radius / arr[i].Layer) + "px";
                        object.style.left = (parseInt(window.innerWidth) / 2 + ((arr[i].XPosition - hero.XPosition) / arr[i].Layer) - arr[i].Radius / arr[i].Layer) + "px";

                        if (arr[i].Class == "star") {
                            arr[i].Planets.forEach(function (planet) {
                                let p = document.getElementById(planet.Id);

                                p.style.left = (parseInt(window.innerWidth) / 2 + ((planet.XPosition - hero.XPosition) / planet.Layer) - planet.Radius / planet.Layer) + "px";
                                p.style.bottom = (parseInt(window.innerHeight) / 2 + ((planet.YPosition - hero.YPosition) / planet.Layer) - planet.Radius / planet.Layer) + "px";

                                planet.Moons.forEach(function (moon) {
                                    let m = document.getElementById(moon.Id);

                                    m.style.left = (parseInt(window.innerWidth) / 2 + ((moon.XPosition - hero.XPosition) / moon.Layer) - moon.Radius / moon.Layer) + "px";
                                    m.style.bottom = (parseInt(window.innerHeight) / 2 + ((moon.YPosition - hero.YPosition) / moon.Layer) - moon.Radius / moon.Layer) + "px";
                                });
                            });
                        }
                    }

                    if (arr[i].ObjectType == "ship") {
                        let a = - arr[i].Rotation;
                        object.firstChild.style.webkitTransform = 'rotate(' + a + 'deg)';
                        if (arr[i].Speed > 0) {
                            object.firstChild.firstChild.style.display = "block";
                        } else
                            object.firstChild.firstChild.style.display = "none";
                        if (arr[i].UserId == userId) {
                            hero = arr[i];
                        }
                        if (arr[i].Id == hero.LockedTarget) {
                            object.getElementsByClassName("targetFrame")[0].style.display = "block";

                            document.getElementsByClassName("targetInfo")[0].style.display = "block";
                            let s = document.getElementById("targetShield");
                            s.style.width = (arr[i].Shield / arr[i].FullShield * 100) + "%";
                            s.textContent = arr[i].Shield;
                            let h = document.getElementById("targetHP");
                            h.style.width = (arr[i].HP / arr[i].FullHP * 100) + "%";
                            h.textContent = arr[i].HP;
                            let e = document.getElementById("targetEnergy");
                            e.style.width = (arr[i].Energy / arr[i].FullEnergy * 100) + "%";
                            e.textContent = arr[i].Energy;
                            document.getElementById("targetName").textContent = arr[i].Name;
                        } else {
                            object.getElementsByClassName("targetFrame")[0].style.display = "none";
                        }
                        if (arr[i].Id != hero.Id) {
                            let mm = document.getElementById("mm" + arr[i].Id);
                            mm.style.left = ((arr[i].XPosition - hero.XPosition - 2) / 25 + 100) + "px";
                            mm.style.bottom = ((arr[i].YPosition - hero.YPosition - 2) / 25 + 100) + "px";
                        }

                        let wa = arr[i].Weapons;
                        for (let k = 0; k < wa.length; k++) {
                            w = document.getElementById(arr[i].Id).getElementsByClassName(k)[0];
                            w.style.left = wa[k].X - 2 + arr[i].Width / 2;
                            w.style.bottom = wa[k].Y - 2 + arr[i].Height / 2;
                        }

                    }
                    break;
                }
            }
            if (!ex) {
                myRender(arr[i]);
            }
        }
        for (let i = 0; i < objects.length; i++) {
            let ex = false;
            for (var j = 0; j < arr.length; j++) {
                if (arr[j].Id == objects[i].Id) {
                    ex = true;
                    break;
                }
            }
            if (!ex && objects[i].ObjectType != "bgobject") {
                document.body.removeChild(document.getElementById(objects[i].Id));
                if (objects[i].ObjectType == "ship")
                    document.getElementById("minimap").removeChild(document.getElementById("mm"+objects[i].Id));
                if (objects[i].UserId == userId) {
                    hero = null;
                }
            }
        }
        objects = arr;

        if (hero.LockedTarget == "") {
            document.getElementsByClassName("targetInfo")[0].style.display = "none";
        }

        if (hero != null) {
            let s = document.getElementById("userShield");
            s.style.width = (hero.Shield / hero.FullShield * 100) + "%";
            s.textContent = hero.Shield;
            let h = document.getElementById("userHP");
            h.style.width = (hero.HP / hero.FullHP * 100) + "%";
            h.textContent = hero.HP;
            let e = document.getElementById("userEnergy");
            e.style.width = (hero.Energy / hero.FullEnergy * 100) + "%";
            e.textContent = hero.Energy;
            document.getElementById("userName").textContent = hero.Name;
        } else if (isAlive) {
            isAlive = false;
            alert("You are dead");
            document.getElementsByClassName("formBackground")[0].style.display = "block";
        }
    });
}

var isFire = false;
function checkShoot() {
    if (mouseDown == true) {
        isFire = true;
        $.get("https://localhost:44337/Game/Fire?isFire=true&gameId=" + gameId + "&userId=" + userId + "&fireX=" + (mouseX - parseInt(window.innerWidth) / 2 + hero.XPosition) + "&fireY=" + (mouseY - parseInt(window.innerHeight) / 2 + hero.YPosition));
    }
    else {
        if (isFire == true) {
            isFire = false;
            $.get("https://localhost:44337/Game/Fire?isFire=false&gameId=" + gameId + "&userId=" + userId);

        }
    }
}


document.addEventListener('keydown', function (event) {
    $.get("https://localhost:44337/Game/PressButton?buttonCode=" + event.keyCode + "&isDown=true&gameId=" + gameId + "&userId=" + userId);
});

document.addEventListener('keyup', function (event) {
    $.get("https://localhost:44337/Game/PressButton?buttonCode=" + event.keyCode + "&isDown=false&gameId=" + gameId + "&userId=" + userId);
});


function play() {
    checkShoot();
    refresh();
}

function initialize () {

    $.get("https://localhost:44337/Game/GetElements?gameId=" + gameId + "&userId=" + userId, function (data) {

        var arr = JSON.parse(data);
        console.log(arr);
        for (let i = 0; i < arr.length; i++) {
            if (arr[i].ObjectType == "ship" && arr[i].UserId == userId) {
                hero = arr[i];
                break;
            }
        }
        if (hero != null) {
            for (let i = 0; i < arr.length; i++) {
                myRender(arr[i]);
            }
            objects = arr;

            document.getElementsByClassName("formBackground")[0].style.display = "none";

            let playInterval = setInterval(play, 20);

        } //else {
            //document.getElementsByClassName("formBackground")[0].style.display = "block";
        //}

    });

}


document.addEventListener('DOMContentLoaded', function () {
    //console.log(userId);

    document.body.addEventListener('mousemove', function (event) {
        mouseX = event.pageX;
        mouseY = event.pageY;
    });

    document.body.addEventListener('mousedown', function (event) {
        if (event.which === 1)
            mouseDown = true;
    });

    document.addEventListener('mouseup', function (event) {
        if (event.which === 1)
            mouseDown = false;
    });

    document.addEventListener('contextmenu', function (event) {
        event.preventDefault();
        if (event.target && event.target.classList[1]) {
            $.get("https://localhost:44337/Game/LockTarget?targetId=" + event.target.classList[1] + "&isLock=true&gameId=" + gameId + "&userId=" + userId, function (data) {
                console.log(event.target.classList[1] + " " + data);
            });
        }
    });

    let joinFormBack = document.getElementsByClassName("formBackground")[0];
    let joinForm = document.getElementById("selectionShip");
    let selectedShip = joinForm.getElementsByClassName("formSelect")[0];
    let selectedWeapon = joinForm.getElementsByClassName("formSelect")[1];

    $.get("https://localhost:44337/Game/CheckUserInGame?gameId=" + gameId + "&userId=" + userId, function (data) {
        if (data == true) {
            initialize();
            //if (hero.Id) {
            //    joinFormBack.style.display = "none";
            //    //joinForm.style.display = "none";
            //}
        }
    }, false);

    joinForm.addEventListener("submit", function (e) {
        e.preventDefault;
        $.ajax({
            type: "POST",
            url: "https://localhost:44337/Game/Join",
            data: {
                gameId: gameId,
                selectedShip: selectedShip.value,
                selectedWeapon: selectedWeapon.value
            },
            dataType: "json",
            success: function () {
                initialize();
                joinFormBack.style.display = "none";
                //joinForm.style.display = "none";
            }
        });

    }, false);
});
