#region FileHeader

// test
// Project: Assembly-CSharp
// File:    _MusicManager.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.53
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

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// Simple Audio helper class to make audio calls easily without clunky reference holding.
///// Helper classes like these should not be referenced continuously.
///// </summary>
//public class _Audio : MonoBehaviour {

//    private Master M;
//    private bool isNewMusicIncoming;
//    private const string myTag = "AudioHelper";

//    private void Awake()
//    {
//        M = GameObject.FindGameObjectWithTag("Master").GetComponent<Master>();
//    }

//    // Use this for initialization
//    void Start () {
//        isNewMusicIncoming = false;
//        tag = _Audio.myTag;
//  }

//    #region Public Methods
//    //--Static Helpers
//    /// <summary>
//    /// Mostly unused outside of static helper functions. Given public access for convenience.
//    /// </summary>
//    /// <returns></returns>
//    public static _Audio GetRef()
//    {
//        return GameObject.FindGameObjectWithTag(_Audio.myTag).GetComponent<_Audio>();
//    }

//    /// <summary>
//    /// Plays a sound effect. Unless "ChangeMusic()" is called, then swaps music once.
//    /// </summary>
//    /// <param name="audNum">Refers to the location of the audio file within the sfx/msx array.</param>
//    public static void Play(int audNum)
//    {
//        _Audio.GetRef()._play(audNum);
//    }

//    /// <summary>
//    /// Next time _Audio.Play(x) is called, it'll swap the music instead.
//    /// </summary>
//    public static void ChangeMusic()
//    {
//        _Audio.GetRef().isNewMusicIncoming = true;
//    }
//    #endregion

//    #region Private Helper Methods
//    /// <summary>
//    /// Helper function to "_Audio.Play(x)"
//    /// "Plays a sound effect. Unless "ChangeMusic()" is called, then swaps music once.'
//    /// </summary>
//    /// <param name="audNum">Refers to the location of the audio file within the sfx/msx array.</param>
//    private void _play(int audNum)
//    {
//        if (isNewMusicIncoming) {
//            isNewMusicIncoming = false;
//            M.PlayMSX(audNum);
//            return;
//        }
//        M.PlaySFX(audNum);
//    }
//    #endregion
//}//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// Simple Audio helper class to make audio calls easily without clunky reference holding.
///// Helper classes like these should not be referenced continuously.
///// </summary>
//public class _Audio : MonoBehaviour {

//    private Master M;
//    private bool isNewMusicIncoming;
//    private const string myTag = "AudioHelper";

//    private void Awake()
//    {
//        M = GameObject.FindGameObjectWithTag("Master").GetComponent<Master>();
//    }

//    // Use this for initialization
//    void Start () {
//        isNewMusicIncoming = false;
//        tag = _Audio.myTag;
//  }

//    #region Public Methods
//    //--Static Helpers
//    /// <summary>
//    /// Mostly unused outside of static helper functions. Given public access for convenience.
//    /// </summary>
//    /// <returns></returns>
//    public static _Audio GetRef()
//    {
//        return GameObject.FindGameObjectWithTag(_Audio.myTag).GetComponent<_Audio>();
//    }

//    /// <summary>
//    /// Plays a sound effect. Unless "ChangeMusic()" is called, then swaps music once.
//    /// </summary>
//    /// <param name="audNum">Refers to the location of the audio file within the sfx/msx array.</param>
//    public static void Play(int audNum)
//    {
//        _Audio.GetRef()._play(audNum);
//    }

//    /// <summary>
//    /// Next time _Audio.Play(x) is called, it'll swap the music instead.
//    /// </summary>
//    public static void ChangeMusic()
//    {
//        _Audio.GetRef().isNewMusicIncoming = true;
//    }
//    #endregion

//    #region Private Helper Methods
//    /// <summary>
//    /// Helper function to "_Audio.Play(x)"
//    /// "Plays a sound effect. Unless "ChangeMusic()" is called, then swaps music once.'
//    /// </summary>
//    /// <param name="audNum">Refers to the location of the audio file within the sfx/msx array.</param>
//    private void _play(int audNum)
//    {
//        if (isNewMusicIncoming) {
//            isNewMusicIncoming = false;
//            M.PlayMSX(audNum);
//            return;
//        }
//        M.PlaySFX(audNum);
//    }
//    #endregion
//}
