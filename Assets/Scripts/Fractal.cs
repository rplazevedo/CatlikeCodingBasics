
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int depth = 4;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;

    static Vector3[] directions =
    {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f),
    };

    struct FractalPart
    {
        public Vector3 direction;
        public Quaternion rotation;
        public Vector3 worldPosition;
        public Quaternion worldRotation;
        public float spinAngle;
    }

    FractalPart[][] parts;
    Matrix4x4[][] matrices;

    private void Awake()
    {
        parts = new FractalPart[depth][];
        matrices = new Matrix4x4[depth][];
        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) 
        {
            parts[i] = new FractalPart[length];
            matrices[i] = new Matrix4x4[length];
        }

        parts[0][0] = CreatePart(0);
        for (int li = 1; li < parts.Length; li++)
        {
            FractalPart[] levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
                for (int ci = 0; ci < 5; ci++)
                {  
                    levelParts[fpi + ci] = CreatePart(ci);
                }
        }
    }

    FractalPart CreatePart(int childIndex) =>
        new()
        {
            direction = directions[childIndex],
            rotation = rotations[childIndex],
        };

    public void Update()
    {
        float spingAngleDelta = 22.5f * Time.deltaTime;

        FractalPart rootPart = parts[0][0];
        rootPart.spinAngle += spingAngleDelta;
        rootPart.worldRotation = rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f);
        parts[0][0] = rootPart;
        matrices[0][0] = Matrix4x4.TRS(rootPart.worldPosition, rootPart.worldRotation, Vector3.one);

        float scale = 1f;
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            FractalPart[] parentParts = parts[li - 1];
            FractalPart[] levelParts = parts[li];
            Matrix4x4[] levelMatrices = matrices[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                FractalPart parent = levelParts[fpi / 5];
                FractalPart part = levelParts[fpi];
                part.spinAngle += spingAngleDelta;
                part.worldRotation = 
                    parent.worldRotation * (part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));
                part.worldPosition =
                    parent.worldPosition +
                    parent.worldRotation *
                    (1.5f *scale * part.direction);
                levelParts[fpi] = part;
                levelMatrices[fpi] = Matrix4x4.TRS(part.worldPosition, part.worldRotation, scale * Vector3.one);
            }
        }
    }
}
