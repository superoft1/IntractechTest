using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    public void OnLoadXmlClick()
    {
        HierarchyManager.Instance?.OnLoadXml();
    }

    public void OnOpenObjectClick()
    {
        HierarchyManager.Instance?.ToggleHierarchyView(false);
        ObjectManager.Instance?.LoadObject();
    }

    public void OnSplitObjectClick()
    {
        HierarchyManager.Instance?.ToggleHierarchyView(false);
        ObjectManager.Instance?.SplitObject();
    }

    public void OnExportObjectClick()
    {
        HierarchyManager.Instance?.ToggleHierarchyView(false);
        ObjectManager.Instance?.ExportObject();
    }
}
