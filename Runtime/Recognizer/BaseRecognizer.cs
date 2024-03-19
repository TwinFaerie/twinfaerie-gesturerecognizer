using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TF.GestureRecognizer.Recognizer
{
    public abstract class BaseRecognizer
    {
        public abstract List<StrokePoint> Normalize(IList<StrokePoint> pointList, int total);
        public abstract Result Recognize(IList<StrokePoint> pointList, int total, GestureLibrary gestureLibrary);

        #region Recognize Variation
        
        public Result Recognize(IList<StrokePoint> pointList, int total, Gesture gesture)
        {
            return Recognize(pointList, total, new GestureLibrary(new List<Gesture> { gesture }));
        }
        
        public Result Recognize(IList<StrokePoint> pointList, int total, GestureLibrary gestureLibrary, string name)
        {
            return Recognize(pointList, total, new GestureLibrary(new List<Gesture>(gestureLibrary.GestureList.Where(gesture => gesture.Name == name))));
        }

        #endregion Recognize Variation
        
        public static List<StrokePoint> Resample(IList<StrokePoint> pointList, int total)
        {
            var interval = GetPathLength(pointList) / (total - 1);
            var distance = 0f;

            var newPointList = new List<StrokePoint> { pointList.First() };

            var strokeIndex = 0;
            var checkPointList = pointList.Where(point => point.StrokeIndex == strokeIndex).ToList();
            
            while (checkPointList.Count > 1)
            {
                for (var i = 1; i < checkPointList.Count; i++)
                {
                    var lastPoint = checkPointList[i - 1];
                    var currentPoint = checkPointList[i];

                    var segmentLength = Vector2.Distance(lastPoint.Point, currentPoint.Point);

                    if (distance + segmentLength >= interval)
                    {
                        while (distance + segmentLength >= interval)
                        {
                            var t = Math.Clamp((interval - distance) / segmentLength, 0f, 1f);

                            if (float.IsNaN(t))
                            {
                                t = 0.5f;
                            }

                            var x = lastPoint.Point.x + t * (currentPoint.Point.x - lastPoint.Point.x);
                            var y = lastPoint.Point.y + t * (currentPoint.Point.y - lastPoint.Point.y);

                            var approximate = new StrokePoint
                            {
                                Point = new Vector2(x, y),
                                StrokeIndex = lastPoint.StrokeIndex
                            };

                            newPointList.Add(approximate);

                            segmentLength = distance + segmentLength - interval;
                            distance = 0f;
                            lastPoint = newPointList.Last();
                        }

                        distance = segmentLength;
                    }
                    else
                    {
                        distance += segmentLength;
                    }
                }
                
                checkPointList = pointList.Where(point => point.StrokeIndex == ++strokeIndex).ToList();
            }
            
            if (distance > 0f)
            {
                newPointList.Add(pointList[^1]);
            }

            return newPointList;
        }

        protected static Vector2 GetCenterPoint(IList<StrokePoint> pointList)
        {
            var x = pointList.Sum(point => point.Point.x) / pointList.Count;
            var y = pointList.Sum(point => point.Point.y) / pointList.Count;
            return new Vector2(x, y);
        }

        private static float GetPathLength(IList<StrokePoint> pointList)
        {
            float length = 0;

            for (var i = 1; i < pointList.Count; i++)
            {
                var previous = pointList[i - 1];
                var current = pointList[i];
                var distance = Vector2.Distance(previous.Point, current.Point);

                if (!float.IsNaN(distance) && previous.StrokeIndex == current.StrokeIndex)
                {
                    length += distance;
                }
            }

            return length;
        }
    }
}