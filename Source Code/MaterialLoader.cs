using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GitIMGLoader
{
    public class MaterialLoader : MonoBehaviour
    {
        public MaterialToLoad[] materialsToLoad;
        public bool forceLoad = false;
        public float loadCheck = 2f;
        private bool isLoading = false;

        void Start()
        {
            StartCoroutine(LoadCheck());
        }

        IEnumerator LoadCheck()
        {
            while (true)
            {
                if (!isLoading || forceLoad)
                {
                    isLoading = true;
                    forceLoad = false;
                    StartCoroutine(LoadImages());
                }
                yield return new WaitForSeconds(loadCheck);
            }
        }

        IEnumerator LoadImages()
        {
            foreach (var materialData in materialsToLoad)
            {
                if (string.IsNullOrEmpty(materialData.textureURL) || materialData.targetMaterial == null)
                {
                    Debug.LogWarning("Invalid material data.");
                    continue;
                }

                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(materialData.textureURL))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                        materialData.targetMaterial.mainTexture = texture;

                        materialData.targetMaterial.color = materialData.materialColor;

                        Debug.Log($"Image loaded successfully for material {materialData.targetMaterial.name}");
                    }
                    else
                    {
                        Debug.LogError($"Image load failed for {materialData.textureURL}: {request.error}");
                    }
                }
            }

            isLoading = false;
        }
    }

    [System.Serializable]
    public class MaterialToLoad
    {
        public string textureURL;
        public Material targetMaterial;
        public Color materialColor;
    }
}
