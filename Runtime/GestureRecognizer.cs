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
        [SerializeField] private string path;
        
        public BaseRecognizer Recognizer { get; private set; }
        public GestureLibrary GestureLibrary { get; private set; }
        
        private void Start()
        {
            SetupRecognizer();
            SetupLibrary();
        }

        private void SetupRecognizer()
        {
            Recognizer = type switch
            {
                RecognizerType.OneDollar => new DollarOneRecognizer(),
                _ => null
            };
        }

        private void SetupLibrary()
        {
            GestureLibrary = new GestureLibrary(path);
        }

        public Result Recognize(List<StrokePoint> pointList)
        {
            return Recognizer.Recognize(pointList, 64, GestureLibrary);
        }

        public Result Recognize(List<StrokePoint> pointList, string gestureName)
        {
            return Recognizer.Recognize(pointList, 64, GestureLibrary, gestureName);
        }
        
        public void Save(string gestureName, List<StrokePoint> pointList)
        {
            GestureLibrary.Add(gestureName, Recognizer.Normalize(pointList, 64));
        }
    }
}