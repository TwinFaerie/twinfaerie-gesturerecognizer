using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TF.GestureRecognizer.Recognizer
{
    public class GestureLibrary
    {
        public List<Gesture> GestureList { get; private set; }
        public readonly string SavePath;

        public GestureLibrary(string path = null)
        {
            if (path is not null)
            {
                SavePath = Path.Combine(Application.dataPath, path);
            }
            
            LoadFromJson();
        }
        
        public GestureLibrary(List<Gesture> gestureList)
        {
            SavePath = null;
            GestureList = gestureList;
        }

        public void Add(string name, List<StrokePoint> pointList)
        {
            GestureList.Add(new Gesture(name, pointList));
        }
        
        public void SaveToJson()
        {
            if (string.IsNullOrEmpty(SavePath))
            { return; }
            
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            if (GestureList is null || GestureList.Any())
            { return; }

            foreach (var gesture in GestureList)
            {
                var json = JsonUtility.ToJson(gesture);

                var index = 1;
                var filePath = Path.Combine(SavePath, $"{gesture.Name}_{index}.json");
                
                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(SavePath, $"{gesture.Name}_{++index}.json");
                }
                
                File.WriteAllText(filePath, json);
            }
        }

        public void LoadFromJson()
        {
            GestureList = new List<Gesture>();
            
            if (string.IsNullOrEmpty(SavePath) || !Directory.Exists(SavePath))
            { return; }

            var filePathList = Directory.GetFiles(SavePath, "*.json");

            foreach (var filePath in filePathList)
            {
                var json = File.ReadAllText(filePath);
                var gesture = JsonUtility.FromJson<Gesture>(json);
                
                GestureList.Add(gesture);
            }
        }
    }
}