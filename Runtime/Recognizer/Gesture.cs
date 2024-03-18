using System;
using System.Collections.Generic;

namespace TF.GestureRecognizer.Recognizer
{
    [Serializable]
    public class Gesture
    {
        public readonly string ID;
        public readonly string Name;
        public readonly List<StrokePoint> PointList;

        public Gesture(string name, List<StrokePoint> pointList)
        {
            ID = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            Name = name;
            PointList = pointList;
        }
    }
}