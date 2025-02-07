using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pooling;
using System.Collections;

[RequireComponent(typeof(Button))]
public class HierarchyButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button folderButton;
    [SerializeField] private Transform childrenContainer;
    // [SerializeField] private GameObject hierarchyButtonPrefab;
    
    private HierarchyButton parentHierarchy = null;
    private List<HierarchyButton> hierarchyChildren = new List<HierarchyButton>();
    private RectTransform rect = null;
    private FolderTree currentFolder;
    private bool isOpenChildren = false;

    private void Awake() {
        rect = this.transform as RectTransform;
        if (!folderButton)
        {
            folderButton = this.GetComponent<Button>();
        }
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
        // Debug.Log("Despawn " + currentFolder.folderName);
        RemoveChildren();
        this.parentHierarchy = null;
        this.currentFolder = null;
        SetLabel("");
        this.gameObject.Despawn();
    }
    
#region Children Manager
    public void SetParent(HierarchyButton parent)
    {
        this.parentHierarchy = parent;
    }

    private void CreateChildren()
    {
        RemoveChildren();
        if (currentFolder != null && currentFolder.children != null)
        {
            // Debug.Log($"Create [{currentFolder.children.Count}] children of {this.currentFolder.folderName}");
            foreach (var child in currentFolder.children)
            {
                var newChildButton = SpawnChild(child);
                if (newChildButton != null)
                {
                    hierarchyChildren.Add(newChildButton);
                }
            }
        }
        ForceUpdateLayout();
        isOpenChildren = true;
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
        if (hierarchyChildren != null && hierarchyChildren.Count > 0)
        {
            // Debug.Log($"Remove [{hierarchyChildren.Count}] children of {this.currentFolder.folderName}");
            foreach (var folder in hierarchyChildren)
            {
                folder.Despawn();
            }
            hierarchyChildren.Clear();
            ForceUpdateLayout();
        }
        isOpenChildren = false;
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
        else
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent as RectTransform);
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
        if (folderButton && folderButton.interactable)
        {
            DisableClick();
            if (isOpenChildren)
            {
                RemoveChildren();
            }
            else
            {
                CreateChildren();
            }
        }
    }

    private void DisableClick()
    {
        SetButtonInteractable(false);
        StartCoroutine(EnableClick_Cor());
    }

    IEnumerator EnableClick_Cor()
    {
        yield return new WaitForSeconds(0.2f);
        SetButtonInteractable(true);
    }

    private void SetButtonInteractable(bool interactable)
    {
        if (folderButton)
        {
            folderButton.interactable = interactable;
        }
    }
#endregion
}
