using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoSingleton<ObjectManager>
{
    [SerializeField] private OBJLoader objLoader;
    [SerializeField] private MeshSplitter meshSplitter;

#region Load Object
    private GameObject CurrentLoadedObject = null;

    public void LoadObject()
    {
        if (objLoader)
        {
            objLoader.OpenFilePicker(onComplete: OnObjectLoaded);
        }
    }

    private void OnObjectLoaded(GameObject obj)
    {
        // Remove old models
        RemoveLoadedObject();
        RemoveSplittedParts();

        // Show new loaded object
        obj.transform.SetParent(this.transform);
        CurrentLoadedObject = obj;
    }

    private void RemoveLoadedObject()
    {
        if (CurrentLoadedObject != null)
        {
            Destroy(CurrentLoadedObject);
        }
    }
#endregion

#region Split Object
    private List<GameObject> CurrentSplittedParts = new List<GameObject>();

    public void SplitObject()
    {
        if (meshSplitter == null)
        {
            return;
        }

        if (CurrentLoadedObject == null)
        {
            return;
        }

        meshSplitter.SplitMesh(CurrentLoadedObject, onComplete: OnObjectSplitted);
    }

    private void OnObjectSplitted(List<GameObject> objList)
    {
        // Remove old models
        RemoveSplittedParts();
        CurrentLoadedObject.gameObject.SetActive(false);

        // Show new loaded parts
        CurrentSplittedParts = objList;
        CurrentSplittedParts.ForEach(obj => {
            obj.SetActive(true);
            obj.transform.SetParent(this.transform);
        });
    }

    private void RemoveSplittedParts()
    {
        if (CurrentSplittedParts != null && CurrentSplittedParts.Count > 0)
        {
            foreach (var part in CurrentSplittedParts)
            {
                Destroy(part);
            }
            CurrentSplittedParts.Clear();
        }
    }
#endregion
}
