using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private LayerMask moveableLayer;

    private Camera mainCamera;
    private Transform selectedObject;
    private Vector3 offset;
    private float objectZPosition;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void MakeMoveableObject(GameObject obj)
    {
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            obj.AddComponent<BoxCollider>();
        }
        obj.tag = Utilities.MoveableObjectTag;
        int layer = LayerMask.NameToLayer(Utilities.ObjectLayer);
        if (layer >= 0)
        {
            obj.layer = layer;
        }
        
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left click to select object
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, moveableLayer)
                && hit.collider.gameObject.CompareTag(Utilities.MoveableObjectTag))
            {
                selectedObject = hit.transform;
                objectZPosition = mainCamera.WorldToScreenPoint(selectedObject.position).z;
                offset = selectedObject.position - GetMouseWorldPosition();
            }
        }

        if (Input.GetMouseButton(0) && selectedObject != null) // Drag object
        {
            selectedObject.position = GetMouseWorldPosition() + offset;
        }

        if (Input.GetMouseButtonUp(0)) // Release object
        {
            selectedObject = null;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = objectZPosition;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
