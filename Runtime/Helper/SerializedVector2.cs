using System;
using UnityEngine;

namespace TF.GestureRecognizer
{
    [Serializable]
    public class SerializedVector2
    {
        public float x;
        public float y;

        public SerializedVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static implicit operator SerializedVector2(Vector2 original)
        {
            return new SerializedVector2(original.x, original.y);
        }

        public static implicit operator Vector2(SerializedVector2 serialized)
        {
            return serialized.ToVector2();
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}