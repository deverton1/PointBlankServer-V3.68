﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointBlank.Battle.Data.Models.Event
{
    public class DropWeaponInfo
    {
        public byte WeaponFlag, Extensions;
        public ushort AmmoPrin, AmmoDual, AmmoTotal, Unk1;
        public int WeaponId, Unk2;
    }
}
