using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoSingleton<ObjectManager>
{
    [SerializeField] private OBJLoader objLoader;
    [SerializeField] private ObjectMover objMover;
    [SerializeField] private MeshSplitter meshSplitter;
    [SerializeField] private MeshExporter meshExporter;
    [SerializeField] private LayerMask raycastLayer;

    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

#region Load Object
    private List<GameObject> CurrentLoadedObjects = new List<GameObject>();

    public void LoadObject()
    {
        if (objLoader)
        {
            objLoader.OpenFilePicker(onComplete: OnObjectLoaded);
        }
        else
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("OBJLoader not found, please add in Unity!");
        }
    }

    private void OnObjectLoaded(List<GameObject> listObj)
    {
        // Remove old models
        RemoveLoadedObjects();
        RemoveSplittedParts();

        // Show new loaded object
        if (listObj != null && listObj.Count > 0)
        {
            NotificationHelper.SHOW_SUCCESS_NOTI?.Invoke($"Loaded {listObj.Count} file(s) success!");
            listObj.ForEach(obj => {
                obj.SetActive(true);
                obj.transform.SetParent(this.transform);
                if (objMover)
                {
                    objMover.MakeMoveableObject(obj);
                }
            });
            CurrentLoadedObjects = listObj;
        }
    }

    private void RemoveLoadedObjects()
    {
        if (CurrentLoadedObjects != null && CurrentLoadedObjects.Count > 0)
        {
            foreach (var obj in CurrentLoadedObjects)
            {
                Destroy(obj);
            }
            CurrentLoadedObjects.Clear();
        }
    }
#endregion

#region Split Object
    private List<GameObject> CurrentSplittedParts = new List<GameObject>();
    private GameObject currentSelectedObject = null;
    public void SplitObject()
    {
        if (meshSplitter == null)
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("MeshSplitter not found, please add in Unity!");
            return;
        }

        if (currentSelectedObject == null)
        {
            NotificationHelper.SHOW_WARNING_NOTI?.Invoke("Please select object to split!");
            return;
        }

        meshSplitter.SplitMesh(currentSelectedObject, onComplete: OnObjectSplitted);
    }

    private void OnObjectSplitted(List<GameObject> objList)
    {
        // Remove old models
        RemoveSplittedParts();
        currentSelectedObject.gameObject.SetActive(false);

        // Show new loaded parts
        NotificationHelper.SHOW_SUCCESS_NOTI?.Invoke($"Split {currentSelectedObject.name} successfully!");
        CurrentSplittedParts = objList;
        CurrentSplittedParts.ForEach(obj => {
            obj.SetActive(true);
            obj.transform.SetParent(this.transform);
            if (objMover)
            {
                objMover.MakeMoveableObject(obj);
            }
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

#region Mesh Exporter
    public void ExportObject()
    {
        if (CurrentSplittedParts == null || CurrentSplittedParts.Count <= 0)
        {
            NotificationHelper.SHOW_WARNING_NOTI?.Invoke("Please plit object before export!");
            return;
        }

        if (meshExporter == null)
        {
            NotificationHelper.SHOW_ERROR_NOTI?.Invoke("MeshExporter not found, please add in Unity!");
            return;
        }

        meshExporter.ExportMesh(CurrentSplittedParts);
    }
#endregion

#region Select Object
    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayer)
                && hit.collider.gameObject.CompareTag(Utilities.MoveableObjectTag))
            {
                currentSelectedObject = hit.collider.gameObject;
            }
        }
    }
#endregion
}
