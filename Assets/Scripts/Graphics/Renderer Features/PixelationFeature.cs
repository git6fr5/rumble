using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Gobblefish.Graphics;

public class PixelationFeature : ScriptableRendererFeature {

    [System.Serializable]
    public class PixelationSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int pixelsPerUnit = 16;
        [HideInInspector] public Material pixelationMaterial;
    }

    class PixelationRenderPass : ScriptableRenderPass {

        private PixelationSettings m_Settings;
        private RenderTargetIdentifier m_CameraColorBuffer;

        private RenderTargetIdentifier m_PixelColorBuffer;
        private int m_PixelColorBufferID = 0;

        public PixelationRenderPass(PixelationSettings settings) {
            this.m_Settings = settings;
            this.renderPassEvent = settings.renderPassEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {

            m_CameraColorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

            // Create a tmp texture of the same dims.
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(m_PixelColorBufferID, descriptor, FilterMode.Point);
            m_PixelColorBuffer = new RenderTargetIdentifier(m_PixelColorBufferID);
            
            m_Settings.pixelationMaterial.SetFloat("_PixelsPerUnit", m_Settings.pixelsPerUnit);
            Vector2 screenDim = renderingData.cameraData.camera.GetOrthographicDimensions();
            
            // Debug.Log(screenDim);
            m_Settings.pixelationMaterial.SetFloat("_ScreenWidth", screenDim.x);
            m_Settings.pixelationMaterial.SetFloat("_ScreenHeight", screenDim.y);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Pixelization Pass"))) {
                // Blit from the camera into tmp tex using material
                // Then blit that tmp tex back into cam so it renders to screen.
                Blit(cmd, m_CameraColorBuffer, m_PixelColorBuffer, m_Settings.pixelationMaterial);
                Blit(cmd, m_PixelColorBuffer, m_CameraColorBuffer);
            }

            // Execute then release.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd) {
            if (cmd != null) {
                cmd.ReleaseTemporaryRT(m_PixelColorBufferID);
            }
        }

    }

    [SerializeField]
    private PixelationSettings m_Settings;
    private PixelationRenderPass m_ScriptablePass;

    [SerializeField]
    private Shader m_pixelationShader;
    private Material m_pixelationMaterial;

    public override void Create() {
        m_Settings.pixelationMaterial = CoreUtils.CreateEngineMaterial(m_pixelationShader);
        m_ScriptablePass = new PixelationRenderPass(m_Settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing) {
        CoreUtils.Destroy(m_pixelationMaterial);
    }

}


