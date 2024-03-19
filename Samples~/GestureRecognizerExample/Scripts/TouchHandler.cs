using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TF.GestureRecognizer.Recognizer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace TF.GestureRecognizer.Sample
{
    [RequireComponent(typeof(GestureRecognizer))]
    public class TouchHandler : MonoBehaviour
    {
        [SerializeField] private bool enableDebug;
        [SerializeField] private DebugLine debugLine;
        [SerializeField] private string path;

        public UnityEvent OnDrawStart;
        public UnityEvent OnDrawFinish;
        public UnityEvent<string, double> OnCalculateResult;
        
        private Dictionary<int, Dictionary<int, DebugLine>> debugLineList = new();
        private Dictionary<int, List<StrokePoint>> pointList = new();
        private Dictionary<int, bool> touchUIList = new();
        private Dictionary<int, bool> movingList = new();
        private Dictionary<int, int> strokeIndexList = new();
        
        private GestureRecognizer recognizer;
        
        private void Start()
        {
            recognizer = GetComponent<GestureRecognizer>();
            recognizer.SetLibrary(LoadAllFromJson());
            
            EnhancedTouch.Touch.onFingerDown += OnFingerDown;
            EnhancedTouch.Touch.onFingerUp += OnFingerUp;
            EnhancedTouch.Touch.onFingerMove += OnFingerMove;
        }

        private void OnEnable()
        {
            EnhancedTouch.TouchSimulation.Enable();
            EnhancedTouch.EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouch.TouchSimulation.Disable();
            EnhancedTouch.EnhancedTouchSupport.Disable();
        }

        public void CalculateAll()
        {
            foreach (var key in pointList.Keys)
            {
                Calculate(key);
            }
        }

        public void Calculate(int index)
        {
            if (!pointList.TryGetValue(index, out var point))
            { return; }

            var result = recognizer.Recognize(point);

            if (result is null)
            {
                OnCalculateResult?.Invoke("Undefined", 0);
                
                if (enableDebug)
                {
                    debugLineList[index].Values.ToList().ForEach(line => line.CompleteLine(false));
                }
            }
            else
            {
                OnCalculateResult?.Invoke(result.Name, result.Score);

                if (enableDebug)
                {
                    debugLineList[index].Values.ToList().ForEach(line => line.CompleteLine(true));
                }
            }
            
            ResetCache(index);
        }

        public void SaveFirst(string saveName)
        {
            Save(saveName, pointList.First().Key);
        }

        public void Save(string saveName, int index)
        {
            SaveToJson(recognizer.Save(saveName, pointList[index]));
            ResetCache(index);
        }

        public void ResetCache(int index)
        {
            if (enableDebug)
            {
                debugLineList[index].Values.ToList().ForEach(line => line.ResetLine());
            }
            
            pointList[index].Clear();
            strokeIndexList[index] = -1;
        }

        private void OnFingerDown(EnhancedTouch.Finger finger)
        {
            if (IsTouchOverUI(finger.screenPosition))
            {
                touchUIList.TryAdd(finger.index, true);
                touchUIList[finger.index] = true;
                return;
            }
            
            pointList.TryAdd(finger.index, new List<StrokePoint>());

            strokeIndexList.TryAdd(finger.index, -1);
            strokeIndexList[finger.index]++;

            if (enableDebug)
            {
                debugLineList.TryAdd(finger.index, new Dictionary<int, DebugLine>());
                
                if (!debugLineList[finger.index].ContainsKey(strokeIndexList[finger.index]))
                {
                    debugLineList[finger.index].Add(strokeIndexList[finger.index], CreateNewDebugLine(finger.index, strokeIndexList[finger.index]));
                }
            }

            movingList.TryAdd(finger.index, false);
            movingList[finger.index] = false;
        }

        private void OnFingerMove(EnhancedTouch.Finger finger)
        {
            if (touchUIList.TryGetValue(finger.index, out var isTouchUI) && isTouchUI)
            { return; }
            
            pointList[finger.index].Add(new StrokePoint(finger.screenPosition, strokeIndexList[finger.index]));

            if (enableDebug)
            {
                debugLineList[finger.index][strokeIndexList[finger.index]].UpdateLine(pointList[finger.index]
                    .Where(point => point.StrokeIndex == strokeIndexList[finger.index])
                    .Select(point => point.Point.ToVector2())
                    .ToList());
            }

            if (movingList.TryGetValue(finger.index, out var isMoving) || !isMoving)
            {
                movingList[finger.index] = true;
                OnDrawStart?.Invoke();
            }
        }

        private void OnFingerUp(EnhancedTouch.Finger finger)
        {
            if (touchUIList.ContainsKey(finger.index))
            {
                touchUIList[finger.index] = false;
            }
            
            OnDrawFinish?.Invoke();
        }
        
        private bool IsTouchOverUI(Vector2 position)
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = position
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            
            return results.Count > 0;
        }

        private DebugLine CreateNewDebugLine(int index, int strokeIndex)
        {
            var line = Instantiate(debugLine, transform);
            line.name = $"draw_{index}_{strokeIndex}";

            return line;
        }
        
        private void SaveToJson(Gesture gesture)
        {
            if (gesture is null)
            { return; }
            
            if (string.IsNullOrEmpty(path))
            { return; }

            var savePath = Path.Combine(Application.dataPath, path);
            
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            
            var json = JsonConvert.SerializeObject(gesture);
            Debug.Log("test3 : \n" + json);

            var index = 1;
            var filePath = Path.Combine(savePath, $"{gesture.Name}_{index}.json");
            
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(savePath, $"{gesture.Name}_{++index}.json");
            }
            
            File.WriteAllText(filePath, json);
            
            Debug.Log("Saved new Gesture json on path : " + filePath);
        }
        
        private List<Gesture> LoadAllFromJson()
        {
            var gestureList = new List<Gesture>();
            
            if (string.IsNullOrEmpty(path))
            { return gestureList; }
            
            var savePath = Path.Combine(Application.dataPath, path);

            if (!Directory.Exists(savePath))
            { return gestureList; }

            var filePathList = Directory.GetFiles(savePath, "*.json");

            foreach (var filePath in filePathList)
            {
                var json = File.ReadAllText(filePath);
                var gesture = JsonConvert.DeserializeObject<Gesture>(json);
                
                gestureList.Add(gesture);
            }

            return gestureList;
        }
    }
}