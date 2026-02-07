using UnityEngine;

namespace GorillaLevelEditor.Core.Modules
{
    internal class RenderingModule
    {
        public GUISkin UISkin;

        private AssetBundle UISkin_assetbundle;

        public void InitializeUISkin()
        {
            UISkin_assetbundle = AssetsLoader.LoadAssetBundleFromEmbeddedResource("GorillaLevelEditor.Resources.gorillaleveleditor");
            UISkin = UISkin_assetbundle.LoadAsset<GUISkin>("Skin");
        }

        public void RenderUI()
        {
            GUI.skin = UISkin;
            GUI.Label(new Rect(0, 10, 300, 100), "yo wasgud");
        }
    }
}