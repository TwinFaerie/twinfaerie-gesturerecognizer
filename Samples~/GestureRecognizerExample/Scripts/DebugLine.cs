using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TF.GestureRecognizer.Sample
{
    [RequireComponent(typeof(LineRenderer))]
    public class DebugLine : MonoBehaviour
    {
        [Header("Color")]
        [SerializeField] private Color drawColor = Color.yellow;
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color failColor = Color.red;
        
        private LineRenderer line;
        private new Camera camera;

        private Coroutine delayHideRoutine;
        
        private void Start()
        {
            line = GetComponent<LineRenderer>();
            camera = Camera.main;

            ResetLine();
        }

        private void SetColor(Color color)
        {
            line.startColor = color;
            line.endColor = color;
        }

        public void UpdateLine(List<Vector2> pointList)
        {
            if (line is null)
            { return; }
            
            line.positionCount = pointList.Count;
            line.SetPositions(pointList.Select(point2D => camera.ScreenToWorldPoint(new Vector3(point2D.x, point2D.y, 10f))).ToArray());
        }

        public void CompleteLine(bool success)
        {
            SetColor(success ? successColor : failColor);
        }

        public void ResetLine()
        {
            line.positionCount = 0;
            line.SetPositions(Array.Empty<Vector3>());
            
            SetColor(drawColor);
        }
    }
}