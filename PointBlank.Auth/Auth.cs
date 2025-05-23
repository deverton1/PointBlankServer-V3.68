using Microsoft.VisualBasic.Devices;
using PointBlank.Auth.Data.Managers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace PointBlank.Auth
{
    public class Auth
    {
        public static async void Update()
        {
            while (true)
            {
                try
                {
                    string users = AuthManager._socketList?.Count.ToString() ?? "0";
                    string serverInfo = ServersXml.getServer(0)?._LastCount.ToString() ?? "0";
                    long memoryUsed = GC.GetTotalMemory(false) / 1024;

                    Console.Title = $"Point Blank - Auth [Users: {users} Online: {serverInfo} Used RAM: {memoryUsed} KB]";

                    if (AuthManager._socketList != null)
                    {
                        foreach (var client in AuthManager._socketList.Values.ToList())
                        {
                            if (client != null && !client._client.Connected)
                            {
                                client.Close(0, true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.error($"Auth.Update: {ex}");
                }

                await Task.Delay(1000);
            }
        }
    }
}