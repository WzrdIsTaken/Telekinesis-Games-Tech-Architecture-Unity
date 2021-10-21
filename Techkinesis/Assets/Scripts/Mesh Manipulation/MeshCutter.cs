using UnityEngine;
using Parabox.CSG;

// using Unity.Jobs;
// TODO: Think about some optimisation here

// I can't be asked to actually code any optimisiations but can discuss them. Eg multithreading, 
// spliting the map up into sections so don't have to cut as big of a mesh, etc

// Performs CSG operations a mesh | Credot: karl- (https://bit.ly/30sqtiL)

public static class MeshCutter
{
    public enum BoolOp
    {
        Union,
        SubtractLR,
        SubtractRL,
        Intersect
    }; 

    public static void DoOperation(BoolOp operation, GameObject left, GameObject right)
    {
        Model result = null;

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
        composite.AddComponent<MeshRenderer>().sharedMaterials = left.GetComponent<MeshRenderer>().sharedMaterials;  // result.Materials.ToArray();
        composite.AddComponent<MeshCollider>();

        GenerateBarycentric(composite);

        Object.Destroy(left);
        Object.Destroy(right);
    }

    static void GenerateBarycentric(GameObject go)
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