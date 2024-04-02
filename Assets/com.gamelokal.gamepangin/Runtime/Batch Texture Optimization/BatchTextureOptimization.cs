using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gamepangin
{
    [GlobalConfig("_Gamepangin/Resources")]
    public class BatchTextureOptimization : GlobalConfig<BatchTextureOptimization>
    {
        private static int[] textureSizes = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
        
        #if UNITY_EDITOR

        [InfoBox("Texture optimization only works for Default, Normal, and Sprite texture type")]
        [FolderPath, Required, BoxGroup("Configuration")]
        public string rootFolder;
        
        [InfoBox("Sprite texture always have mip map disabled")]
        [BoxGroup("Mip Map")]
        public bool generateMipmaps = true;
        [BoxGroup("Mip Map")]
        public bool mipmapStreaming = true;
        
        [BoxGroup("Size")]
        public bool overrideMaximumSize;
        [BoxGroup("Size"), ValueDropdown("textureSizes"), ShowIf("overrideMaximumSize")]
        public int maximumSize = 2048;
        
        [InfoBox("Image will be resized to nearest power of 4 to enable crunch compression")]
        [BoxGroup("Compression")]
        public bool useCrunchCompression;
        [BoxGroup("Compression"), Range(0, 100), ShowIf("useCrunchCompression")]
        public int compressionQuality = 50;
        private bool autoResizeImage = true;

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
            if (autoResizeImage)
            {
                ResizeImageInFolder(rootFolder);
            }
            
            int totalOptimized = 0;

            // Get all texture assets in the project.
            string[] guids = AssetDatabase.FindAssets("t:texture", new[] { rootFolder });
            
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

                        textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                        
                        
                        // Mip Map Settings
                        if (textureImporter.textureType == TextureImporterType.Sprite)
                        {
                            textureImporter.mipmapEnabled = false;
                            textureImporter.streamingMipmaps = false;
                        }
                        else
                        {
                            textureImporter.mipmapEnabled = generateMipmaps;
                            textureImporter.streamingMipmaps = mipmapStreaming;
                        }
                        
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
                        else
                        {
                            var androidSettings = new TextureImporterPlatformSettings
                            {
                                overridden = false,
                                name = "Android"
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
                        else
                        {
                            var iosSettings = new TextureImporterPlatformSettings
                            {
                                overridden = false,
                                name = "iOS"
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

            Debug.Log($"Texture Optimization in directory: {rootFolder} success. {totalOptimized} textures has been optimized");
        }

        private void ResizeImageInFolder(string path)
        {
            int resizedImages = 0;
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file).ToLower();
                if (extension == ".jpg" || extension == ".png")
                {
                    if (TryResizeImage(file))
                    {
                        resizedImages++;
                    }
                }
            }
            
            if(resizedImages > 0)
                Debug.Log($"Successfully detect and resized {resizedImages} images to power of 4 to enable crunch compression");
        }
        
        private bool TryResizeImage(string filePath)
        {
            try
            {
                var power = 4;
                var fileFormat = Path.GetExtension(filePath).ToLower();
                var fileData = File.ReadAllBytes(filePath);
                
                var tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);

                // Do nothing if image size is match
                if (tex.height % power == 0 && tex.width % power == 0)
                {
                    return false;
                }
                
                var height = (int)Math.Round(tex.height / (float)power) * power;
                var width = (int)Math.Round(tex.width / (float)power) * power;
                
                var resizeRT = RenderTexture.GetTemporary(width, height, 0);
                Graphics.Blit(tex, resizeRT);
                
                var nArray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                var request = AsyncGPUReadback.RequestIntoNativeArray (ref nArray, resizeRT, 0, (AsyncGPUReadbackRequest request) =>
                {
                    if (!request.hasError)
                    {
                        NativeArray<byte> encoded;
 
                        switch (fileFormat)
                        {
                            case ".jpg":
                                encoded = ImageConversion.EncodeNativeArrayToJPG(nArray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0, 95);
                                break;
                            default:
                                encoded = ImageConversion.EncodeNativeArrayToPNG(nArray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                                break;
                        }
 
                        File.WriteAllBytes(filePath, encoded.ToArray());
                        encoded.Dispose();
                    }
 
                    nArray.Dispose();
                });
                
                request.WaitForCompletion();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
#endif
    }
}
