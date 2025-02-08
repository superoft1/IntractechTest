using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoSingleton<ObjectManager>
{
    [SerializeField] private OBJLoader objLoader;
    [SerializeField] private MeshSplitter meshSplitter;

    private GameObject CurrentLoadedObject = null;
    private List<GameObject> CurrentSplittedObjects = new List<GameObject>();

    public void LoadObject()
    {
        if (objLoader)
        {
            objLoader.OpenFilePicker(onComplete: OnObjectLoaded);
        }
    }

    private void OnObjectLoaded(GameObject obj)
    {
        obj.transform.SetParent(this.transform);
        CurrentLoadedObject = obj;
    }

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
        CurrentSplittedObjects = objList;
        CurrentSplittedObjects.ForEach(obj => {
            obj.transform.SetParent(this.transform);
        });
    }
}
