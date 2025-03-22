﻿using PointBlank.Core;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using System;
using System.Threading.Tasks;

namespace PointBlank.Game
{
    public static class Game
    {
        public static async void Update()
        {
            while (true)
            {
                Console.Title = "Point Blank - Game [Users: " + GameManager._socketList.Count + " Online: " + ServersXml.getServer(GameConfig.serverId)._LastCount + " Used RAM: " + (GC.GetTotalMemory(true) / 1024) + " KB]";
                if (DateTime.Now.ToString("HH:mm") == "00:00")
                {
                    foreach (Account Player in AccountManager._accounts.Values)
                    {
                        if (Player != null)
                        {
                            Player.Daily = new PlayerDailyRecord();
                        }
                    }
                    foreach (GameClient Client in GameManager._socketList.Values)
                    {
                        if (Client != null && Client._player != null && Client._player._isOnline)
                        {
                            Client._player.Daily = new PlayerDailyRecord();
                        }
                    }
                    ComDiv.updateDB("player_dailyrecord", "total", 0);
                    ComDiv.updateDB("player_dailyrecord", "wins", 0);
                    ComDiv.updateDB("player_dailyrecord", "loses", 0);
                    ComDiv.updateDB("player_dailyrecord", "draws", 0);
                    ComDiv.updateDB("player_dailyrecord", "kills", 0);
                    ComDiv.updateDB("player_dailyrecord", "deaths", 0);
                    ComDiv.updateDB("player_dailyrecord", "headshots", 0);
                    ComDiv.updateDB("player_dailyrecord", "point", 0);
                    ComDiv.updateDB("player_dailyrecord", "exp", 0);
                }
                await Task.Delay(1000);
            }
        }
    }
}