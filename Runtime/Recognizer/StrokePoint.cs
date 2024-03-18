using System;
using UnityEngine;

namespace TF.GestureRecognizer.Recognizer
{
    [Serializable]
    public struct StrokePoint
    {
        public Vector2 Point;
        public int StrokeIndex;

        public StrokePoint(float pointX, float pointY, int strokeIndex)
        {
            Point = new Vector2(pointX, pointY);
            StrokeIndex = strokeIndex;
        }
        
        public StrokePoint(Vector2 point, int strokeIndex)
        {
            Point = point;
            StrokeIndex = strokeIndex;
        }
    }
}