using PointBlank.Core;
using PointBlank.Core.Network;
using System.Net;

namespace PointBlank.Auth.Network.ServerPacket
{
    public class PROTOCOL_BASE_CONNECT_ACK : SendPacket
    {
        private IPAddress Ip;
        private uint SessionId;
        private ushort SessionSeed;
        private int SetKey;

        public PROTOCOL_BASE_CONNECT_ACK(AuthClient Client)
        {
            SessionId = Client.SessionId;
            SessionSeed = Client.SessionSeed;
            SetKey = Client.SetKey;
            Ip = Client.GetAddress();
        }

        public override void write()
        {
            CheckIp(Ip);
            writeH(514);
            writeH(0);
            writeD(SessionId);
            writeB(new byte[16]);
            writeC(3);
            writeD(SetKey);
            writeH(0);
        }

        public void CheckIp(IPAddress Ip)
        {
            Logger.LogProblems(Ip.ToString(), "Ip/Auth");
        }
    }
}