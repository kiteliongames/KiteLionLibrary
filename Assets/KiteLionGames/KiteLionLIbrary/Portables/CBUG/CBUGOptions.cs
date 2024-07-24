using UnityEngine;

namespace KiteLionGames.BetterDebug
{
    public class CBUGOptions : MonoBehaviour
    {

        #region Public Unity-Assigned Vars
        public bool isEnabledOnScreen = true;
        public bool isEnabledOnConsole = true;
        public bool isEnabledForUnityLog = true;
        public bool isEnabledForEditor = true;
        public bool isEnabledForDevelopmentBuild = true;
        public bool isEnabledForReleaseBuild = false;
        //private bool    _enabledForDevelopmentBuild = true; //unused
        //private bool    _enabledForReleaseBuild     = true; //unused
        public float clearLineTimeInSeconds = 10;
        public bool clearNow = false;
        public int maxCharactersPerLine = 100;
        #endregion
    }
}