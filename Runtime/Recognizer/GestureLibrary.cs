using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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
            
            LoadAllFromJson();
        }
        
        public GestureLibrary(List<Gesture> gestureList)
        {
            SavePath = null;
            GestureList = gestureList;
        }

        public void Add(string name, List<StrokePoint> pointList)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Can't save with empty name");
                return;
            }
            
            var gesture = new Gesture(name, pointList);
            
            SaveToJson(gesture);
            GestureList.Add(gesture);
        }
        
        public void SaveToJson(Gesture gesture)
        {
            if (string.IsNullOrEmpty(SavePath))
            { return; }
            
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            
            var json = JsonConvert.SerializeObject(gesture);

            var index = 1;
            var filePath = Path.Combine(SavePath, $"{gesture.Name}_{index}.json");
            
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(SavePath, $"{gesture.Name}_{++index}.json");
            }
            
            File.WriteAllText(filePath, json);
        }

        public void LoadAllFromJson()
        {
            GestureList = new List<Gesture>();
            
            if (string.IsNullOrEmpty(SavePath) || !Directory.Exists(SavePath))
            { return; }

            var filePathList = Directory.GetFiles(SavePath, "*.json");

            foreach (var filePath in filePathList)
            {
                var json = File.ReadAllText(filePath);
                var gesture = JsonConvert.DeserializeObject<Gesture>(json);
                
                GestureList.Add(gesture);
            }
        }
    }
}