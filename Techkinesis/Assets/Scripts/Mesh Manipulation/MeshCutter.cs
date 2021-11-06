using UnityEngine;
using Parabox.CSG;

// Performs CSG operations a mesh | Base: karl- (https://bit.ly/30sqtiL)

public static class MeshCutter
{
    enum BoolOp
    {
        Union,
        SubtractLR,
        SubtractRL,
        Intersect
    };

    public static Rigidbody CutAndReturnRandomMesh(RaycastHit hit, float minMeshSize, float maxMeshSize, bool actuallyCutMesh)
    {
        // We actually create two objects, one for the boolean operation and one to be part of the shield so they need to be the same size
        float meshSize = Random.Range(minMeshSize, maxMeshSize);

        // Create the object that the player will see / which will be thrown
        Rigidbody randomMesh = MeshCutoutCreator.CreateMesh(hit.point, meshSize, hit.collider.GetComponent<MeshRenderer>().sharedMaterials).AddComponent<Rigidbody>();
        randomMesh.tag = TagNameManager.LAUNCHABLE;
        randomMesh.gameObject.layer = LayerMask.NameToLayer(LayerNameManager.IGNORE_PLAYER_COLLISION);

        if (actuallyCutMesh)
        {
            GameObject one = hit.collider.gameObject;                            // The object that will have the hole cut out of it
            GameObject two = MeshCutoutCreator.CreateMesh(hit.point, meshSize);  // The object that will be used to cut out the other object
            DoOperation(BoolOp.SubtractLR, one, two);                            // Perform the boolean operation
        }
        else
        {
            DebugLogManager.Print("TODO: Cool mesh breaking shader magic / particles would go here! " +
                                  "But.. if someone good at this sort of stuff wanted to do it then the position to create the effect is " + hit.point
                                  + " :))", DebugLogManager.OutputType.HALF_TODO_HALF_NOT_MY_JOB);
        }

        return randomMesh;
    }

    static void DoOperation(BoolOp operation, GameObject left, GameObject right)
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
        composite.tag = TagNameManager.CUTTABLE;

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

/** 
    TODO: Think about some optimisation here. Dynamic mesh cutting is well hard so if don't want to go down that rabbit hole just discuss them as well
    as the benefits and limitations of this approach in the writeup
        - using Unity.Jobs;
**/