using System.Collections.Generic;
using Pooling;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HierarchyManager : MonoSingleton<HierarchyManager>
{
    [SerializeField] private XMLLoader xmlLoader;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private GameObject buttonPrefab;

    private Transform hierarchyContainer => scrollView != null ? scrollView.content : this.transform;

    private HierarchyButton currentHierarchy = null;

    public void OnLoadXml()
    {
        if (xmlLoader.LoadXML(out var folderTree))
        {
            CreateHierarchy(folderTree);
        }
    }

    private void CreateHierarchy(FolderTree folderTree)
    {
        RemoveCurrentHierarchy();
        if (buttonPrefab != null)
        {
            currentHierarchy = SpawnHierarchyButton();
            currentHierarchy.SetFolderTree(folderTree);
            currentHierarchy.OnButtonClick();
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

    // private HierarchyButton CreateFolder(FolderTree folderTree)
    // {
    //     var folderButton = GetPooledButton();
    //     if (folderButton != null)
    //     {
    //         folderButton.SetLabel(folderTree.folderName);
    //         List<HierarchyButton> childrenButtons = new List<HierarchyButton>();
    //         if (folderTree.children != null && folderTree.children.Count > 0)
    //         {
    //             folderTree.children.ForEach(child => {
    //                 childrenButtons.Add(CreateFolder(child));
    //             });
    //             folderButton.AddChildren(childrenButtons);
    //         }
    //     }
    //     return folderButton;
    // }

    // private static HierarchyButton CreateHierarchyButton()
    // {
    //     if (buttonPrefab)
    //     {
    //         HierarchyButton newButton =  Instantiate(buttonPrefab, hierarchyContainer);
    //         return newButton;
    //     }
    //     else
    //     {
    //         return null;
    //     }
    // }
}
