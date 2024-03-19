using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace TF.GestureRecognizer.Recognizer
{
    public class GestureLibrary
    {
        public readonly List<Gesture> GestureList;
        
        public GestureLibrary(List<Gesture> gestureList)
        {
            GestureList = gestureList;
        }

        public Gesture Add(string name, List<StrokePoint> pointList)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Can't save with empty name");
                return null;
            }
            
            var gesture = new Gesture(name, pointList);
            GestureList.Add(gesture);

            Debug.Log("test2 : " + gesture.Name);
            return gesture;
        }

        public void Remove(Gesture gesture)
        {
            if (!GestureList.Contains(gesture))
            { return; }
            
            GestureList.Remove(gesture);
        }
    }
}