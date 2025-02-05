using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyButton : MonoBehaviour
{
    private HierarchyButton parentHierarchy = null;
    private List<HierarchyButton> hierarchyChildren = new List<HierarchyButton>();
    private RectTransform rect = null;

    private void Awake() {
        rect = this.transform as RectTransform;
    }
    
    public void SetParent(HierarchyButton parent)
    {
        this.parentHierarchy = parent;
    }

    public void AddChild(HierarchyButton child)
    {
        if (child != null && !hierarchyChildren.Contains(child))
        {
            child.SetParent(this);
            hierarchyChildren.Add(child);
        }
    }

    public void AddChildren(List<HierarchyButton> children)
    {
        if (children != null && children.Count > 0)
        {
            children.ForEach(child => {
                AddChild(child);
            });
        }
    }

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

    public void OnButtonClick()
    {

    }
}
