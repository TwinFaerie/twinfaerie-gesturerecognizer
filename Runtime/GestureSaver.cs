using System.Collections.Generic;
using TF.GestureRecognizer.Recognizer;
using UnityEngine;

namespace TF.GestureRecognizer
{
    [RequireComponent(typeof(GestureRecognizer))]
    public class GestureSaver : MonoBehaviour
    {
        private GestureRecognizer recognizer;
        
        private void Start()
        {
            recognizer = GetComponent<GestureRecognizer>();
        }

        public void Save(string gestureName, List<StrokePoint> pointList)
        {
            recognizer.GestureLibrary.Add(gestureName, recognizer.Recognizer.Normalize(pointList, 64));
        }
    }
}