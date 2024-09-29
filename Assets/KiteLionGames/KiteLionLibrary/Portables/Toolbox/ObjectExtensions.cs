#region FileHeader
// Project: Portables
// File:    ObjectExtensions.cs
// Author:  Eliot CS
// Created: 2024.09.26.00.09.36
// Edited: 2024.09.26.00.09.37
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
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    /// <summary>
    ///     Default Class Script Description
    /// </summary>
    public static class ObjectExtensions
    {
        #region Static Members

        /// <summary>
        /// Returns type if found, otherwise throws an error an reports via CBUG. A good validator.
        /// NOT PERFORMANT.
        /// </summary>
        /// <typeparam name="T"> Existing component in scene, active OR inactive.</typeparam>
        /// <returns>The found type.</returns>
        /// <exception cref="NotSupportedException">If count != 1 found, this is an NotSupportedException.</exception>
        public static T FindSingleOrThrow<T>(this Object o) where T : Component
        {
            return FindSingleOrThrow<T>();
        }

        /// <inheritdoc cref="FindSingleOrThrow{T}(Object)" />
        public static T FindSingleOrThrow<T>() where T : Component
        {
            var find = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (find == null || find.Length == 0)
            {
                CBUG.LogError($"FindSingleOrThrow failed, found no {typeof(T)}");
            }
            else if (find.Length > 1)
            {
                CBUG.LogError($"FindSingleOrThrow failed, found multiple {typeof(T)}. Count: {find.Length}");
            }
            else
            {
                return find[0];
            }
            return null;
        }

        #endregion
    }
}
