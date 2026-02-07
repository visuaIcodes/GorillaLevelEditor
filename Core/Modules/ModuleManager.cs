namespace GorillaLevelEditor.Core.Modules
{
    internal class ModuleManager : IDisposable
    {
        private RenderingModule RenderingModule;

        public void InitializeBaseModules()
        {
            RenderingModule = new RenderingModule();
            RenderingModule.InitializeUISkin();
        }

        public void OnUpdate()
        {
            
        }

        public void OnRenderUI()
        {
            RenderingModule.RenderUI();
        }

        public void Dispose()
        {
        }
    }
}