using System;
using UnityEngine;

namespace com.vwds.arika
{
    [CreateAssetMenu(fileName = "New ARIKA Avatar", menuName = "ARIKA/Avatar", order = 0)]
    public class Avatar : ScriptableObject
    {
        public string Name;
        public float Height;
    }
    [Serializable]
    public struct AvatarProperty
    {
        public string blendName;
        [RangeAttribute(0f, 100f)]
        public float value;
    }
}