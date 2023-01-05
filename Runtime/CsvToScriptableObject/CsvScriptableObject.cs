using System;
using System.Collections;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

namespace Gamepangin
{
    public abstract class CsvScriptableObject<T> : ScriptableObject
    {
        public bool useWeb;
        [HideIf("useWeb"), FilePath(Extensions = "csv", ParentFolder = "Assets")]
        public string csvFile;
        [ShowIf("useWeb")]
        public string csvUrl;
        [ReadOnly]
        public T[] data;
    
        [Button(ButtonSizes.Large)]
        public void ReloadData()
        {
            if (useWeb)
            {
                ReloadFromWeb();
            }
            else
            {
                ReloadFromFile();
            }
        }
    
        [Button(ButtonSizes.Large)]
        public void ClearData()
        {
            data = null;
        }
    
        private void ReloadFromFile()
        {
            var allLines = File.ReadAllText(Application.dataPath + "/" + csvFile);
            data = CsvSerializer.Deserialize<T>(allLines);
            Debug.Log($"Reload done...");
        }
    
        private void ReloadFromWeb()
        {
    #if UNITY_EDITOR
            EditorCoroutines.Execute(DownloadAndImport());
    #endif
        }
    
        private IEnumerator DownloadAndImport()
        {
            var www = UnityWebRequest.Get(csvUrl);
            yield return www.SendWebRequest();
    
            while (www.isDone == false)
            {
                yield return new WaitForEndOfFrame();
            }
    
            if (www.error != null)
            {
                Debug.Log("UnityWebRequest.error:" + www.error);
            }
            else if (www.downloadHandler.text == "" || www.downloadHandler.text.IndexOf("<!DOCTYPE", StringComparison.Ordinal) != -1)
            {
                Debug.Log("Unknown Format:" + www.downloadHandler.text);
            }
            else
            {
                var rows = CsvSerializer.ParseCsv(www.downloadHandler.text);
                data = CsvSerializer.Deserialize<T>(rows);
                Debug.Log($"Reload done...");
            }
        }
    }
}
