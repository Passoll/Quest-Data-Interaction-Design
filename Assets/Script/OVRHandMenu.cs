using Oculus.Interaction;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class OVRHandMenu : MonoBehaviour
{
    [SerializeField]
    private OVRCameraRig cameraRig;

    [SerializeField]
    private GameObject targetHand;

    [SerializeField]
    private List<GameObject> HandMenu;

    [SerializeField]
    private List<Vector3> offsetFromHand = new List<Vector3>{ new Vector3(0.5f, 0.5f, 0.5f) };
    
    // menuState
    [SerializeField]
    private InteractableUnityEventWrapper SeethroughButton;
    private bool SeethroughOn = true;

    private void Awake()
    {
        if (HandMenu.Count != offsetFromHand.Count)
        {
            Debug.LogError("Hand Button and offset do not match");
        }

    }

    public void SeethroughButtonVisibility(bool state)
    {
        foreach (GameObject button in HandMenu)
        {
            button.SetActive(state);
        }
        
    }

    private void Update()
    {
        if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands))
        {
            int index = 0;
            foreach (GameObject button in HandMenu)
            {
                button.transform.position = targetHand.transform.position + offsetFromHand[index];
                button.transform.rotation = Quaternion.LookRotation(button.transform.position - cameraRig.centerEyeAnchor.transform.position, Vector3.up);
                index++;
            }
           
        }
        else
        {
            int index = 0;
            foreach (GameObject button in HandMenu)
            {
                if (!button.activeSelf) button.SetActive(true);
                button.transform.position = targetHand.transform.position + offsetFromHand[index];
                button.transform.rotation = Quaternion.LookRotation(button.transform.position - cameraRig.centerEyeAnchor.transform.position, Vector3.up);
                index++;
            }
           
        }

        // HandMenu.transform.position = targetHand.transform.position + offsetFromHand;
        // HandMenu.transform.rotation = Quaternion.LookRotation(HandMenu.transform.position - cameraRig.centerEyeAnchor.transform.position, Vector3.up);
        //
    }

}
    