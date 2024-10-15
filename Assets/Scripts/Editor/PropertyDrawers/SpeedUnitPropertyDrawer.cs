using System;
using Runtime.Units;
using UnityEditor;
using UnityEngine;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SpeedUnitAttribute))]
    public class SpeedUnitPropertyDrawer : PropertyDrawer
    {
        public SpeedUnitAttribute.Unit unit;
        public bool initialized;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type != "float")
            {
                GUI.Label(position, "SpeedUnit Attribute must be on type float");
                return;
            }
            
            
            if (!initialized)
            {
                unit = ((SpeedUnitAttribute)attribute).defaultUnit;
                initialized = true;
            }

            position.width -= 62f;
            var unitScale = unit switch
            {
                SpeedUnitAttribute.Unit.KmpH => 3.6f,
                SpeedUnitAttribute.Unit.Mps => 1f,
                _ => throw new ArgumentOutOfRangeException()
            };

            EditorGUI.BeginChangeCheck();
            var value = property.floatValue * unitScale;
            value = EditorGUI.FloatField(position, label, value);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = value / unitScale;
            }
            
            position.x += position.width + 2f;
            position.width = 60f;
            if (GUI.Button(position, unit.ToString()))
            {
                var count = Enum.GetValues(typeof(SpeedUnitAttribute.Unit)).Length;
                if ((int)unit == count - 1) unit = 0;
                else unit++;
            }
        }
    }
}