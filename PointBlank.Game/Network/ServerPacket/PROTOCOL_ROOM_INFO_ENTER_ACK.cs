﻿using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
    public class PROTOCOL_ROOM_INFO_ENTER_ACK : SendPacket
    {
        public PROTOCOL_ROOM_INFO_ENTER_ACK()
        {

        }

        public override void write()
        {
            writeH(3926);
            writeD(0);
        }
    }
}