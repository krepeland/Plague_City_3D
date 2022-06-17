using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlinePass : ScriptableRenderPass
{
    private string _profilerTag = "Outline";
    private Material _material;

    public OutlinePass(Material material)
    {
        _material = material;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get(_profilerTag);

        using (new ProfilingScope(cmd, new ProfilingSampler(_profilerTag)))
        {            
            var mesh = RenderingUtils.fullscreenMesh;
            cmd.DrawMesh(mesh, Matrix4x4.identity, _material, 0, 0);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
