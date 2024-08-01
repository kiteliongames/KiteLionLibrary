using KiteLionGames.BetterTag;
using UnityEngine;

namespace KiteLionGames.Common
{
    public static class GameObjectHelper
    {
        public static GameObject FindGameObjectWithTag(Tag.label label)
        {
            return Tag.FindGameObjectWithBetterTag(label);
        }

        public static GameObject[] FindGameObjectsWithTag(Tag.label label)
        {
            return Tag.FindGameObjectsWithBetterTag(label);
        }
    }
}
