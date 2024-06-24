using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Gobblefish.Graphics;

public class RemapValueFeature : ScriptableRendererFeature {

    [System.Serializable]
    public class CustomSettings { // ScriptableRenderPassSettings

        public string passName = "Remap Values Pass";

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        [HideInInspector] public Material material;

        public float flatValue = -1f;
        public float preLinTrans = 0f;
        public float preLinScale = 1f;
        public float expScale = 1f;
        public float postLinTrans = 0f;
        public float postLinScale = 1f;

        public bool keepOnlyValues = false;
        public bool clampValues = false;
        public float posterizeLevel = 0f;


        public void OnSetup(ref RenderingData renderingData) {
            material.SetFloat("_FlatValue", flatValue);
            material.SetFloat("_PreLinearTranslate", preLinTrans);
            material.SetFloat("_PreLinearScale", preLinScale);
            material.SetFloat("_ExponentialScale", expScale);
            material.SetFloat("_PostLinearTranslate", postLinTrans);
            material.SetFloat("_PostLinearScale", postLinScale);
            material.SetFloat("_KeepOnlyValues", keepOnlyValues ? 1f : -1f);
            material.SetFloat("_ClampValues", clampValues ? 1f : -1f);
            material.SetFloat("_PosterizeLevel", posterizeLevel);
            Vector2 screenDim = renderingData.cameraData.camera.GetOrthographicDimensions();
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


