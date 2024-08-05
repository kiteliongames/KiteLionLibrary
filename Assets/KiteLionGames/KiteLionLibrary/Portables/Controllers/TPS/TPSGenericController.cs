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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiteLionGames;

public class TPSGenericController : MonoBehaviour
{
    public float rotateSpeed = 6;
    public float forwardSpeed = 20;

    public float MouseSensitivity = 50.0f;

    public Transform TorsoTransform;
    public Transform BodyTransform;
    public Transform CameraTransform;

    float _sideInput = 0;
    float _forwardInput = 0;


    Vector2 _MouseChange;


    bool _isBroken = false;
    // Use this for initialization
    void Start()
    {
        if (TorsoTransform == null) 
        {
            _isBroken = true;
            Debug.Log("Torso is null.");
        }
        if(BodyTransform == null)
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
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBroken) return;

        List<string> a = new();
        
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

        GetComponent<Rigidbody>().isKinematic = true;
        Vector3 previousRotation = new(0, GetComponent<Rigidbody>().rotation.eulerAngles.y, 0);
        Vector3 newRotation = new(0, previousRotation.y + _sideInput * rotateSpeed, 0);

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(newRotation);
        GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed * _forwardInput;

        GetComponent<Rigidbody>().isKinematic = false;

        // Mouse stuff
        float newCameraRotX = CameraTransform.rotation.eulerAngles.x - _MouseChange.y * MouseSensitivity * Time.deltaTime;
        if (newCameraRotX > 90.0f && newCameraRotX < 180.0f)
            newCameraRotX = 90.0f;
        if (newCameraRotX < 270.0f && newCameraRotX > 180.0f)
            newCameraRotX = 270.0f;
        float newCameraRotY = CameraTransform.rotation.eulerAngles.y + _MouseChange.x * MouseSensitivity * Time.deltaTime;
        CameraTransform.rotation = Quaternion.Euler(newCameraRotX, newCameraRotY, CameraTransform.rotation.eulerAngles.z);

    }
}

