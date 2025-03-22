using PointBlank.Auth.Data.Managers;
using PointBlank.Auth.Data.Model;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Models.Account.Clan;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System;

namespace PointBlank.Auth.Network.ServerPacket
{
    public class PROTOCOL_BASE_GET_USER_INFO_ACK : SendPacket
    {
        private Account Player;
        private Clan Clan;
        private uint Error;
        //private bool Xmas;
        private byte[] Flag = new byte[4];

        public PROTOCOL_BASE_GET_USER_INFO_ACK(Account Player)
        {
            this.Player = Player;
            if (Player != null && Player._inventory._items.Count == 0)
            {
                Clan = ClanManager.getClanDB(Player.clan_id, 1);
                Player.LoadInventory();
                Player.LoadMissionList();
                Player.DiscountPlayerItems();
            }
            else
            {
                Error = 0x80000000;
            }
        }

        private void CheckGameEvents(EventVisitModel evVisit)
        {
            long dateNow = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            PlayerEvent pev = Player._event;
            if (pev != null)
            {
                QuestModel evQuest = EventQuestSyncer.getRunningEvent();
                if (evQuest != null)
                {
                    long date = pev.LastQuestDate, finish = pev.LastQuestFinish;
                    if (pev.LastQuestDate < evQuest.startDate)
                    {
                        pev.LastQuestDate = 0;
                        pev.LastQuestFinish = 0;
                    }
                    if (pev.LastQuestFinish == 0)
                    {
                        Player._mission.mission4 = 13; // MissionId
                        if (pev.LastQuestDate == 0)
                        {
                            pev.LastQuestDate = (uint)dateNow;
                        }
                    }
                    if (pev.LastQuestDate != date || pev.LastQuestFinish != finish)
                    {
                        EventQuestSyncer.ResetPlayerEvent(Player.player_id, pev);
                    }
                }
                EventLoginModel evLogin = EventLoginSyncer.getRunningEvent();
                if (evLogin != null && !(evLogin.startDate < pev.LastLoginDate && pev.LastLoginDate < evLogin.endDate))
                {
                    ItemsModel item = new ItemsModel(evLogin._rewardId, evLogin._category, "Login Event", 1, evLogin._count);
                    PlayerManager.tryCreateItem(item, Player._inventory, Player.player_id);
                    ComDiv.updateDB("player_events", "last_login_date", dateNow, "player_id", Player.player_id);
                }
                if (evVisit != null && pev.LastVisitEventId != evVisit.id)
                {
                    pev.LastVisitEventId = evVisit.id;
                    pev.LastVisitSequence1 = 0;
                    pev.LastVisitSequence2 = 0;
                    pev.NextVisitDate = 0;
                    EventVisitSyncer.ResetPlayerEvent(Player.player_id, evVisit.id);
                }
            }
            ComDiv.updateDB("accounts", "last_login", dateNow, "player_id", Player.player_id);
        }

        public override void write()
        {
            ServerConfig cfg = AuthManager.Config;
            EventVisitModel ev = EventVisitSyncer.getRunningEvent();

            PlayerEvent playerEvent = this.Player._event;

            writeH(525);
            writeH(0);
            writeD(Error);
            if (Error > 0)
                return;

            writeB(new byte[55]);
            writeC((byte)Player.Characters.Count);
            writeH(210);
            writeC((byte)QuickStartXml.QucikStarts.Count);
            for (int i = 0; i < QuickStartXml.QucikStarts.Count; i++)
            {
                QuickStart Quick = QuickStartXml.QucikStarts[i];
                writeC((byte)Quick.MapId);
                writeC((byte)Quick.Rule);
                writeC((byte)Quick.StageOptions);
                writeC((byte)Quick.Type);
            }
            writeB(new byte[34]); //00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
            writeC(4); //04
            writeD(0); //00 00 00 00
            writeD(0); //00 00 00 00
            writeD(0); //00 00 00 00
            writeD(0); //00 00 00 00
            writeD(0); //00 00 00 00
            writeD(Player._titles.Slots); //01 00 00 00
            writeC(3); //03 
            writeC((byte)Player._titles.Equiped1); //00
            writeC((byte)Player._titles.Equiped2); //00
            writeC((byte)Player._titles.Equiped3); //00
            writeQ(Player._titles.Flags); //00 00 00 00 00 00 00 00 
            writeC(160); //A0
            writeB(Player._mission.list1);
            writeB(Player._mission.list2);
            writeB(Player._mission.list3);
            writeB(Player._mission.list4);
            writeC((byte)Player._mission.actualMission);
            writeC((byte)Player._mission.card1);
            writeC((byte)Player._mission.card2);
            writeC((byte)Player._mission.card3);
            writeC((byte)Player._mission.card4);
            writeB(ComDiv.getCardFlags(Player._mission.mission1, Player._mission.list1));
            writeB(ComDiv.getCardFlags(Player._mission.mission2, Player._mission.list2));
            writeB(ComDiv.getCardFlags(Player._mission.mission3, Player._mission.list3));
            writeB(ComDiv.getCardFlags(Player._mission.mission4, Player._mission.list4));
            writeC((byte)Player._mission.mission1); //01
            writeC((byte)Player._mission.mission2); //00
            writeC((byte)Player._mission.mission3); //00
            writeC((byte)Player._mission.mission4); //00
            writeD(Player.blue_order); //E7 03 00 00
            writeD(Player.medal); //E7 03 00 00
            writeD(Player.insignia); //E7 03 00 00
            writeD(Player.brooch); //E7 03 00 00
            writeB(new byte[11]);
            writeIP("127.0.0.1"); //adapter loopback
            writeD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
            if (Player.Characters.Count == 0)
            {
                writeC(0);
                writeC(1);
            }
            else
            {
                writeC((byte)(Player.getCharacter(Player._equip._red) == null ? 0 : Player.getCharacter(Player._equip._red).Slot));
                writeC((byte)(Player.getCharacter(Player._equip._blue) == null ? 1 : Player.getCharacter(Player._equip._blue).Slot));
            }
            writeD(Player._equip._dino);
            if (Player._inventory.getItem(Player._equip._dino) == null)
            {
                writeD(0);
            }
            else
            {
                writeD((uint)Player._inventory.getItem(Player._equip._dino)._objId);
            }

            writeD(0);
            writeD(0);
            writeD(0);
            writeD(0);

            writeD(0); // flags

            writeD(0);
            writeD(0);
            writeC(0);
            writeD(0);
            writeD(0);
            writeC(0);

            writeC((byte)Player.name_color);
            writeD(Player._bonus.fakeRank);
            writeD(Player._bonus.fakeRank);
            writeUnicode(Player._bonus.fakeNick, 66);
            writeH((short)Player._bonus.sightColor);
            writeH((short)Player._bonus.muzzle);
            writeC(0);
            writeD(Player._statistic.fights);
            writeD(Player._statistic.fights_win);
            writeD(Player._statistic.fights_lost);
            writeD(Player._statistic.fights_draw);
            writeD(Player._statistic.kills_count);
            writeD(Player._statistic.headshots_count);
            writeD(Player._statistic.deaths_count);
            writeD(Player._statistic.totalfights_count);
            writeD(Player._statistic.totalkills_count);
            writeD(Player._statistic.escapes);
            writeD(Player._statistic.assist);
            writeD(Player._statistic.mvp); // ?
            writeD(Player._statistic.fights);
            writeD(Player._statistic.fights_win);
            writeD(Player._statistic.fights_lost);
            writeD(Player._statistic.fights_draw);
            writeD(Player._statistic.kills_count);
            writeD(Player._statistic.headshots_count);
            writeD(Player._statistic.deaths_count);
            writeD(Player._statistic.totalfights_count);
            writeD(Player._statistic.totalkills_count);
            writeD(Player._statistic.escapes);
            writeD(Player._statistic.assist);
            writeD(Player._statistic.mvp);
            writeUnicode(Player.player_name, 66);
            writeD(Player._rank);
            writeD(Player._rank);
            writeD(Player._gp);
            writeD(Player._exp);
            writeB(new byte[15]);
            writeD(1024); //tags
            writeD(0);
            writeH(0);
            writeH(0);
            writeD(Player._money);
            writeD(Clan._id);
            writeD(Player.clanAccess);
            writeQ(Player.Status());
            writeC((byte)Player.pc_cafe);
            writeC((byte)Player.tourneyLevel);
            writeUnicode(Clan._name, 34);
            writeC((byte)Clan._rank);
            writeC((byte)Clan.getClanUnit());
            writeD(Clan._logo);
            writeC((byte)Clan._name_color);
            writeC((byte)(AuthManager.Config.BloodEnable ? Player.age : 27)); //29
        }

        private void WriteDormantEvent()
        {
            writeB(new byte[375]);
        }

        private void WriteVisitEvent(EventVisitModel ev)
        {
            PlayerEvent pev = Player._event;
            if (ev != null && (pev.LastVisitSequence1 < ev.checks && pev.NextVisitDate <= int.Parse(DateTime.Now.ToString("yyMMdd")) || pev.LastVisitSequence2 < ev.checks && pev.LastVisitSequence2 != pev.LastVisitSequence1))
            {
                writeUnicode(ev.title, 70);
                writeC((byte)pev.LastVisitSequence1);
                writeC((byte)ev.checks);
                writeD(ev.id);
                writeD(ev.startDate);
                writeD(ev.endDate);
                writeB(new byte[12]);
                for (int i = 0; i < 32; i++)
                {
                    VisitBox box = ev.box[i];
                    writeC((byte)box.RewardCount);
                    writeD(box.reward1.good_id);
                    writeD(box.reward2.good_id);
                }
            }
            else
            {
                writeB(new byte[375]);
            }
        }
    }
}