using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Gobblefish.Graphics;

public class PosterizationFeature : ScriptableRendererFeature {

    [System.Serializable]
    public class PosterizationSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int valuePosterizationLevel = 16;
        [HideInInspector] public Material posterizationMaterial;
    }

    class PosterizationRenderPass : ScriptableRenderPass {

        private PosterizationSettings m_Settings;
        private RenderTargetIdentifier m_CameraColorBuffer;

        private RenderTargetIdentifier m_PosterizationColorBuffer;
        private int m_PosterizationColorBufferID = 0;

        public PosterizationRenderPass(PosterizationSettings settings) {
            this.m_Settings = settings;
            this.renderPassEvent = settings.renderPassEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {

            m_CameraColorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

            // Create a tmp texture of the same dims.
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(m_PosterizationColorBufferID, descriptor, FilterMode.Point);
            m_PosterizationColorBuffer = new RenderTargetIdentifier(m_PosterizationColorBufferID);
            
            m_Settings.posterizationMaterial.SetFloat("_ValuePosterity", m_Settings.valuePosterizationLevel);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Posterization Pass"))) {
                // Blit from the camera into tmp tex using material
                // Then blit that tmp tex back into cam so it renders to screen.
                Blit(cmd, m_CameraColorBuffer, m_PosterizationColorBuffer, m_Settings.posterizationMaterial);
                Blit(cmd, m_PosterizationColorBuffer, m_CameraColorBuffer);
            }

            // Execute then release.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd) {
            if (cmd != null) {
                cmd.ReleaseTemporaryRT(m_PosterizationColorBufferID);
            }
        }

    }

    [SerializeField]
    private PosterizationSettings m_Settings;
    private PosterizationRenderPass m_ScriptablePass;

    [SerializeField]
    private Shader m_posterizationShader;
    private Material m_posterizationMaterial;

    public override void Create() {
        m_Settings.posterizationMaterial = CoreUtils.CreateEngineMaterial(m_posterizationShader);
        m_ScriptablePass = new PosterizationRenderPass(m_Settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing) {
        CoreUtils.Destroy(m_posterizationMaterial);
    }

}


