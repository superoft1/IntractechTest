using System.Collections.Generic;
using Pooling;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HierarchyManager : MonoSingleton<HierarchyManager>
{
    [SerializeField] private Transform hierarchyView;
    [SerializeField] private ScrollRect scrollView;

    [SerializeField] private XMLLoader xmlLoader;
    [SerializeField] private GameObject buttonPrefab;

    private Transform hierarchyContainer => scrollView != null ? scrollView.content : this.transform;

    private HierarchyButton currentHierarchy = null;

    public void ToggleHierarchyView(bool toggle)
    {
        if (hierarchyView)
        {
            hierarchyView.gameObject.SetActive(toggle);
        }
    }

    public void OnLoadXml()
    {
        if (xmlLoader)
        {
            xmlLoader.OpenFilePicker(CreateHierarchy);
        }
    }

    private void CreateHierarchy(FolderTree folderTree)
    {
        if (folderTree != null)
        {
            RemoveCurrentHierarchy();
            ToggleHierarchyView(true);
            if (buttonPrefab != null)
            {
                currentHierarchy = SpawnHierarchyButton();
                currentHierarchy.SetFolderTree(folderTree);
                currentHierarchy.OnButtonClick();
                NotificationHelper.SHOW_SUCCESS_NOTI?.Invoke("Load XML file successfully!");
            }
        }
    }

    private void RemoveCurrentHierarchy()
    {
        if (currentHierarchy != null)
        {
            currentHierarchy.Despawn();
        }
    }

    public HierarchyButton SpawnHierarchyButton()
    {
        return buttonPrefab.Spawn(hierarchyContainer).GetComponent<HierarchyButton>();
    }
}
