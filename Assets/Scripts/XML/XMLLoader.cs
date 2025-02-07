using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SFB;
using System;

public class XMLLoader : MonoBehaviour
{
    public FolderTree LoadedFolder { get; private set; }

    private string xmlFileName = "SP3DTrain_A2.xml";
    private string selectedFilePath;

    public void OpenFilePicker(Action<FolderTree> xmlFilePicked = null)
    {
        // Open file picker dialog (Windows, Mac, Linux)
#if UNITY_STANDALONE || UNITY_EDITOR
        var paths = StandaloneFileBrowser.OpenFilePanel("Select XML File", Application.dataPath, "xml", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            selectedFilePath = paths[0];
            LoadXML(selectedFilePath, onComplete: xmlFilePicked);
        }
#else
        LoadXML(Path.Combine(Application.streamingAssetsPath, xmlFileName), onComplete: xmlFilePicked);
#endif
    }

    private void LoadXML(string filePath, Action<FolderTree> onComplete = null)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogError("XML file not found! Path: " + filePath);
            return;
        }

        try
        {
            XDocument xmlDoc = XDocument.Load(filePath);
            LoadedFolder = ConverterToFolderTree(xmlDoc.Root, 0);
            onComplete?.Invoke(LoadedFolder);
        }
        catch
        {
            Debug.LogError("Invalid XML format!");
        }
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
