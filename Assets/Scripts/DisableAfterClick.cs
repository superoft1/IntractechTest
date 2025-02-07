using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DisableAfterClick : MonoBehaviour
{
    [SerializeField] private bool reEnableClick = false;
    [SerializeField] private float timeToEnableClick = 0.5f;

    private Button mButton;
    
    private void Awake() {
        this.mButton = this.GetComponent<Button>();
        mButton.onClick.AddListener(DisableClick);
    }

    private void DisableClick()
    {
        mButton.interactable = false;
        if (reEnableClick)
        {
            EnableClick_Cor();
        }
    }

    IEnumerator EnableClick_Cor()
    {
        yield return new WaitForSeconds(timeToEnableClick);
        mButton.interactable = true;
    }
}
