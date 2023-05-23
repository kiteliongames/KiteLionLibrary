using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Forces
{
    public static float G = 9.81f;

    /// <summary>
    /// todo NAME
    /// </summary>
    public static Vector3 Gravity3 { get => Vector3.down * G; }


    /// <summary>
    /// todo description
    /// </summary>
    public delegate Vector3 ExternalForce ();
    //public delegate Vector3 ExternalForce (bool isPermanent, Vector3 initialForce);
    //public delegate Vector3 ExternalForce (bool isPermanent, Vector3 initialForce, float[,] xPosTime, float[,] yPosTime, float[,] zPosTime);
    //public delegate Vector3 ExternalForce (bool isPermanent, Vector3 initialForce, float[,] xPosTime, float[,] yPosTime, float[,] zPosTime, float endTime);

    //public static Vector3 GetGravity() {
    //    CBUG.Do("Gravity is: " + Gravity3.ToString());
    //    return Gravity3;
    //}

}
