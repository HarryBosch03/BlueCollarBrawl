using System;
using UnityEngine;

namespace Runtime.Units
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SpeedUnitAttribute : PropertyAttribute
    {
        public SpeedUnitAttribute.Unit defaultUnit;

        public SpeedUnitAttribute(Unit defaultUnit = Unit.Mps)
        {
            this.defaultUnit = defaultUnit;
        }
        
        
        public enum Unit
        {
            KmpH,
            Mps,
        }
    }
}