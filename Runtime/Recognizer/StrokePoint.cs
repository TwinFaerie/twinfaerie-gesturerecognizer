using System;
using UnityEngine;

namespace TF.GestureRecognizer.Recognizer
{
    [Serializable]
    public struct StrokePoint
    {
        public SerializedVector2 Point;
        public int StrokeIndex;

        public StrokePoint(float pointX, float pointY, int strokeIndex)
        {
            Point = new SerializedVector2(pointX, pointY);
            StrokeIndex = strokeIndex;
        }
        
        public StrokePoint(Vector2 point, int strokeIndex)
        {
            Point = point;
            StrokeIndex = strokeIndex;
        }
    }
}