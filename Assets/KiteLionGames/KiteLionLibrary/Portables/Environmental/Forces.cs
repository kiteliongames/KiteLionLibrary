#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Forces.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.20
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

namespace KiteLionGames.KiteLionLibrary.Portables.Environmental
{
    public static class Forces
    {
        /// <summary>
        ///     todo description
        /// </summary>
        public delegate Vector3 ExternalForce();
        public static readonly float G = 9.81f;

        /// <summary>
        ///     todo NAME
        /// </summary>
        public static Vector3 Gravity3 { get => Vector3.down * G; }
        //public delegate Vector3 ExternalForce (bool isPermanent, Vector3 initialForce);
        //public delegate Vector3 ExternalForce (bool isPermanent, Vector3 initialForce, float[,] xPosTime, float[,] yPosTime, float[,] zPosTime);
        //public delegate Vector3 ExternalForce (bool isPermanent, Vector3 initialForce, float[,] xPosTime, float[,] yPosTime, float[,] zPosTime, float endTime);

        //public static Vector3 GetGravity() {
        //    CBUG.Do("Gravity is: " + Gravity3.ToString());
        //    return Gravity3;
        //}
    }
}
