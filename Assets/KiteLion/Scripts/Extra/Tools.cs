using KiteLion.Debugging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A set of easily callable functions built to be used through a singleton in Unity.
/// Current Functionalities:
///  - Delay an animation
///  - Delay a function
///  - Fade a Text object.
/// 
/// Contributors:
///  - Eliot Leo Carney-Seim
///  
/// Credit:
/// https://benjaminbeagley.wordpress.com/2013/10/14/calling-a-static-ienumerator-in-unity/
///  
/// Further Reading:
/// http://wiki.unity3d.com/index.php/Singleton
/// http://wiki.unity3d.com/index.php/Toolbox
/// </summary>
namespace KiteLion.Common {
    public class Tools : MonoBehaviour {

        public delegate void VanillaFunction();

        void Awake() {
            if (instance == null) {
                instance = this as Tools;
            }
            tag = "Tools";
            DontDestroyOnLoad(gameObject);
        }

        #region Singleton Control
        private int priority;

        public static Tools instance {
            get {
                Tools currentInstance = GameObject.FindObjectOfType(typeof(Tools)) as Tools;
                if (currentInstance == null) {
                    currentInstance = new GameObject("Tools").AddComponent<Tools>();

                }
                return currentInstance;
            }

            set {
                instance = value;
            }
        }

        #endregion

        protected Tools ()
        {
            //No instantiation!
        }

        #region Public Static Hooks
        public static string RemoveWhitespace(string inString) {
            int inputLen = inString.Length;
            string outString = inString;

            int charPos = -1;
            foreach (char ch in inString) {
                charPos++;
                if (char.IsWhiteSpace(ch)) {
                    outString = outString.Replace(inString.Substring(charPos, 1), "");
                }
            }

            return outString;
        }

        /// <summary>
        /// For delaying a function call by a single frame.
        /// </summary>
        /// <param name="Call">Parameter-less function</param>
        /// <param name="Time">How long to delay by. To delay by 1 frame, Time == 0.</param>
        /// <returns></returns>
        public static void DelayFunction(VanillaFunction Call, float Time)
        {
            instance.StartCoroutine(instance._delayFunction(Call, Time));
        }

        /// <summary>
        /// For delaying an animation call by a single frame.
        /// </summary>
        /// <param name="Call">Animation object</param>
        /// <param name="Time">How long to delay by. To delay by 1 frame, Time == 0.</param>
        /// <returns></returns>
        public static void DelayAnim(Animator anim, float time, 
            string clipName = "",
            string trigger = "",
            float floatParam = -1f,
            int intParam = -1,
            bool boolParam = false,
            bool hasBoolParam = false)
        {
            instance.StartCoroutine(instance._delayAnim( anim, time, clipName,
                    trigger: trigger,
                    floatParam: floatParam,
                    intParam: intParam,
                    boolParam: boolParam,
                    hasBoolParam: hasBoolParam
                )
            );
        }

        /// <summary>
        /// Returns Tools class. GetComponent<Tools>() supplies the same functionality.
        /// </summary>
        /// <returns></returns>
        public static Tools GetSelf() {
            return GameObject.FindGameObjectWithTag("Tools").GetComponent<Tools>()._getSelf();
        }
        #endregion


        #region Private Core Functionality 
        private IEnumerator _delayAnim(Animator anim, float time, 
            string clipName = "",
            string trigger = "",
            float floatParam = -1f,
            int intParam = -1,
            bool boolParam = false,
            bool hasBoolParam = false)
        {
            if (time <= 0f)
                yield return 0f;
            else
                yield return new WaitForSeconds(time);

            if (clipName != "" && trigger == "")
                CBUG.SrsError("clipName Required!");

            if (hasBoolParam)
                anim.SetBool(clipName, boolParam);
            else if (trigger != "")
                anim.SetTrigger(trigger);
            else if (floatParam != -1f)
                anim.SetFloat(clipName, floatParam);
            else if (intParam != -1)
                anim.SetFloat(clipName, intParam);
        }

        private IEnumerator _delayFunction(VanillaFunction call, float time)
        {
            if (time <= 0f)
                yield return 0f;
            else
                yield return new WaitForSeconds(time);
            call();
        }

        private Tools _getSelf()
        {
            return this;
        }
        #endregion

        #region Helper Functions
        private void _fadeText(Text textToFade, float from, float to, float speed)
        {

        }
        #endregion

        #region Getters and Setters
        public int Priority {
            set {
                priority = value;
            }
        }
        #endregion
    }
}
