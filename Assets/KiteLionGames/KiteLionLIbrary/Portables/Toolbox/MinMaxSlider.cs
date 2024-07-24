//MinMaxSlider Editor Script
using UnityEngine;

namespace KiteLionGames.Utilities.Editor
{
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;

        public MinMaxSliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

}

#if UNITY_EDITOR
namespace KiteLionGames.Utilities.Editor
{
    using UnityEditor;
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public float labelWidth = 100;
        public float floatFieldWidth = 50;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 range = property.vector2Value;
                float min = range.x;
                float max = range.y;
                MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;
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
}
#endif