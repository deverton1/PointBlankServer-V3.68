﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointBlank.Battle.Data.Models
{
    public class AssistModel
    {
        public int Killer, Victim, Damage, RoomId;
        public bool IsAssist, IsKiller, VictimDead;
    }
}
