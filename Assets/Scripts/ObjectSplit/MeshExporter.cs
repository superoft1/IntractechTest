using System.IO;
using UnityEngine;

public class MeshExporter : MonoBehaviour
{
    public void ExportMesh(Mesh mesh, string fileName)
    {
        string path = Application.dataPath + "/ExportedModels/" + fileName + ".obj";

        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine("o " + fileName);

            foreach (Vector3 v in mesh.vertices)
            {
                sw.WriteLine($"v {v.x} {v.y} {v.z}");
            }

            foreach (Vector3 n in mesh.normals)
            {
                sw.WriteLine($"vn {n.x} {n.y} {n.z}");
            }

            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sw.WriteLine($"f {triangles[i] + 1} {triangles[i + 1] + 1} {triangles[i + 2] + 1}");
            }
        }

        Debug.Log($"Exported {fileName}.obj to {path}");
    }
}
