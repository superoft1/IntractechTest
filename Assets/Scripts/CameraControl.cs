using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lookSpeed = 3f;
    [SerializeField] private float zoomSpeed = 10f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -90, 90);
            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 move = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0) * moveSpeed * Time.deltaTime;
            transform.position += transform.right * move.x + transform.up * move.y;
        }
    }
}
