using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pooling;

public class HierarchyButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Transform childrenContainer;
    // [SerializeField] private GameObject hierarchyButtonPrefab;

    private HierarchyButton parentHierarchy = null;
    private List<HierarchyButton> hierarchyChildren = new List<HierarchyButton>();
    private RectTransform rect = null;
    private FolderTree currentFolder;
    private bool isOpenChildren = false;

    private void Awake() {
        rect = this.transform as RectTransform;
    }

    private void OnEnable() {
        RemoveChildren();
    }

    public void SetFolderTree(FolderTree tree)
    {
        currentFolder = tree;
        SetLabel(currentFolder.folderName);
    }

    private void SetLabel(string text)
    {
        if (label)
        {
            label.text = text;
        }
    }

    public void Despawn()
    {
        this.parentHierarchy = null;
        this.currentFolder = null;
        SetLabel("");
        RemoveChildren();
        this.gameObject.Despawn();
    }
    
#region Children Manager
    public void SetParent(HierarchyButton parent)
    {
        this.parentHierarchy = parent;
    }

    private void CreateChildren()
    {
        isOpenChildren = true;
        RemoveChildren();
        if (currentFolder != null && currentFolder.children != null)
        {
            Debug.Log($"Create [{currentFolder.children.Count}] children of {this.currentFolder.folderName}");
            foreach (var child in currentFolder.children)
            {
                var newChildButton = SpawnChild(child);
                if (newChildButton != null)
                {
                    hierarchyChildren.Add(newChildButton);
                }
            }
        }
    }

    private HierarchyButton SpawnChild(FolderTree folder)
    {
        HierarchyButton newChild = HierarchyManager.Instance.SpawnHierarchyButton();
        if (newChild)
        {
            newChild.transform.SetParent(this.childrenContainer);
            newChild.SetParent(this);
            newChild.SetFolderTree(folder);
        }
        return newChild;
    }

    private void RemoveChildren()
    {
        isOpenChildren = false;
        if (hierarchyChildren != null && hierarchyChildren.Count > 0)
        {
            Debug.Log($"Remove {hierarchyChildren.Count} children of {this.currentFolder.folderName}");
            foreach (var folder in hierarchyChildren)
            {
                folder.Despawn();
            }
            hierarchyChildren.Clear();
        }
    }
#endregion

#region Layout
    private void ToggleContent(bool toggle)
    {
        if (hierarchyChildren != null)
        {
            hierarchyChildren.ForEach(child => 
            {
                child.ToggleButton(toggle);
            });
        }
    }

    public void ToggleButton(bool toggle)
    {
        this.gameObject.SetActive(toggle);
        ToggleContent(toggle);
        ForceUpdateLayout();
    }

    private void UpdateParent()
    {
        if (parentHierarchy)
        {
            parentHierarchy.ForceUpdateLayout();
        }
    }

    public void ForceUpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        UpdateParent();
    }
#endregion

#region Event
    public void OnButtonClick()
    {
        if (isOpenChildren)
        {
            RemoveChildren();
        }
        else
        {
            CreateChildren();
        }
    }
#endregion
}
