using System;
using UnityEditor;

namespace Gamepangin.Editor
{
    [CustomEditor(typeof(TAssetRepository), true)]
    public class TAssetRepositoryEditor : UnityEditor.Editor
    {
        [InitializeOnLoadMethod]
        private static void OnEditorLoad()
        {
            Type[] types = TypeUtils.GetTypesDerivedFrom(typeof(TAssetRepository));
            foreach (Type type in types)
            {
                string[] foundGuids = AssetDatabase.FindAssets($"t:{type}");
                if (foundGuids.Length > 0) continue;

                TAssetRepository asset = CreateInstance(type) as TAssetRepository;
                if (asset == null) continue;
                
                DirectoryUtils.RequirePath(asset.AssetPath);
                string assetPath = PathUtils.Combine(
                    asset.AssetPath, 
                    $"{asset.RepositoryId}.asset"
                );
                
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
            }
        }
    }
}