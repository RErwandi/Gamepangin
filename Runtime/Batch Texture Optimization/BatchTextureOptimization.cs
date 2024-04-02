using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Gamepangin
{
    [GlobalConfig("_Gamepangin/Resources")]
    public class BatchTextureOptimization : GlobalConfig<BatchTextureOptimization>
    {
        private static int[] textureSizes = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
        
        #if UNITY_EDITOR

        [InfoBox("Texture optimization only works for Default, Normal, and Sprite texture type")]
        [FolderPath, Required, BoxGroup("Required")]
        public string rootFolder;
        
        [BoxGroup("Mip Map")]
        public bool generateMipmaps = true;
        [BoxGroup("Mip Map")]
        public bool mipmapStreaming = true;
        
        [BoxGroup("Size")]
        public bool overrideMaximumSize;
        [BoxGroup("Size"), ValueDropdown("textureSizes"), ShowIf("overrideMaximumSize")]
        public int maximumSize = 2048;
        
        [BoxGroup("Compression")]
        public bool useCrunchCompression;
        [BoxGroup("Compression"), Range(0, 100), ShowIf("useCrunchCompression")]
        public int compressionQuality = 50;

        [BoxGroup("Mobile")]
        public bool overrideAndroid;
        [BoxGroup("Mobile")]
        public bool overrideIOS;
        [BoxGroup("Mobile")]
        public TextureImporterFormat mobileCompressionFormat = TextureImporterFormat.ETC2_RGBA8Crunched;

        [PropertySpace(spaceBefore:30)]
        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
        public void OptimizeTextures()
        {
            string targetDirectory = System.IO.Path.GetDirectoryName(rootFolder);
            int totalOptimized = 0;

            // Get all texture assets in the project.
            string[] guids = AssetDatabase.FindAssets("t:texture", new[] { targetDirectory });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                // Check if the texture importer is not null and is set to a non-compressed format.
                if (textureImporter != null)
                {
                    // Compressing splatmap cause pixelated texture in terrains, so we skip them
                    if(path.Contains("Splatmap"))
                    {
                        continue;
                    }
                    
                    // Only change Default, Normal and Sprite texture
                    if(textureImporter.textureType == TextureImporterType.Default || textureImporter.textureType == TextureImporterType.Sprite || textureImporter.textureType == TextureImporterType.NormalMap)
                    {
                        var prevTextureSize = textureImporter.maxTextureSize;
                        
                        // Mip Map Settings
                        textureImporter.mipmapEnabled = generateMipmaps;
                        textureImporter.streamingMipmaps = mipmapStreaming;
                        
                        // Size Settings
                        if (overrideMaximumSize)
                        {
                            textureImporter.maxTextureSize = maximumSize;
                            prevTextureSize = maximumSize;
                        }
                        
                        // Crunch Compression Settings
                        textureImporter.crunchedCompression = useCrunchCompression;
                        textureImporter.textureCompression = TextureImporterCompression.Compressed;
                        textureImporter.compressionQuality = compressionQuality;

                        if (overrideAndroid)
                        {
                            var androidSettings = new TextureImporterPlatformSettings
                            {
                                overridden = true,
                                name = "Android",
                                maxTextureSize = prevTextureSize,
                                format = mobileCompressionFormat,
                                compressionQuality = compressionQuality
                            };
                        
                            textureImporter.SetPlatformTextureSettings(androidSettings);
                        }

                        if (overrideIOS)
                        {
                            var iosSettings = new TextureImporterPlatformSettings
                            {
                                overridden = true,
                                name = "iOS",
                                maxTextureSize = prevTextureSize,
                                format = mobileCompressionFormat,
                                compressionQuality = compressionQuality
                            };
                        
                            textureImporter.SetPlatformTextureSettings(iosSettings);
                        }
                        
                        // Automatically set texture size to power of two if its not for default type texture
                        if (textureImporter.textureType == TextureImporterType.Default)
                        {
                            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                        }

                        totalOptimized++;
                        
                        // Apply the changes.
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }
            }

            Debug.Log($"Texture Optimization in directory: {targetDirectory} success. {totalOptimized} textures has been optimized");
        }
#endif
    }
}
