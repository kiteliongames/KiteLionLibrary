#region FileHeader

// test
// Project: Assembly-CSharp
// File:    TPSGenericController.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.06
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

/// <summary>
/// SanityTestCharacterController
/// License: GNU AGPLv3
/// Copyright (C) 2022 KiteLion Games
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <https://www.gnu.org/licenses/>.
///
/// Contact: support@kiteliongames.com
/// </summary>

using System.Collections.Generic;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Controllers.TPS
{
    public class TPSGenericController : MonoBehaviour
    {
        public float rotateSpeed = 6;
        public float forwardSpeed = 20;

        public float MouseSensitivity = 50.0f;

        public Transform TorsoTransform;
        public Transform BodyTransform;
        public Transform CameraTransform;
        private float _forwardInput;


        private bool _isBroken;


        private Vector2 _MouseChange;

        private float _sideInput;
        // Use this for initialization
        private void Start()
        {
            if (TorsoTransform == null)
            {
                _isBroken = true;
                Debug.Log("Torso is null.");
            }
            if (BodyTransform == null)
            {
                _isBroken = true;
                Debug.Log("Body is null.");
            }
            if (CameraTransform == null)
            {
                _isBroken = true;
                Debug.Log("Camera is null.");
            }

            if (_isBroken)
            {
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isBroken) return;

            List<string> a = new List<string>();

            _sideInput = Input.GetAxis("Horizontal");
            _forwardInput = Input.GetAxis("Vertical");

            #region Mouse stuff

            _MouseChange = Vector2.zero;
            _MouseChange.x = Input.GetAxis("Mouse X");
            _MouseChange.y = Input.GetAxis("Mouse Y");

            #endregion
        }

        private void FixedUpdate()
        {
            if (_isBroken) return;

            this.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 previousRotation = new Vector3(0, this.GetComponent<Rigidbody>().rotation.eulerAngles.y, 0);
            Vector3 newRotation = new Vector3(0, previousRotation.y + _sideInput * rotateSpeed, 0);

            this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(newRotation);
            this.GetComponent<Rigidbody>().velocity = this.transform.forward * forwardSpeed * _forwardInput;

            this.GetComponent<Rigidbody>().isKinematic = false;

            // Mouse stuff
            var newCameraRotX = CameraTransform.rotation.eulerAngles.x - _MouseChange.y * MouseSensitivity * Time.deltaTime;
            if (newCameraRotX > 90.0f && newCameraRotX < 180.0f)
                newCameraRotX = 90.0f;
            if (newCameraRotX < 270.0f && newCameraRotX > 180.0f)
                newCameraRotX = 270.0f;
            var newCameraRotY = CameraTransform.rotation.eulerAngles.y + _MouseChange.x * MouseSensitivity * Time.deltaTime;
            CameraTransform.rotation = Quaternion.Euler(newCameraRotX, newCameraRotY, CameraTransform.rotation.eulerAngles.z);
        }
    }
}
