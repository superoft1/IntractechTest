using System;
using System.IO;
using UnityEngine;
using SFB;
using System.Collections.Generic;

public class OBJLoader : MonoBehaviour
{
    public Material defaultMaterial;

    public void OpenFilePicker(Action<List<GameObject>> onComplete = null)
    {
        // Open file picker dialog for .obj files
        var paths = StandaloneFileBrowser.OpenFilePanel("Select OBJ File", "", "obj", true);

        List<GameObject> loadedObjects = new List<GameObject>();
        if (paths.Length > 0)
        {
            foreach (string path in paths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var newObj = LoadOBJFile(path);
                    if (newObj != null)
                    {
                        loadedObjects.Add(newObj);
                    }
                }
            }
        }
        onComplete?.Invoke(loadedObjects);
    }

    private GameObject LoadOBJFile(string filePath)
    {
        GameObject resultObject = null;
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("OBJ file not found!");
            // Debug.LogError("OBJ file not found!");
            return null;
        }

        string fileName = Path.GetFileNameWithoutExtension(filePath);
        GameObject objModel = new GameObject(fileName);
        MeshFilter meshFilter = objModel.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = objModel.AddComponent<MeshRenderer>();

        meshRenderer.material = defaultMaterial != null ? defaultMaterial : new Material(Shader.Find("Standard"));

        Mesh loadedMesh = LoadObjectMesh(filePath);
        if (loadedMesh != null)
        {
            
            meshFilter.mesh = loadedMesh;
            objModel.transform.position = Vector3.zero;
            objModel.transform.localScale = Vector3.one;
            resultObject = objModel;
        }
        else
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke($"Failed to load OBJ file! {filePath}");
            //Debug.LogError($"Failed to load OBJ file! {filePath}");
        }
        return resultObject;
    }

    Mesh LoadObjectMesh(string filePath)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                if (parts.Length < 2) continue;

                if (parts[0] == "v") // Vertex
                    vertices.Add(new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                else if (parts[0] == "f") // Face (Triangle)
                    for (int i = 1; i <= 3; i++)
                        triangles.Add(int.Parse(parts[i].Split('/')[0]) - 1);
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            mesh.RecalculateNormals();

            return mesh;
        }
        catch
        {
            // Debug.LogError("Error reading OBJ file.");
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("Error reading OBJ file.");
            return null;
        }
    }
}
