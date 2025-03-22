using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;

namespace PointBlank.Auth.Network.ClientPacket
{
    public class PROTOCOL_BASE_GET_SYSTEM_INFO_REQ : ReceivePacket
    {
        public PROTOCOL_BASE_GET_SYSTEM_INFO_REQ(AuthClient lc, byte[] buff)
        {
            makeme(lc, buff);
        }

        public override void read()
        {
            readC();
        }

        public override void run()
        {
            try
            {
                Account player = _client._player;
                _client.SendPacket(new PROTOCOL_BASE_NOTICE_ACK());
                _client.SendPacket(new PROTOCOL_BASE_URL_LIST_ACK());
                //_client.SendPacket(new PROTOCOL_BASE_BOOSTEVENT_INFO_ACK());
                //_client.SendPacket(new PROTOCOL_BASE_STEPUP_MODE_INFO_ACK());
                _client.SendPacket(new PROTOCOL_BASE_CHANNELTYPE_CONDITION_ACK());
                _client.SendPacket(new PROTOCOL_BASE_GET_SYSTEM_INFO_ACK());

                _client.SendPacket(new PROTOCOL_BASE_GET_OPTION_ACK(0, player._config));
                List<Message> gifts = MessageManager.getGifts(player.player_id);
                if (gifts.Count > 0)
                {
                    MessageManager.RecicleMessages(player.player_id, gifts);
                    if (gifts.Count > 0)
                    {
                        //_client.SendPacket(new PROTOCOL_BASE_USER_GIFT_LIST_ACK(gifts));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.warning(ex.ToString());
            }
        }
        private void Test(IAsyncResult ar)
        {
            try
            {
                Socket asyncState = (Socket)ar.AsyncState;
                if (asyncState == null || !asyncState.Connected)
                    return;
                asyncState.EndSend(ar);
            }
            catch (Exception ex)
            {
                Logger.warning(ex.ToString());
            }
        }
    }
}


/*using PointBlank.Core;
using PointBlank.Auth.Network.ServerPacket;
using System;
using PointBlank.Auth.Data.Model;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account;
using System.Collections.Generic;
using PointBlank.Battle.Data.Models;

namespace PointBlank.Auth.Network.ClientPacket
{
    public class PROTOCOL_BASE_GET_SYSTEM_INFO_REQ : ReceivePacket
    {
        public PROTOCOL_BASE_GET_SYSTEM_INFO_REQ(AuthClient lc, byte[] buff)
        {
            makeme(lc, buff);
        }

        public override void read()
        {
            readC();
        }

        public override void run()
        {
            Account player = _client._player;
            if (player != null)
            {
                _client.SendPacket(new PROTOCOL_BASE_NOTICE_ACK());
                _client.SendPacket(new PROTOCOL_BASE_URL_LIST_ACK());
                //_client.SendPacket(new PROTOCOL_BASE_BOOSTEVENT_INFO_ACK());
               // _client.SendPacket(new PROTOCOL_BASE_STEPUP_MODE_INFO_ACK());
                _client.SendPacket(new PROTOCOL_BASE_CHANNELTYPE_CONDITION_ACK());
                _client.SendPacket(new PROTOCOL_BASE_GET_SYSTEM_INFO_ACK());

            }
        }
    }
}*/