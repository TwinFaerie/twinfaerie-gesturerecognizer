using System.Collections.Generic;
using System.Linq;
using TF.GestureRecognizer.Recognizer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace TF.GestureRecognizer.Sample
{
    [RequireComponent(typeof(GestureRecognizer), typeof(GestureSaver))]
    public class TouchHandler : MonoBehaviour
    {
        [SerializeField] private bool enableDebug;
        [SerializeField] private DebugLine debugLine;

        public UnityEvent OnDrawStart;
        public UnityEvent OnDrawFinish;
        public UnityEvent<string, double> OnCalculateResult;
        
        private Dictionary<int, Dictionary<int, DebugLine>> debugLineList = new();
        private Dictionary<int, List<StrokePoint>> pointList = new();
        private Dictionary<int, bool> touchUIList = new();
        private Dictionary<int, bool> movingList = new();
        private Dictionary<int, int> strokeIndexList = new();
        
        private GestureRecognizer recognizer;
        private GestureSaver saver;
        
        private void Start()
        {
            recognizer = GetComponent<GestureRecognizer>();
            saver = GetComponent<GestureSaver>();
            
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
            saver.Save(saveName, pointList[index]);
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
                    .Select(point => point.Point)
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
    }
}