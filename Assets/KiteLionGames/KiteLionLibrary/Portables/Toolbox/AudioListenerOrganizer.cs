#region FileHeader
// Project: Portables
// File:    AudioListenerOrganizer.cs
// Author:  Eliot CS
// Created: 2024.09.24.04.09.24
// Edited: 2024.09.24.04.09.24
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

using System;
using System.Collections.Generic;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    /// <summary>
    ///     Sometimes we WILL have multiple audio listeners at once for a frame.
    /// This is for organizing that edge case.
    /// </summary>
    [RequireComponent(typeof(AudioListener))]
    public class AudioListenerOrganizer : MonoBehaviour
    {
        #region Static Members

        private static List<AudioListenerOrganizer> Listeners { get; set; } = new List<AudioListenerOrganizer>();
        private static Action _onListenerAdded;
        private static Action _onListenerRemoved;
        #endregion

        #region Serialized Members

        [field: SerializeField]
        private ushort Priority { get; set; }

        #endregion

        #region Non-Private Members

        protected void Awake()
        {
            _audioListener = GetComponent<AudioListener>();
            _audioListener.enabled = false;
            Listeners.Add(this);
            _onListenerAdded += SetEnabledDisabledHelper;
            _onListenerRemoved += SetEnabledDisabledHelper;
            _onListenerAdded?.Invoke();
        }

        protected void OnDestroy()
        {
            Listeners.Remove(this);
            _onListenerAdded -= SetEnabledDisabledHelper;
            _onListenerRemoved -= SetEnabledDisabledHelper;
            _onListenerRemoved?.Invoke();
        }

        #endregion

        #region Private Members

        private AudioListener _audioListener;
        private void SetEnabledDisabledHelper()
        {
            AudioListenerOrganizer highestPriorityListener = null;
            ushort highestFound = 0;
            foreach (var organizer in Listeners)
            {
                if (highestFound < organizer.Priority)
                {
                    highestPriorityListener = organizer;
                    highestFound = organizer.Priority;
                }
            }
            if (highestPriorityListener == null)
            {
                CBUG.LogError("No AudioListenerOrganizer found. This shouldn't happen.");
            }
            else if (highestPriorityListener == this)
            {
                _audioListener.enabled = true;
            }
            else
            {
                _audioListener.enabled = false;
            }
        }

        #endregion
    }
}
