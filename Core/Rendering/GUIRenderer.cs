using ImGuiNET;
using UnityEngine;
using UnityEngine.Rendering;

namespace GorillaLevelEditor.Core.Rendering
{
    internal class GUIRenderer : MonoBehaviour
    {
        public static GUIRenderer Instance { get; private set; }

        public Material GUIMaterial;
        private Texture2D FontTexture;

        private CommandBuffer CommandBuffer;
        private Mesh UIMesh;

        private bool Initialized;

        public void Init()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            ImGui.CreateContext();

            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.AddFontDefault();

            UploadFontTexture();
            UIInput.Initialize();

            UIMesh = new Mesh();
            UIMesh.MarkDynamic();

            CommandBuffer = new CommandBuffer { name = "ImGuiRenderer" };
            Initialized = true;
        }

        private void UploadFontTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            unsafe
            {
                byte* pixels;
                int width, height, bpp;
                io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bpp);

                FontTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                byte[] data = new byte[width * height * 4];
                System.Runtime.InteropServices.Marshal.Copy((System.IntPtr)pixels, data, 0, data.Length);
                FontTexture.LoadRawTextureData(data);
                FontTexture.Apply();

                io.Fonts.SetTexID((IntPtr)1);
                io.Fonts.ClearTexData();
            }

            if (GUIMaterial == null)
            {
                GUIMaterial = new Material(Shader.Find("UI/Default"));
                GUIMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                GUIMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                GUIMaterial.SetInt("_Cull", (int)CullMode.Off);
                GUIMaterial.SetInt("_ZWrite", 0);
                GUIMaterial.SetTexture("_MainTex", FontTexture);
            }
        }

        public void RenderDrawDataSRP(ScriptableRenderContext context, ImDrawDataPtr drawData)
        {
            if (!Initialized)
                return;

            int fbWidth = (int)(drawData.DisplaySize.x * drawData.FramebufferScale.x);
            int fbHeight = (int)(drawData.DisplaySize.y * drawData.FramebufferScale.y);
            if (fbWidth == 0 || fbHeight == 0)
                return;

            CommandBuffer.Clear();
            CommandBuffer.SetViewport(new Rect(0, 0, fbWidth, fbHeight));

            var projection = Matrix4x4.Ortho(0, fbWidth, fbHeight, 0, -1f, 1f);
            CommandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, projection);

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdListsRange[n];

                int vertexCount = cmdList.VtxBuffer.Size;

                Vector3[] vertices = new Vector3[vertexCount];
                Vector2[] uvs = new Vector2[vertexCount];
                Color32[] colors = new Color32[vertexCount];

                for (int i = 0; i < vertexCount; i++)
                {
                    var v = cmdList.VtxBuffer[i];
                    vertices[i] = new Vector3(v.pos.x, v.pos.y, 0);
                    uvs[i] = new Vector2(v.uv.x, v.uv.y);

                    uint c = v.col;
                    colors[i] = new Color32(
                        (byte)(c & 0xFF),
                        (byte)((c >> 8) & 0xFF),
                        (byte)((c >> 16) & 0xFF),
                        (byte)((c >> 24) & 0xFF)
                    );
                }

                int idxOffset = 0;

                for (int cmd_i = 0; cmd_i < cmdList.CmdBuffer.Size; cmd_i++)
                {
                    var drawCmd = cmdList.CmdBuffer[cmd_i];

                    if (drawCmd.UserCallback != IntPtr.Zero)
                    {
                        idxOffset += (int)drawCmd.ElemCount;
                        continue;
                    }

                    int indexCount = (int)drawCmd.ElemCount;
                    int[] indices = new int[indexCount];
                    for (int i = 0; i < indexCount; i++)
                    {
                        indices[i] = (int)cmdList.IdxBuffer[idxOffset + i];
                    }

                    Mesh drawMesh = new Mesh();
                    drawMesh.vertices = vertices;
                    drawMesh.uv = uvs;
                    drawMesh.colors32 = colors;
                    drawMesh.SetIndices(indices, MeshTopology.Triangles, 0);

                    var props = new MaterialPropertyBlock();
                    Texture texture = (drawCmd.TextureId == (IntPtr)1) ? FontTexture : Texture2D.whiteTexture;
                    props.SetTexture("_MainTex", texture);

                    var clip = drawCmd.ClipRect;
                    Rect scissor = new Rect(
                        Mathf.Max(0, clip.x),
                        Mathf.Max(0, fbHeight - clip.w),
                        Mathf.Max(0, clip.z - clip.x),
                        Mathf.Max(0, clip.w - clip.y)
                    );

                    CommandBuffer.EnableScissorRect(scissor);
                    CommandBuffer.DrawMesh(drawMesh, Matrix4x4.identity, GUIMaterial, 0, -1, props);
                    CommandBuffer.DisableScissorRect();

                    idxOffset += indexCount;

                    Destroy(drawMesh);
                }
            }

            context.ExecuteCommandBuffer(CommandBuffer);
            context.Submit();
        }
    }
}