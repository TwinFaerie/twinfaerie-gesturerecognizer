using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TF.GestureRecognizer.Recognizer
{
    public class DollarOneRecognizer : BaseRecognizer
    {
        private const int scaledSize = 250;

        public override List<StrokePoint> Normalize(IList<StrokePoint> pointList, int total)
        {
            var resampled = Resample(pointList, total);
            var rotatedPoints = RotateToZero(resampled);
            var scaledPoints = ScaleToSquare(rotatedPoints, scaledSize);
            var translatedToOrigin = TranslateToOrigin(scaledPoints);
            
            return translatedToOrigin;
        }

        #region Rotate
        
        private static List<StrokePoint> RotateToZero(IList<StrokePoint> pointList)
        {
            var angle = IndicativeAngle(pointList);
            var newPoints = RotateBy(pointList, -angle);
            
            return newPoints;
        }

        private static List<StrokePoint> RotateBy(IList<StrokePoint> pointList, float angle)
        {
            var newPointList = new List<StrokePoint>();
            var center = GetCenterPoint(pointList);
            
            foreach (var point in pointList)
            {
                var x = (point.Point.x - center.x) * Mathf.Cos(angle) - (point.Point.y - center.y) * Mathf.Sin(angle) + center.x;
                var y = (point.Point.x - center.x) * Mathf.Sin(angle) + (point.Point.y - center.y) * Mathf.Cos(angle) + center.y;
                
                newPointList.Add(new StrokePoint(x, y, 0));
            }

            return newPointList;
        }

        private static float IndicativeAngle(IList<StrokePoint> pointList)
        {
            var center = BaseRecognizer.GetCenterPoint(pointList);
            return Mathf.Atan2(pointList[0].Point.y - center.y, pointList[0].Point.x - center.x);
        }

        #endregion Rotate

        #region Scale
        
        private static List<StrokePoint> ScaleToSquare(IList<StrokePoint> pointList, float size)
        {
            var newPointList = new List<StrokePoint>();
            var box = GetBoundingBox(pointList);
            
            foreach (var point in pointList)
            {
                var x = point.Point.x * size / box.width;
                var y = point.Point.y * size / box.height;
                
                newPointList.Add(new StrokePoint(x, y, 0));
            }

            return newPointList;
        }

        private static Rect GetBoundingBox(IList<StrokePoint> pointList)
        {
            var minX = pointList.Select(point => point.Point.x).Min();
            var maxX = pointList.Select(point => point.Point.x).Max();
            var minY = pointList.Select(point => point.Point.y).Min();
            var maxY = pointList.Select(point => point.Point.y).Max();
            
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        #endregion Scale

        #region Translate
        
        private static List<StrokePoint> TranslateToOrigin(IList<StrokePoint> pointList)
        {
            var newPointList = new List<StrokePoint>();
            var center = GetCenterPoint(pointList);
            
            foreach (var point in pointList)
            {
                var x = point.Point.x - center.x;
                var y = point.Point.y - center.y;
                
                newPointList.Add(new StrokePoint(x, y, 0));
            }

            return newPointList;
        }

        #endregion Translate

        #region Recognize

        public override Result Recognize(IList<StrokePoint> pointList, int total, GestureLibrary gestureLibrary)
        {
            var normalized = Normalize(pointList, total);
            var angle = 0.5f * (-1 + Mathf.Sqrt(5));
            
            return Recognize(normalized, 250, angle, gestureLibrary);
        }

        private Result Recognize(IList<StrokePoint> pointList, float size, float angle, GestureLibrary gestureLibrary)
        {
            const float theta = 45f;
            const float deltaTheta = 2f;
            
            var bestDistance = float.MaxValue;
            Gesture bestGesture = null;

            var checkedList = gestureLibrary.GestureList.Where(gesture => gesture.PointList.Count == pointList.Count);

            foreach (var gesture in checkedList)
            {
                var distance = DistanceAtBestAngle(pointList, gesture, -theta, theta, deltaTheta, angle);
                
                if (!(distance < bestDistance)) 
                { continue; }
                
                bestDistance = distance;
                bestGesture = gesture;
            }

            if (bestGesture == null)
            { return null; }

            var score = 1 - bestDistance / (0.5f * Math.Sqrt(2 * size * size));
            return new Result(bestGesture.Name, score);
        }
        
        #endregion Recognize

        #region Helper

        private float DistanceAtBestAngle(IList<StrokePoint> points, Gesture gesture, float thetaA, float thetaB, float deltaTheta, float angle)
        {
            var firstX = angle * thetaA + (1 - angle) * thetaB;
            var firstDistance = DistanceAtAngle(points, gesture, firstX);
            var secondX = (1 - angle) * thetaA + angle * thetaB;
            var secondDistance = DistanceAtAngle(points, gesture, secondX);

            while (thetaB - thetaA > deltaTheta)
            {
                if (firstDistance < secondDistance)
                {
                    thetaB = secondX;
                    secondX = firstX;
                    secondDistance = firstDistance;
                    firstX = angle * thetaA + (1 - angle) * thetaB;
                    firstDistance = DistanceAtAngle(points, gesture, firstX);
                }
                else
                {
                    thetaA = firstX;
                    firstX = secondX;
                    firstDistance = secondDistance;
                    secondX = (1 - angle) * thetaA + angle * thetaB;
                    secondDistance = DistanceAtAngle(points, gesture, secondX);
                }
            }

            return Mathf.Min(firstDistance, secondDistance);
        }

        private float DistanceAtAngle(IList<StrokePoint> pointList, Gesture gesture, float angle)
        {
            var newPointList = RotateBy(pointList, angle);
            return PathDistance(newPointList, gesture);
        }

        private float PathDistance(IList<StrokePoint> points, Gesture gesture)
        {
            var distance = 0f;

            for (var i = 0; i < points.Count; i++)
            {
                distance += Vector2.Distance(points[i].Point, gesture.PointList[i].Point);
            }

            return distance / points.Count;
        }

        #endregion Helper
    }
}