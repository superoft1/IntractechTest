using System.Collections.Generic;
using UnityEngine;

public class MeshSplitter : MonoBehaviour
{
    public enum SplitMode { TopLeft, TopRight, BottomLeft, BottomRight }
    public SplitMode splitMode;

    private Mesh originalMesh;
    private Vector3 center;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("No MeshFilter found!");
            return;
        }

        originalMesh = meshFilter.mesh;
        center = GetMeshCenter();

        CreateMeshPart(splitMode);
    }

    Vector3 GetMeshCenter()
    {
        Bounds bounds = originalMesh.bounds;
        return bounds.center; // Get center of the bounding box
    }

    void CreateMeshPart(SplitMode mode)
    {
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;

        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        Dictionary<int, int> vertexRemap = new Dictionary<int, int>();

        // Step 1: Filter vertices based on position
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 v = originalVertices[i];

            bool isValid = false;
            if (mode == SplitMode.TopLeft && v.x < center.x && v.z > center.z) isValid = true;
            if (mode == SplitMode.TopRight && v.x > center.x && v.z > center.z) isValid = true;
            if (mode == SplitMode.BottomLeft && v.x < center.x && v.z < center.z) isValid = true;
            if (mode == SplitMode.BottomRight && v.x > center.x && v.z < center.z) isValid = true;

            if (isValid)
            {
                vertexRemap[i] = newVertices.Count;
                newVertices.Add(v);
            }
        }

        // Step 2: Filter triangles to ensure only valid vertices are referenced
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            int v1 = originalTriangles[i];
            int v2 = originalTriangles[i + 1];
            int v3 = originalTriangles[i + 2];

            if (vertexRemap.ContainsKey(v1) && vertexRemap.ContainsKey(v2) && vertexRemap.ContainsKey(v3))
            {
                newTriangles.Add(vertexRemap[v1]);
                newTriangles.Add(vertexRemap[v2]);
                newTriangles.Add(vertexRemap[v3]);
            }
        }

        // Step 3: Create a new GameObject for this mesh part
        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals();

        GameObject part = new GameObject($"Teapot_{mode}");
        part.AddComponent<MeshFilter>().mesh = newMesh;
        part.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;

        // Position it next to the original for visibility
        Vector3 offset = new Vector3((mode == SplitMode.TopRight || mode == SplitMode.BottomRight) ? 1f : -1f, 0, (mode == SplitMode.TopLeft || mode == SplitMode.TopRight) ? 1f : -1f);
        part.transform.position = transform.position + offset;
    }
}
