﻿using PointBlank.Battle.Data.Xml;
using System;

namespace PointBlank.Battle.Data.Models
{
    public class ObjectInfo
    {
        public int Id, Life = 100, DestroyState;
        public AnimModel Animation;
        public DateTime UseDate;
        public ObjectModel Model;

        public ObjectInfo()
        {

        }

        public ObjectInfo(int Id)
        {
            this.Id = Id;
        }
    }
}