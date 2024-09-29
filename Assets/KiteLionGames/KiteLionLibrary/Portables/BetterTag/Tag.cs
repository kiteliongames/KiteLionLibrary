#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Tag.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.13
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
using System.Linq;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using Unity.VisualScripting;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace KiteLionGames.KiteLionLibrary.Portables.BetterTag
{
    /// <summary>
    /// todo: On rightclick->find references: show all objects with this tag IN EDITOR!
    /// todo: subgroups
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        ///     Better Tag Extension
        /// </summary>
        /// <param name="tag">Better Tag label</param>
        /// <param name="includeInactive">True = include disabled gameobjects.</param>
        /// <returns>Gameobject containing a Tag component. Null if not found.</returns>
        public static GameObject FindGameObjectWithBetterTag(this GameObject gameObject, Tag.label tag, bool includeInactive = false)
        {
            //gameObject.scene.GetRootGameObjects().Select(x => x.FindGameObjectWithBetterTag(label, includeInactive)).Where(x => x != null).FirstOrDefault();
            return Tag.FindGameObjectWithBetterTag(tag, includeInactive);
        }

        /// <summary>
        ///     Better Tag Extension
        /// </summary>
        /// <param name="tag">Better Tag label</param>
        /// <param name="includeInactive">True = include disabled gameobjects.</param>
        /// <returns>Gameobject array containing all with Tag components. Null if not found.</returns>
        public static GameObject[] FindGameObjectsWithBetterTag(this GameObject gameObject, Tag.label tag, bool includeInactive = false)
        {
            return Tag.FindGameObjectsWithBetterTag(tag, includeInactive);
        }

        //GameObject Extension Methods
        /// <summary>
        ///     BetterTag Extension Method
        /// </summary>
        /// <returns>BetterTag Class, creates component if not found.</returns>
        public static Tag GetTag(this GameObject gameObject)
        {
            return Tag.GetTagComponent(gameObject);
        }

        /// <summary>
        ///     BetterTag Extension Method
        /// </summary>
        /// <returns>BetterTag Class, creates component if not found.</returns>
        public static Tag GetTag(this Transform transform)
        {
            return Tag.GetTagComponent(transform.gameObject);
        }

        /// <summary>
        ///     BetterTag Extension Method
        /// </summary>
        /// <returns>
        ///     BetterTag Class, creates component if not found. NULL if <see langword="static" /> property
        ///     ADD_COMPONENT_IF_NOT_FOUND = false.
        /// </returns>
        public static Tag GetTag(this MonoBehaviour monoBehaviour)
        {
            return Tag.GetTagComponent(monoBehaviour.gameObject);
        }

        /// <summary>
        ///     BetterTag Extension Method
        /// </summary>
        /// <returns>
        ///     BetterTag Class, creates component if not found. NULL if <see langword="static" /> property
        ///     ADD_COMPONENT_IF_NOT_FOUND = false.
        /// </returns>
        public static Tag GetTag(this Component component)
        {
            return Tag.GetTagComponent(component.gameObject);
        }
    }

    /// <summary>
    ///     This is the Better Tag asset class. A Tag is the component with one or more labels.
    ///     Flag and Label are used interchangeably.
    /// </summary>
    [InspectorLabel("Better Tag")]
    public class Tag : MonoBehaviour
    {
        #region MODIFY THESE TO ADD YOUR OWN

        /// <summary>
        ///     All possible labels. MODIFY THESE TO ADD YOUR OWN. Make sure to employ the bitshift method when adding new labels.
        ///     MAX 62 LABELS!
        /// </summary>
        [Flags]
        public enum label
        {
            Untagged = 0,//default, unavailable label
            Tools = 1 << 0,//available labels
            Locals = 1 << 1,
/*
            ___unused = 1 << 2,
*/
            MainCamera = 1 << 3,
            Player = 1 << 4,
            Enemy = 1 << 5,
            EnemyPlayZone = 1 << 6,
            EnemyHand = 1 << 7,
            PlayZoneBarrier = 1 << 8,
            Card = 1 << 9,
/*
            ___________unused = 1 << 10,
*/
            Deck = 1 << 11,
            Hand = 1 << 12,
            Dragger = 1 << 13,
/*
            _______________unused = 1 << 14,
            ________________unused = 1 << 15,
*/
            Tabletop = 1 << 16,
            /// <summary>
            /// Where the player's camera should go when spawning in world hub.
            /// </summary>
            CameraStartPosition = 1 << 17,
            CameraStartAnimator = 1 << 18,

            InvisibleWall = 1 << 19,
/*
            _____________________unused = 1 << 20,
*/
            Discard = 1 << 21,
            BlankSpace = 1 << 22,
            GameplayCode = 1 << 23,
/*
            _________________________unused = 1 << 24,
*/
            /// <summary>
            /// Tied to the quantum menu button that when fired, launches world hub.
            /// </summary>
            WorldHubButton = 1 << 25,
/*
            ___________________________unused = 1 << 26,
            ____________________________unused = 1 << 27,
            _____________________________unused = 1 << 28,
            ______________________________unused = 1 << 29,
            _______________________________unused = 1 << 30,
            ________________________________unused = 1 << 31,//last available label
*/
            Everything = ~0,//default, unavailable label //Max 62 available labels
        }

        #endregion

        /// <summary>
        ///     Adds Tag component to GameObject if not found.
        /// </summary>
        public const bool ADD_COMPONENT_IF_NOT_FOUND = true;

        [SerializeField]
        private label _flags = 0;

        private int _flagsCount;
        /// <summary>
        ///     returns bitmask enum _flags.
        /// </summary>
        public label Flags { get => _flags; }

        private void Start()
        {
            //Get the number of flags set
            _flagsCount = GetFlagCount();
        }

        /// <summary>
        ///     Counts total flags set.
        /// </summary>
        /// <returns>Total flags set.</returns>
        public int GetFlagCount()
        {
            var count = 0;
            var temp = _flags;
            while (temp != 0)
            {
                count += (int)temp & 1;
                temp = (label)((int)temp >> 1);
            }
            return count;
        }

        public static Tag GetTagComponent(GameObject Tagged)
        {
            var _tag = Tagged.GetComponent<Tag>();
            if (_tag == null)
            {
                CBUG.LogWarning($"Tagged {Tagged.name} does not have a Better Tag (Tag) component.");
                if (ADD_COMPONENT_IF_NOT_FOUND)
                {
                    CBUG.LogWarning($"Adding Better Tag (Tag) component to {Tagged.name}. This is default behavior. CreateTagComponent 'ADD_IF_NOT_SET' if you wish to change this.");
                    _tag = Tagged.AddComponent<Tag>();
                    return _tag;
                }
                return null;
            }
            return _tag;
        }

        public static Tag GetTagComponent(GameObject Tagged, bool createIfNotFound, label tag = label.Untagged)
        {
            if (createIfNotFound == false)
            {
                return GetTagComponent(Tagged);
            }
            var _tag = Tagged.GetComponent<Tag>();
            if (_tag == null)
            {
                _tag = Tagged.AddComponent<Tag>();
                // add new flag to enum variable label
                _tag._flags = tag;
            }
            return _tag;
        }

        public static void CreateTagComponent(GameObject TaglessObj, label flags)
        {
            var _tag = TaglessObj.GetComponent<Tag>();
            if (_tag == null)
            {
                _tag = TaglessObj.AddComponent<Tag>();
                _tag._flags = flags;
            }
            else
            {
                CBUG.LogWarning($"Tagged {TaglessObj.name} already has a Better Tag (Tag) component.");
            }
        }

        /// <summary>
        ///     Returns flags, if any.
        /// </summary>
        /// <param name="Tagged"> Gameobject containing a Better Tag (Tag) component.</param>
        /// <returns>label enum with all flags. Null if static property ADD_COMPONENT_IF_NOT_FOUND = false.</returns>
        public static label? Get(Transform Tagged)
        {
            return (label?)GetTagComponent(Tagged.gameObject);
        }

        /// <summary>
        ///     Returns flags, if any.
        /// </summary>
        /// <param name="Tagged"> Gameobject containing a Better Tag (Tag) component.</param>
        /// <returns>label enum with all flags. Null if static property ADD_COMPONENT_IF_NOT_FOUND = false. </returns>
        public static label? Get(GameObject Tagged)
        {
            return (label?)GetTagComponent(Tagged.gameObject);
        }

        /// <summary>
        ///     Returns a newly created array of all assigned labels.
        /// </summary>
        /// <returns>Array of set labels. Always returns at minimum Array size 1 (Untagged).</returns>
        public label[] GetLabels()
        {
            if (_flags == 0)
                return new[]
                {
                    label.Untagged,
                };


            var labels = new label[_flagsCount];

            var maxPossible = 31;
            var foundCount = 0;
            for (int i = 0, value = 0; i <= maxPossible; i++, value = 1 << i)
            {
                if (((int)_flags & value) != 0)
                {
                    labels[foundCount] = (label)value;
                    foundCount++;
                }
            }
            if (foundCount == 0)
                throw new Exception($"No labels found on {this.gameObject.name}. This shouldn't be possible!");
            if (foundCount != _flagsCount)
                throw new Exception($"Found {foundCount} labels on {this.gameObject.name}, but expected {_flagsCount}.");
            return labels;
        }

        /// <summary>
        ///     Returns first found label. Use this if you only have one label.
        /// </summary>
        /// <returns> Single label.</returns>
        public label Get()
        {
            return GetLabels()[0];
        }

        public override string ToString()
        {
            return $"{_flags}";
        }

        public static explicit operator label(Tag tag)
        {
            return tag._flags;
        }

        /// <summary>
        ///     Sets flags. Zeroes all flags then applies @flags.
        ///     Want one flag? Use this.
        /// </summary>
        /// <param name="flags"> New Tag labels.</param>
        public void OverwriteFlags(label flags)
        {
            _flags = flags;
            _flagsCount = GetFlagCount();
        }

        public void AddFlag(label flag)
        {
            if (ContainsFlag(flag))
            {
                CBUG.LogWarning($"Tag {flag} already exists on {this.gameObject.name}.");
            }
            else
            {
                _flags |= flag;
                _flagsCount++;
            }
        }

        /// <summary>
        /// Remove a flag.
        /// </summary>
        /// <param name="flag"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveFlag(label flag)
        {
            if (flag == label.Untagged)
                throw new Exception("Cannot remove Untagged label.");

            if (ContainsFlag(flag))
            {
                _flagsCount--;
                _flags &= ~flag;
            }
            else
            {
                CBUG.LogWarning($"Tag {flag} does not exist on {this.gameObject.name}.");
            }
        }

        public bool ContainsFlag(label flag)
        {
            return _flags.HasFlag(flag);
        }

        public bool HasFlag(label flag)
        {
            return ContainsFlag(flag);
        }

        private void AddTaggedGameObjectHelper()
        {
            var labels = this.gameObject.GetTag().GetLabels();
            for (var i = 0; i < labels.Length; i++)
            {
            }
        }

        #region STATICS

        /// <summary>
        /// Better Tag Extension, not performant don't run every frame.
        /// </summary>
        /// <param name="label">Better Tag label</param>
        /// <param name="includeInactive">True = include disabled gameobjects.</param>
        /// <returns>Gameobject containing a Tag component. Null if not found.</returns>
        public static GameObject FindGameObjectWithBetterTag(label label, bool includeInactive = false)
        {
            //gameObject.scene.GetRootGameObjects().Select(x => x.FindGameObjectWithBetterTag(label, includeInactive)).Where(x => x != null).FirstOrDefault();
            var tags = FindObjectsByType<Tag>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            return tags.FirstOrDefault(x => (label)x == label)?.gameObject;
        }

        /// <summary>
        ///     Better Tag Extension, not performant don't run every frame.
        /// </summary>
        /// <param name="label">Better Tag label</param>
        /// <param name="includeInactive">True = include disabled gameobjects.</param>
        /// <returns>Gameobject array containing all with Tag components. Null if not found.</returns>
        public static GameObject[] FindGameObjectsWithBetterTag(label label, bool includeInactive = false)
        {
            var allTagged = FindObjectsByType<Tag>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None) ?
                .Select(x => (label)x == label ? x.gameObject : null).ToArray();
            allTagged = allTagged.Where(x => x != null).ToArray();

            return allTagged;
        }

        public static Dictionary<label, GameObject> TAGGED_GAMEOBJECTS = new Dictionary<label, GameObject>();

        #endregion
    }
}
