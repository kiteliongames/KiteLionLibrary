#region FileHeader

// test
// Project: Assembly-CSharp
// File:    PlaneControls.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.19
//
// Copyright (c) 2024 SomeGameDevs, LLC. All rights reserved.
//
// This source code is the property of SomeGameDevs, LLC and may not be
// copied, distributed, modified, or used in any way without prior written
// permission from SomeGameDevs, LLC.
//
// Description:
// [Provide a brief description of what this file/class does.]
//
// Previous Header (if any):
//
// License:
// This code is provided "as is," without warranty of any kind, express or
// implied, including but not limited to the warranties of merchantability,
// fitness for a particular purpose, and noninfringement. In no event shall
// the authors or copyright holders be liable for any claim, damages, or
// other liability, whether in an action of contract, tort, or otherwise,
// arising from, out of, or in connection with the software or the use or
// other dealings in the software.

#endregion

using UnityEngine;
using UnityEngine.UI;

namespace KiteLionGames.KiteLionLibrary.Portables.Controllers.Flying
{
    public class PlaneControls : MonoBehaviour
    {
        public float TargetRotZ;
        public float TargetRotX;
        public float RotSpeedZ;
        public float RotSpeedX;
        public float MovSpeedX;
        public float MovSpeedY;
        public float Ceiling;
        public float Floor;
        public float LeftWall;
        public float RightWall;

        public float OpeningSpeed;
        public float TextFadeInTime;
        public float TextFadeInSpeed;
        public float ClosingSpeed;
        public float TextFadeOutTime;
        public float TextFadeOutSpeed;

        public float TotalPlayTime;

        public Text TextToFade;
        public GameObject EndGameText;
        public string FriendCode;

        private bool canFade;
        private bool closing;
        private float currentFogEnd;
        private Vector3 currentRot;
        private Vector3 currentSpd;

        private bool fadeTextIn;
        private bool fadeTextOut;

        private float fogEnd;
        private Vector3 movSpeedXVec;
        private Vector3 movSpeedYVec;
        private bool opening;

        private Rigidbody r;
        private Vector3 rotSpeedXVec;
        private Vector3 rotSpeedZVec;
        private bool textFadedIn;
        private bool textFadedOut;

        // Use this for initialization
        private void Start()
        {
            r = this.GetComponent<Rigidbody>();
            currentRot = r.rotation.eulerAngles;
            currentSpd = r.velocity;
            rotSpeedXVec = new Vector3(RotSpeedX, 0f, 0f);
            rotSpeedZVec = new Vector3(0, 0f, RotSpeedZ);
            movSpeedXVec = new Vector3(MovSpeedX, 0f, 0f);
            movSpeedYVec = new Vector3(0f, MovSpeedY, 0f);
            fogEnd = RenderSettings.fogEndDistance;
            currentFogEnd = 0f;

            opening = true;
            closing = false;

            fadeTextOut = false;
            fadeTextIn = false;
            textFadedIn = false;
            textFadedOut = false;

            TextToFade.CrossFadeColor(Color.clear, 0f, true, true, true);
            canFade = false;
            if (Application.absoluteURL.Contains(FriendCode) || Application.isEditor)
                canFade = true;
        }

// Update is called once per frame
        private void Update()
        {
            if (opening && currentFogEnd <= fogEnd * 0.95f)
            {
                currentFogEnd = Mathf.Lerp(currentFogEnd, fogEnd, Time.deltaTime / OpeningSpeed);
                RenderSettings.fogEndDistance = currentFogEnd;
            }
            else if (opening)
            {
                currentFogEnd = fogEnd;
                RenderSettings.fogEndDistance = currentFogEnd;
                opening = false;
            }

            if (!opening && !closing && Time.time >= TotalPlayTime)
            {
                closing = true;
            }

            if (canFade && !textFadedIn && Time.time >= TextFadeInTime)
                fadeTextIn = true;

            if (canFade && !textFadedOut && Time.time >= TextFadeOutTime)
                fadeTextOut = true;


            if (Time.time > TotalPlayTime + ClosingSpeed)
                EndGameText.SetActive(true);

            if (fadeTextIn)
            {
                fadeTextIn = false;
                textFadedIn = true;
                TextToFade.CrossFadeColor(Color.white, TextFadeInSpeed, true, true, true);
            }

            if (fadeTextOut)
            {
                fadeTextOut = false;
                textFadedOut = true;
                TextToFade.CrossFadeColor(Color.clear, TextFadeOutSpeed, true, true, true);
            }

            if (closing && currentFogEnd >= RenderSettings.fogStartDistance)
            {
                currentFogEnd = Mathf.Lerp(currentFogEnd, RenderSettings.fogStartDistance, Time.deltaTime / ClosingSpeed);
                RenderSettings.fogEndDistance = currentFogEnd;
            }
            else if (closing)
            {
                RenderSettings.fogEndDistance = 0f;
                closing = false;
            }


            currentSpd = Vector3.zero;
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (currentRot.z >= -TargetRotZ)
                    currentRot -= rotSpeedZVec;
                if (this.transform.position.x <= RightWall)
                    currentSpd += movSpeedXVec;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (currentRot.z <= TargetRotZ)
                    currentRot += rotSpeedZVec;
                if (this.transform.position.x >= LeftWall)
                    currentSpd -= movSpeedXVec;
            }
            else
            {
                if (currentRot.z > 0f)
                    currentRot -= rotSpeedZVec;
                if (currentRot.z < 0f)
                    currentRot += rotSpeedZVec;
            }
            if (Input.GetAxis("Vertical") > 0)
            {
                if (currentRot.x >= -TargetRotX)
                    currentRot -= rotSpeedXVec;
                if (this.transform.position.y <= Ceiling)
                    currentSpd += movSpeedYVec;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                if (currentRot.x <= TargetRotX)
                    currentRot += rotSpeedXVec;
                if (this.transform.position.y >= Floor)
                    currentSpd -= movSpeedYVec;
            }
            else
            {
                if (currentRot.x > 0f)
                    currentRot -= rotSpeedXVec;
                if (currentRot.x < 0f)
                    currentRot += rotSpeedXVec;
            }
            r.rotation = Quaternion.Euler(currentRot);
            r.velocity = currentSpd;
        }
    }
}
