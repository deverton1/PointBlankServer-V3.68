using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Data.Xml;
using System;
using System.Collections.Generic;
using PointBlank.Core.Managers.Server;

namespace PointBlank.Auth.Network.ServerPacket
{
    public class PROTOCOL_BASE_GET_SYSTEM_INFO_ACK : SendPacket
    {
        public PROTOCOL_BASE_GET_SYSTEM_INFO_ACK()
        {

        }

        public override void write()
        {
            ServerConfig cfg = AuthManager.Config;
            writeH(523);
            writeC(0);
            writeC(32);
            writeB(new byte[5]);
            writeB(new byte[4] { 0x64, 0x58, 0x02, 0x00 });
            writeC(0);
            writeD(1);
            writeB(new byte[370]);
            writeB(new byte[26]
            {
                0x05, 0x05, 0x04, 0xAC, 0x0D,
                0x00, 0x00, 0xAA, 0x05, 0x00,
                0x00, 0x31, 0x00, 0x00, 0x00,
                0x01, 0x00, 0x00, 0x00, 0x07,
                0x01, 0x00, 0x17, 0x15, 0x0F,
                0x04
            });
            writeB(new byte[49]);
            writeH(2565);
            writeB(new byte[225]);

            writeC(0);
            writeD(0);

            writeC(3);
            writeD(600);
            writeD(2400);
            writeD(6000);

            writeC(1); // Mission Card
            writeB(new byte[2] { 0xE2, 0x1F });
            writeH(0);

            writeH(29890);
            writeC((byte)ServersXml._servers.Count);
            foreach (GameServerModel server in ServersXml._servers)
            {
                writeD(server._state);
                writeIP(server.Connection.Address);
                writeH(server._port);
                writeH(server._port);
                writeH(server._port);
                writeC((byte)server._type);
                writeH((ushort)server._maxPlayers);
                writeD(server._LastCount);

                foreach (Channel channel in ChannelsXml._channels)
                {
                    writeC((byte)ChannelsXml._channels.Count);
                }

                writeC(0xE);
                writeD(server._type);
            }
            writeC(1);
            writeH(0);

            writeH((ushort)AuthManager.Config.ExitURL.Length);
            writeS(AuthManager.Config.ExitURL, AuthManager.Config.ExitURL.Length);

            writeC(51);
            for (int index = 0; index < 52; index++)
            {
                writeC((byte)index);
                List<ItemsModel> Items = RankXml.getAwards(index);

                for (int Idx = 0; Idx < Items.Count; Idx++)
                {
                    ItemsModel Item = Items[Idx];
                    writeD(ShopManager.getItemId(Item._id) == null ? 0 : ShopManager.getItemId(Item._id).id);
                    for (int i = Items.Count; (4 - i) > 0; i++)
                        writeD(0);
                }
                writeC(1);
            }
        }
    }
}