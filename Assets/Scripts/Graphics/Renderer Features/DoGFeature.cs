using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Gobblefish.Graphics;

public class DoGFeature : ScriptableRendererFeature {

    [System.Serializable]
    public class CustomSettings { // ScriptableRenderPassSettings

        public string passName = "DoG Pass";

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        [HideInInspector] public Material material;

        [Range(1, 10)]
        public int gaussianKernelSize = 5;

        [Range(0.1f, 5.0f)]
        public float stdev = 2.0f;

        [Range(0.1f, 5.0f)]
        public float stdevScale = 1.6f;

        [Range(0.01f, 5.0f)]
        public float tau = 1.0f;

        public bool thresholding = true;

        public bool tanh = false;

        [Range(0.01f, 100.0f)]
        public float phi = 1.0f;

        [Range(-1.0f, 1.0f)]
        public float threshold = 0.005f;

        public bool invert = false;

        public void OnSetup(ref RenderingData renderingData) {
            
            material.SetInt("_GaussianKernelSize", gaussianKernelSize);
            material.SetFloat("_Sigma", stdev);
            material.SetFloat("_K", stdevScale);
            material.SetFloat("_Tau", tau);
            material.SetFloat("_Phi", phi);
            material.SetFloat("_Threshold", threshold);
            material.SetInt("_Thresholding", thresholding ? 1 : 0);
            material.SetInt("_Invert", invert ? 1 : 0);
            material.SetInt("_Tanh", tanh ? 1 : 0);

        }
        
    }

    class CustomRenderPass : ScriptableRenderPass {

        private CustomSettings m_Settings;
        private RenderTargetIdentifier m_CameraColorBuffer;

        private RenderTargetIdentifier m_OutputColorBuffer0;
        private int m_OutputBufferID_0 = 0;

        private RenderTargetIdentifier m_OutputColorBuffer;
        private int m_OutputBufferID = 0;

        RenderTexture gaussian1;
        RenderTexture gaussian2;

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
            
            gaussian1 = RenderTexture.GetTemporary(descriptor.width, descriptor.height, 0, RenderTextureFormat.RG32);
            gaussian2 = RenderTexture.GetTemporary(descriptor.width, descriptor.height, 0, RenderTextureFormat.RG32);
            
            
            m_Settings.OnSetup(ref renderingData);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(m_Settings.passName))) {
                // Blit from the camera into tmp tex using material
                // Then blit that tmp tex back into cam so it renders to screen.
                Blit(cmd, m_CameraColorBuffer, gaussian1, m_Settings.material, 0);
                Blit(cmd, gaussian1, gaussian2, m_Settings.material, 1);

                m_Settings.material.SetTexture("_GaussianTex", gaussian2);

                Blit(cmd, m_CameraColorBuffer, m_OutputColorBuffer, m_Settings.material, 2);
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
                RenderTexture.ReleaseTemporary(gaussian1);
                RenderTexture.ReleaseTemporary(gaussian2);
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


