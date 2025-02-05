using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XMLLoader : MonoBehaviour
{
    public Button loadXmlButton;
    public Transform contentPanel;
    public GameObject textPrefab;  // UI prefab for displaying hierarchy

    private void Start()
    {
        loadXmlButton.onClick.AddListener(LoadXML);
    }

    void LoadXML()
    {
        string path = UnityEngine.Application.streamingAssetsPath + "/SP3DTrain_A2.xml";
        XDocument xmlDoc = XDocument.Load(path);

        TransformHierarchy(xmlDoc.Root, contentPanel, 0);
    }

    void TransformHierarchy(XElement element, Transform parent, int indentLevel)
    {
        GameObject newText = Instantiate(textPrefab, parent);
        newText.GetComponent<TextMeshProUGUI>().text = new string(' ', indentLevel * 4) + element.Attribute("text")?.Value;

        foreach (XElement child in element.Elements())
        {
            TransformHierarchy(child, parent, indentLevel + 1);
        }
    }
}
