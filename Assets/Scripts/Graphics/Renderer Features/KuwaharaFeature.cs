using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Gobblefish.Graphics;

public class KuwaharaFeature : ScriptableRendererFeature {

    [System.Serializable]
    public class CustomSettings { // ScriptableRenderPassSettings

        public string passName = "Kuwahara Pass";

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        [HideInInspector] public Material material;

        public int boxSize;

        public void OnSetup(ref RenderingData renderingData) {
            material.SetFloat("_BoxSize", boxSize);
            
            // material.SetFloat("_BoxSize", boxSize);
            
            Vector2 screenDim = renderingData.cameraData.camera.GetOrthographicDimensions();
            material.SetFloat("_ScreenWidth", screenDim.x);
            material.SetFloat("_ScreenHeight", screenDim.y);
        }
        
    }

    class CustomRenderPass : ScriptableRenderPass {

        private CustomSettings m_Settings;
        private RenderTargetIdentifier m_CameraColorBuffer;

        private RenderTargetIdentifier m_OutputColorBuffer;
        private int m_OutputBufferID = 0;

        public CustomRenderPass(CustomSettings settings) {
            this.m_Settings = settings;
            this.renderPassEvent = settings.renderPassEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {

            m_CameraColorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

            // Create a tmp texture of the same dims.
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(m_OutputBufferID, descriptor, FilterMode.Point);
            m_OutputColorBuffer = new RenderTargetIdentifier(m_OutputBufferID);
            
            m_Settings.OnSetup(ref renderingData);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(m_Settings.passName))) {
                // Blit from the camera into tmp tex using material
                // Then blit that tmp tex back into cam so it renders to screen.
                Blit(cmd, m_CameraColorBuffer, m_OutputColorBuffer, m_Settings.material);
                Blit(cmd, m_OutputColorBuffer, m_CameraColorBuffer);
            }

            // Execute then release.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd) {
            if (cmd != null) {
                cmd.ReleaseTemporaryRT(m_OutputBufferID);
            }
        }

    }

    [SerializeField]
    private CustomSettings m_Settings;
    private CustomRenderPass m_ScriptablePass;

    [SerializeField]
    private Shader m_Shader;
    private Material m_Material;

    public override void Create() {
        m_Settings.material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_ScriptablePass = new CustomRenderPass(m_Settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing) {
        CoreUtils.Destroy(m_Material);
    }

}

