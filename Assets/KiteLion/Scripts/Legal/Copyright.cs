using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KiteLion {
    namespace Legal {
        public static class Copyright 
        {
            private static Dictionary<ILegal, UsageStats> KiteLionGamesSoftware = new Dictionary<ILegal, UsageStats>();

            private struct UsageStats {
                public DateTime UseDate;
                public string SoftwareName;
            }

            public static void RecordUsage (ILegal Obj) {
                UsageStats ObjStats = new UsageStats { UseDate = DateTime.Now, SoftwareName = Obj.KiteLionGamesSoftwareName};
                KiteLionGamesSoftware.Add(Obj, ObjStats);
            }
        }

        public interface ILegal{
            string KiteLionGamesSoftwareName { get; }
        }
    }
}
