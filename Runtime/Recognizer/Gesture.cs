using System;
using System.Collections.Generic;

namespace TF.GestureRecognizer.Recognizer
{
    [Serializable]
    public class Gesture
    {
        public readonly string Name;
        public readonly List<StrokePoint> PointList;

        public Gesture(string name, List<StrokePoint> pointList)
        {
            Name = name;
            PointList = pointList;
        }
    }
}