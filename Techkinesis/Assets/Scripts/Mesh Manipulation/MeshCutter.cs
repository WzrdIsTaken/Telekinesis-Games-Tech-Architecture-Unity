using UnityEngine;
using Parabox.CSG;

// Performs CSG operations a mesh | Credit: karl- (https://bit.ly/30sqtiL)

public class MeshCutter : MonoBehaviour
{
    [SerializeField] GameObject one, two;

    public enum BoolOp
    {
        Union,
        SubtractLR,
        SubtractRL,
        Intersect
    };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) DoOperation(BoolOp.SubtractLR, one, two);
    }

    public void DoOperation(BoolOp operation, GameObject left, GameObject right)
    {
        Model result;

        switch (operation)
        {
            case BoolOp.Union:
                result = CSG.Union(left, right);
                break;

            case BoolOp.SubtractLR:
                result = CSG.Subtract(left, right);
                break;

            case BoolOp.SubtractRL:
                result = CSG.Subtract(right, left);
                break;

            default:
                result = CSG.Intersect(right, left);
                break;
        }

        GameObject composite = new GameObject();
        composite.AddComponent<MeshFilter>().sharedMesh = result.Mesh;
        composite.AddComponent<MeshRenderer>().sharedMaterials = result.Materials.ToArray();

        GenerateBarycentric(composite);

        Destroy(left);
        Destroy(right);
    }

    void GenerateBarycentric(GameObject go)
    {
        Mesh m = go.GetComponent<MeshFilter>().sharedMesh;

        if (m == null) return;

        int[] tris = m.triangles;
        int triangleCount = tris.Length;

        Vector3[] mesh_vertices = m.vertices;
        Vector3[] mesh_normals = m.normals;
        Vector2[] mesh_uv = m.uv;

        Vector3[] vertices = new Vector3[triangleCount];
        Vector3[] normals = new Vector3[triangleCount];
        Vector2[] uv = new Vector2[triangleCount];
        Color[] colors = new Color[triangleCount];

        for (int i = 0; i < triangleCount; i++)
        {
            vertices[i] = mesh_vertices[tris[i]];
            normals[i] = mesh_normals[tris[i]];
            uv[i] = mesh_uv[tris[i]];

            colors[i] = i % 3 == 0 ? new Color(1, 0, 0, 0) : (i % 3) == 1 ? new Color(0, 1, 0, 0) : new Color(0, 0, 1, 0);

            tris[i] = i;
        }

        Mesh wireframeMesh = new Mesh();

        wireframeMesh.Clear();
        wireframeMesh.vertices = vertices;
        wireframeMesh.triangles = tris;
        wireframeMesh.normals = normals;
        wireframeMesh.colors = colors;
        wireframeMesh.uv = uv;

        go.GetComponent<MeshFilter>().sharedMesh = wireframeMesh;
    }
}