using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

public class XMLLoader : MonoBehaviour
{
    public string xmlFileName = "SP3DTrain_A2.xml";

    public bool LoadXML(out FolderTree folderTree)
    {
        string path = Path.Combine(Application.streamingAssetsPath, xmlFileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"XML file not found: {path}");
            folderTree = null;
            return false;
        }

        XDocument xmlDoc = XDocument.Load(path);

        folderTree = ConverterToFolderTree(xmlDoc.Root, 0);
        return true;
    }

    FolderTree ConverterToFolderTree(XElement element, int indentLevel)
    {
        FolderTree result = new FolderTree();
        
        // Convert folder name
        result.folderName = new string(' ', indentLevel * 4) + element.Attribute("text")?.Value;
        
        // Conver folder children
        List<FolderTree> children = new List<FolderTree>();
        foreach (XElement child in element.Elements())
        {
            children.Add(ConverterToFolderTree(child, indentLevel + 1));
        }
        result.children = children;

        return result;
    }
}

public class FolderTree
{
    public string folderName;
    public List<FolderTree> children;
}
