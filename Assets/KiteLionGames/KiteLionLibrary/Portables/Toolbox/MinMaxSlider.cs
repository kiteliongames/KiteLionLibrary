#region FileHeader

// test
// Project: Assembly-CSharp
// File:    MinMaxSlider.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.08
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

//MinMaxSlider Editor Script
using UnityEditor;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float max;
        public readonly float min;

        public MinMaxSliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public float floatFieldWidth = 50;
        public float labelWidth = 100;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                var range = property.vector2Value;
                var min = range.x;
                var max = range.y;
                var attr = this.attribute as MinMaxSliderAttribute;
                if (attr != null)
                {
                    if (attr.min <= attr.max)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.LabelField(label, GUILayout.MaxWidth(labelWidth));
                        min = EditorGUILayout.FloatField(min, GUILayout.MaxWidth(floatFieldWidth));
                        if (min < attr.min)
                        {
                            min = attr.min;
                        }
                        EditorGUILayout.MinMaxSlider(ref min, ref max, attr.min, attr.max, GUILayout.ExpandWidth(true));
                        max = EditorGUILayout.FloatField(max, GUILayout.MaxWidth(floatFieldWidth));
                        if (max > attr.max)
                        {
                            max = attr.max;
                        }
                        EditorGUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            //start horizontal
                            range.x = min;
                            range.y = max;
                            property.vector2Value = range;
                        }
                    }
                    else
                    {
                        EditorGUI.LabelField(position, label, "Min should be less than Max");
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, label, "Use only with Vector2");
            }
        }
    }
#endif
}
