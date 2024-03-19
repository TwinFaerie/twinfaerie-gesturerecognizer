using System.Collections.Generic;
using TF.GestureRecognizer.Recognizer;
using UnityEngine;

namespace TF.GestureRecognizer
{
    public class GestureRecognizer : MonoBehaviour
    {
        public enum RecognizerType
        {
            OneDollar,
        }

        [SerializeField] private RecognizerType type;
        
        public BaseRecognizer Recognizer { get; private set; }
        public GestureLibrary GestureLibrary { get; private set; }
        
        private void Start()
        {
            SetupRecognizer();
        }

        private void SetupRecognizer()
        {
            Recognizer = type switch
            {
                RecognizerType.OneDollar => new DollarOneRecognizer(),
                _ => null
            };
        }

        public void SetLibrary(List<Gesture> gestureList)
        {
            GestureLibrary = new GestureLibrary(gestureList);
        }

        public Result Recognize(List<StrokePoint> pointList)
        {
            if (GestureLibrary is null)
            { return null; }
            
            return Recognizer.Recognize(pointList, 64, GestureLibrary);
        }

        public Result Recognize(List<StrokePoint> pointList, string gestureName)
        {
            if (GestureLibrary is null)
            { return null; }
            
            return Recognizer.Recognize(pointList, 64, GestureLibrary, gestureName);
        }
        
        public Gesture Save(string gestureName, List<StrokePoint> pointList)
        {
            Debug.Log("test1");
            return GestureLibrary?.Add(gestureName, Recognizer.Normalize(pointList, 64));
        }
        
        public void Remove(Gesture gesture)
        {
            GestureLibrary?.Remove(gesture);
        }
    }
}