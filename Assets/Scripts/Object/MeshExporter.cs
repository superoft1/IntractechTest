using System.IO;
using UnityEngine;
using SFB;
using System.Collections.Generic;

public class MeshExporter : MonoBehaviour
{
    private readonly string fileExtension = "obj";

    public void ExportMesh(List<GameObject> exportObjList)
    {
        List<MeshFilter> exportMeshList = new List<MeshFilter>();
        if (exportObjList != null)
        {
            exportObjList.ForEach(obj => {
                if (obj.TryGetComponent<MeshFilter>(out var meshFilter) && meshFilter.mesh != null)
                {
                    exportMeshList.Add(meshFilter);
                }
                else
                {
                    NotificationHelper.SHOW_ERROR_NOTI?.Invoke($"No mesh found in object {obj.name}");
                    // Debug.LogError("No mesh found in object " + obj.name);
                }
            });
        }

        if (exportMeshList.Count <= 0)
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke($"No mesh found to export!");
            // Debug.LogError("No mesh found to export!");
            return;
        }

        var folderPath = StandaloneFileBrowser.OpenFolderPanel("Select Save Folder", "", false);
        if (folderPath.Length == 0 || string.IsNullOrEmpty(folderPath[0]))
        {
            // Debug.Log("Export cancelled.");
            NotificationHelper.SHOW_WARNING_NOTI?.Invoke($"Export cancelled.");
            return;
        }

        string saveDirectory = folderPath[0];

        foreach (MeshFilter meshFilter in exportMeshList)
        {
            string objectName = meshFilter.gameObject.name.Replace(" ", "_");
            string filePath = Path.Combine(saveDirectory, objectName + "." + fileExtension);

            SaveMeshAsOBJ(meshFilter.mesh, filePath);
        }

        NotificationHelper.SHOW_SUCCESS_NOTI?.Invoke($"Exported {exportMeshList.Count} meshes to {saveDirectory}");
        // Debug.Log($"Exported {exportMeshList.Count} meshes to {saveDirectory}");
    }

    void SaveMeshAsOBJ(Mesh mesh, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("# Exported Mesh from Unity");

            // Write vertices
            foreach (Vector3 v in mesh.vertices)
            {
                writer.WriteLine($"v {v.x} {v.y} {v.z}");
            }

            // Write normals
            foreach (Vector3 n in mesh.normals)
            {
                writer.WriteLine($"vn {n.x} {n.y} {n.z}");
            }

            // Write UVs (if available)
            foreach (Vector2 uv in mesh.uv)
            {
                writer.WriteLine($"vt {uv.x} {uv.y}");
            }

            // Write faces (triangles)
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                writer.WriteLine($"f {triangles[i] + 1} {triangles[i + 1] + 1} {triangles[i + 2] + 1}");
            }
        }

        Debug.Log($"Mesh exported successfully to: {filePath}");
    }
}
