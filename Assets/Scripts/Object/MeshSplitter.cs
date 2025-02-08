using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshSplitter : MonoBehaviour
{
    public enum SplitMode { TopLeft, TopRight, BottomLeft, BottomRight }

    private Mesh originalMesh;
    private Material originalMaterial;
    private GameObject splitObject;
    private Vector3 center;


    public void SplitMesh(GameObject splitObject, Action<List<GameObject>> onComplete = null)
    {
        // Verify game object befoore split
        if (splitObject == null)
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("Object is empty!");
            //Debug.LogError("Object is empty!");
            return;
        }
        else
        {
            this.splitObject = splitObject;
        }

        if (this.splitObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
        {
            originalMesh = meshFilter.mesh;
        }
        else
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("MeshFilter is empty!");
            //Debug.LogError("MeshFilter is empty!");
            return;
        }

        if (this.splitObject.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            originalMaterial = meshRenderer.material;
        }
        else
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("MeshRenderer is empty!");
            // Debug.LogError("MeshRenderer is empty!");
            return;
        }

        // SPlit mesh
        center = GetMeshCenter();
        List<GameObject> result = new List<GameObject>();
        foreach (SplitMode mode in (SplitMode[]) Enum.GetValues(typeof(SplitMode)))
        {
            result.Add(CreateMeshPart(mode));
        }
        onComplete?.Invoke(result);
    }

    private Vector3 GetMeshCenter()
    {
        Bounds bounds = originalMesh.bounds;
        return bounds.center;
    }

    private GameObject CreateMeshPart(SplitMode mode)
    {
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;

        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        Dictionary<int, int> vertexRemap = new Dictionary<int, int>();

        // Filter vertices based on position
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 v = originalVertices[i];

            bool isValid = false;
            switch (mode)
            {
                case SplitMode.TopLeft:
                    if (v.x <= center.x && v.y >= center.y) isValid = true;
                    break;
                case SplitMode.TopRight:
                    if (v.x >= center.x && v.y >= center.y) isValid = true;
                    break;
                case SplitMode.BottomLeft:
                    if (v.x <= center.x && v.y <= center.y) isValid = true;
                    break;
                case SplitMode.BottomRight:
                    if (v.x >= center.x && v.y <= center.y) isValid = true;
                    break;
            }

            if (isValid)
            {
                vertexRemap[i] = newVertices.Count;
                newVertices.Add(v);
            }
        }

        // Filter triangles to ensure only valid vertices are referenced
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

        // Create a new mesh part
        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals();

        GameObject part = new GameObject($"{this.splitObject.name}_{mode}");
        part.AddComponent<MeshFilter>().mesh = newMesh;
        part.AddComponent<MeshRenderer>().material = originalMaterial != null ? originalMaterial : new Material(Shader.Find("Standard"));

        // Reposition new mesh part
        Vector3 offset = new Vector3((mode == SplitMode.TopRight || mode == SplitMode.BottomRight) ? 1f : -1f, 0, (mode == SplitMode.TopLeft || mode == SplitMode.TopRight) ? 1f : -1f);
        part.transform.position = transform.position + offset;

        return part;
    }
}
