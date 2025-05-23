using Microsoft.VisualBasic.Devices;
using PointBlank.Auth.Data.Managers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

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

                    Console.Title = $"Point Blank - Auth [Users: {users} Online: {serverInfo} Used RAM: {(GC.GetTotalMemory(true) / 1024)} KB]";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao atualizar título: {ex.Message}");
                }

                await Task.Delay(1000);
            }
        }
    }
}