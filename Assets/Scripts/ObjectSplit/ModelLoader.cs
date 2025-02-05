using UnityEngine;

public class ModelLoader : MonoBehaviour
{
    public string modelName = "teapot"; // Ensure teapot.obj is placed in Resources

    void Start()
    {
        GameObject model = Instantiate(Resources.Load<GameObject>(modelName));
        model.transform.position = Vector3.zero;
    }
}
