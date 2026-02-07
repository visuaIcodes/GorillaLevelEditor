using System.Reflection;
using UnityEngine;

namespace GorillaLevelEditor.Core
{
    internal class AssetsLoader
    {
        public static byte[] LoadEmbeddedResource(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            using (Stream stream = asm.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return null;

                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        public static AssetBundle LoadAssetBundleFromEmbeddedResource(string resource)
        {
            byte[] bytes = LoadEmbeddedResource(resource);
            if (bytes == null)
            {
                return null;
            }

            return AssetBundle.LoadFromMemory(bytes);
        }
    }
}