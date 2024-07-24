/// <summary>
/// RotateMe
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

public class RotateMe : MonoBehaviour
{

    private Vector3 myRotation;
    public float RotSpeed;
    public float RotSpeedX;
    public float RotSpeedY;
    public float RotSpeedZ;

    private float x;
    private float y;
    private float z;

    // Use this for initialization
    void Start()
    {
        x = 0f;
        y = 0f;
        z = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        x += RotSpeed + RotSpeedX;
        y += RotSpeed + RotSpeedY;
        z += RotSpeed + RotSpeedZ;

        myRotation.Set(x, y, z);
        gameObject.transform.rotation = Quaternion.Euler(myRotation);
    }
}
