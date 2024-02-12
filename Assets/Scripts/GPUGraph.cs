using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader;

    [SerializeField, Range(10,200)] int resolution = 10;

    [SerializeField] FunctionLibrary.FunctionName function;

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resiolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");

    public enum TransitionMode { Cycle, Random }

    [SerializeField] TransitionMode transitionMode;

    [SerializeField, Min(0f)] float functionDuration = 1f;
    [SerializeField, Min(0f)] float transitionDuration = 1f;

    float duration;
    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionsBuffer;

    [SerializeField] Material material;
    [SerializeField] Mesh mesh;

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    private void Update()
    {
        duration += Time.deltaTime;
        if ( transitioning)
        {
            if (duration>= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();

        UpdateFunctionOnGPU();
        }
    }


    private void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }

    void UpdateFunctionOnGPU ()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resiolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);

        computeShader.SetBuffer(0, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        // deprecated
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);

        // new equivalent
        //RenderParams renderParams = new RenderParams(material);
        //renderParams.worldBounds = bounds;
        //Graphics.RenderMeshPrimitives(renderParams, mesh, 0, positionsBuffer.count);
    }
}
