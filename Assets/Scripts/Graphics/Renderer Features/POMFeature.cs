using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Gobblefish.Graphics;

public class POMFeature : ScriptableRendererFeature {

    [System.Serializable]
    public class CustomMaskPass {

        public string passName = "pass name";

        public Material blendMaterial;
        public Material[] mainMaterials;
        public bool bypass = false;

    }

    [System.Serializable]
    public class CustomSettings { // ScriptableRenderPassSettings

        public string passName = "POM Pass";
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        public CustomMaskPass[] passes;

    }

    class CustomRenderPass : ScriptableRenderPass {

        private CustomSettings m_Settings;
        private RenderTargetIdentifier m_CameraColorBuffer;

        private RenderTargetIdentifier[] m_OutputColorBuffer;
        private RenderTargetIdentifier[] m_PassBuffers;

        public CustomRenderPass(CustomSettings settings) {
            this.m_Settings = settings;
            this.renderPassEvent = settings.renderPassEvent;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {

            m_CameraColorBuffer = renderingData.cameraData.renderer.cameraColorTarget;

            // Create a tmp texture of the same dims.
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(0, descriptor, FilterMode.Point);
            cmd.GetTemporaryRT(1, descriptor, FilterMode.Point);

            if (m_OutputColorBuffer == null || m_OutputColorBuffer.Length < 2) {
                m_OutputColorBuffer = new RenderTargetIdentifier[2];
            }

            if (m_PassBuffers == null || m_PassBuffers.Length < m_Settings.passes.Length) {
                m_PassBuffers = new RenderTargetIdentifier[m_Settings.passes.Length];
            }

            m_OutputColorBuffer[0] = new RenderTargetIdentifier(0);
            m_OutputColorBuffer[1] = new RenderTargetIdentifier(1);
            
            for (int n = 0; n < m_Settings.passes.Length; n++) {
                cmd.GetTemporaryRT(n+2, descriptor, FilterMode.Point);
                m_PassBuffers[n] = new RenderTargetIdentifier(n+2);
            }
            
            // m_Settings.OnSetup(ref renderingData);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(m_Settings.passName))) {
                
                // Blit from the camera into tmp tex using material
                // Then blit that tmp tex back into cam so it renders to screen.

                for (int n = 0; n < m_Settings.passes.Length; n++) {
                    Blit(cmd, m_CameraColorBuffer, m_OutputColorBuffer[0], null);

                    int index = ExecutePass(cmd, n);
                    Blit(cmd, m_OutputColorBuffer[index], m_PassBuffers[n], m_Settings.passes[n].blendMaterial);
                }

                for (int n = 0; n < m_Settings.passes.Length; n++) {
                    if (!m_Settings.passes[n].bypass) {
                        // m_Settings.passes.blendMaterial.SetTexture("_BlendTex", m_PassBuffers[n]);
                        // Blit(cmd, m_CameraColorBuffer, m_OutputColorBuffer[0], m_Settings.passes.blendMaterial);
                        Blit(cmd, m_PassBuffers[n], m_CameraColorBuffer, m_Settings.passes[n].blendMaterial);
                    }
                }

            }

            // Execute then release.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

        public int ExecutePass(CommandBuffer cmd, int passId) {
            CustomMaskPass pass = m_Settings.passes[passId];
            if (pass.bypass) {
                return 0;
            }

            int index = 0;
            for (int i = 0; i < pass.mainMaterials.Length; i++) {
                if (pass.mainMaterials[i] != null) {
                    Blit(cmd, m_OutputColorBuffer[index], m_OutputColorBuffer[1-index], pass.mainMaterials[i]);
                    index = 1-index;
                }
            }

            return index;

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd) {
            if (cmd != null) {
                cmd.ReleaseTemporaryRT(0);
                cmd.ReleaseTemporaryRT(1);
                for (int n = 0; n < m_Settings.passes.Length; n++) {
                    cmd.ReleaseTemporaryRT(n+2);
                }
            }
        }

    }

    [SerializeField]
    private CustomSettings m_Settings;
    private CustomRenderPass m_ScriptablePass;

    public override void Create() {
        // m_Settings.material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_ScriptablePass = new CustomRenderPass(m_Settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing) {
        // CoreUtils.Destroy(m_Material);
    }

}


