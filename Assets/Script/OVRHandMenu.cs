using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class OVRHandMenu : MonoBehaviour
{
    [SerializeField]
    private OVRCameraRig cameraRig;

    [SerializeField]
    private GameObject targetHand;

    [SerializeField]
    private GameObject HandMenu;

    [SerializeField]
    private Vector3 offsetFromHand = new Vector3(0.5f, 0.5f, 0.5f);
    
    // menuState
    [SerializeField]
    private InteractableUnityEventWrapper SeethroughButton;
    private bool SeethroughOn = true;

    private void Awake()
    {
        
        // SeethroughButton.WhenSelect.AddListener(() =>
        // {
        //     SeethroughOn = !SeethroughOn;
        //     var seethroughMenuOption = SeethroughButton.GetComponentInChildren<TextMeshPro>();
        //     var seethroughMenuState = SeethroughOn ? "ON" : "OFF";
        //     seethroughMenuOption.text = $" {seethroughMenuState}";
        //     
        // });
    }

    public void SeethroughButtonVisibility(bool state)
    {
        HandMenu.SetActive(state);
    }

    private void Update()
    {
        HandMenu.transform.position = targetHand.transform.position + offsetFromHand;
        HandMenu.transform.rotation = Quaternion.LookRotation(HandMenu.transform.position - cameraRig.centerEyeAnchor.transform.position, Vector3.up);
    }

}
    