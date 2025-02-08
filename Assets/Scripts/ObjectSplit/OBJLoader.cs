using System;
using System.IO;
using UnityEngine;
using SFB;
using System.Collections.Generic;

public class OBJLoader : MonoBehaviour
{
    public Material defaultMaterial;
    private string selectedFilePath;

    public void OpenFilePicker(Action<GameObject> onComplete = null)
    {
        // Open file picker dialog for .obj files
        var paths = StandaloneFileBrowser.OpenFilePanel("Select OBJ File", "", "obj", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            selectedFilePath = paths[0];
            LoadOBJFile(onComplete);
        }
    }

    void LoadOBJFile(Action<GameObject> onComplete = null)
    {
        if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
        {
            Debug.LogError("OBJ file not found!");
            return;
        }

        GameObject objModel = new GameObject("ImportedObj");
        MeshFilter meshFilter = objModel.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = objModel.AddComponent<MeshRenderer>();

        meshRenderer.material = defaultMaterial != null ? defaultMaterial : new Material(Shader.Find("Standard"));

        Mesh loadedMesh = LoadObjectMesh(selectedFilePath);
        if (loadedMesh != null)
        {
            meshFilter.mesh = loadedMesh;
            objModel.transform.position = Vector3.zero;
            objModel.transform.localScale = Vector3.one;
            onComplete?.Invoke(objModel);
        }
        else
        {
            Debug.LogError("Failed to load OBJ file!");
        }
    }

    Mesh LoadObjectMesh(string filePath)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                if (parts.Length < 2) continue;

                if (parts[0] == "v") // Vertex
                {
                    vertices.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])));
                }
                else if (parts[0] == "vn") // Normal
                {
                    normals.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])));
                }
                else if (parts[0] == "vt") // UV
                {
                    uvs.Add(new Vector2(
                        float.Parse(parts[1]),
                        float.Parse(parts[2])));
                }
                else if (parts[0] == "f") // Face
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        string[] vertexData = parts[i].Split('/');
                        int vertexIndex = int.Parse(vertexData[0]) - 1;
                        triangles.Add(vertexIndex);
                    }
                }
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        catch
        {
            Debug.LogError("Error reading OBJ file.");
            return null;
        }
    }
}
