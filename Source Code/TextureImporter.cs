using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GitIMGLoader
{
    public class TextureImporter : EditorWindow
    {
        private string textureURL = "";
        private string folderName = "Textures";
        public static string lastImportedTexture = "";

        [MenuItem("Tools/GitIMGLoader/Import Texture")]
        public static void ShowWindow()
        {
            GetWindow<TextureImporter>("Texture Importer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Texture Importer", EditorStyles.boldLabel);

            textureURL = EditorGUILayout.TextField("Texture URL", textureURL);
            folderName = EditorGUILayout.TextField("Folder Name", folderName);

            if (GUILayout.Button("Import Texture"))
            {
                if (!string.IsNullOrEmpty(textureURL))
                {
                    string savePath = $"Assets/{folderName}";
                    ImportTexture(textureURL, savePath);
                }
                else
                {
                    Debug.LogError("Texture URL is empty!");
                }
            }
        }

        private async void ImportTexture(string url, string path)
        {
            byte[] imageBytes = await DownloadImage(url);
            if (imageBytes == null) return;

            string fileName = Path.GetFileName(url);
            string fullPath = Path.Combine(path, fileName);
            string assetPath = fullPath.Replace(Application.dataPath, "Assets");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            File.WriteAllBytes(fullPath, imageBytes);
            AssetDatabase.ImportAsset(assetPath);
            lastImportedTexture = assetPath;
            Debug.Log($"Texture imported to: {assetPath}");
        }

        private async Task<byte[]> DownloadImage(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return await client.DownloadDataTaskAsync(url);
                }
                catch (WebException e)
                {
                    Debug.LogError("Failed to download image: " + e.Message);
                    return null;
                }
            }
        }
    }
}
