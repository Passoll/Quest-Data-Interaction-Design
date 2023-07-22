/*
 * Written by: Ayse Zhong, 2023.7
 *
 * @OVRHandMenu
 * brief: This is the oculus hand menu controller that put on camera rig.
 * HandButtonVisibility can be put on the pose wrapper
 *
 */

using Oculus.Interaction;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class OVRHandMenu : MonoBehaviour
{
    [SerializeField]
    private OVRCameraRig cameraRig;

    [SerializeField]
    private Transform targetHandAnchor;
    [SerializeField]
    private Transform targetHandMiddle;
    [SerializeField]
    private Transform targetThumb;

    [SerializeField]
    private List<GameObject> HandMenu;

    [SerializeField]
    private List<Vector3> offsetFromHand = new List<Vector3>{ new Vector3(0.5f, 0.5f, 0.5f) };

    public float offset;

    private void Awake()
    {
        if (HandMenu.Count != offsetFromHand.Count)
        {
            Debug.LogError("Hand Button and offset do not match");
        }

        HandButtonVisibility(false);
    }

    public void HandButtonVisibility(bool state)
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
            //Hand Menu
            int index = 0;
            foreach (GameObject button in HandMenu)
            {
                Vector3 alignVec = Vector3.Normalize(targetHandMiddle.position - targetHandAnchor.position);
                Vector3 startPos = (targetHandMiddle.position + targetHandAnchor.position)/2;

                Vector3 handvec = Vector3.Normalize(Vector3.Cross( Vector3.Normalize(targetThumb.position - targetHandAnchor.position), alignVec));
                Vector3 offsetVec = Vector3.Cross(alignVec, handvec);
                
                button.transform.position = startPos + offsetVec * offset;
               
                //Quaternion temp = Quaternion.FromToRotation(Vector3.up, alignVec) ;
                //button.transform.rotation = Quaternion.LookRotation(handvec, Vector3.up);
                button.transform.rotation =  Quaternion.LookRotation(handvec, alignVec);
                
                index++;
            }
           
        }
        else
        {
            //Controller Menu
            int index = 0;
            foreach (GameObject button in HandMenu)
            {
                if (!button.activeSelf) button.SetActive(true);
                button.transform.position = targetHandAnchor.transform.position + offsetFromHand[index];
                Vector3 eyevec = button.transform.position - cameraRig.centerEyeAnchor.transform.position;
                button.transform.rotation = Quaternion.LookRotation(eyevec, Vector3.up);

                index++;
            }
           
        }

        // HandMenu.transform.position = targetHand.transform.position + offsetFromHand;
        // HandMenu.transform.rotation = Quaternion.LookRotation(HandMenu.transform.position - cameraRig.centerEyeAnchor.transform.position, Vector3.up);
        //
    }

}
    