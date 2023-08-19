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

public class OVRHandMenuDispacher : MonoBehaviour
{
    [SerializeField]
    private OVRCameraRig cameraRig;
    [SerializeField]
    private Transform targetControllerAnchor;
    [SerializeField]
    private Transform targetHandAnchor;
    [SerializeField]
    private Transform targetHandMiddle;
    [SerializeField]
    private Transform targetThumb;

    [Space]
    [SerializeField , Interface(typeof(IOVRHandMenu))]
    private MonoBehaviour HandMenuButton;
    [SerializeField , Interface(typeof(IOVRHandMenu))]
    private MonoBehaviour HandCube;
    
    private List<IOVRHandMenu> m_OVRHandMenu = new List<IOVRHandMenu>(2);
    private CustomHandPlane handplane = new CustomHandPlane();

    private void Awake()
    {
        IOVRHandMenu handbutton = HandMenuButton as IOVRHandMenu;
        IOVRHandMenu handproxy = HandCube as IOVRHandMenu;
        m_OVRHandMenu.Add(handbutton);
        m_OVRHandMenu.Add(handproxy);
    }
    
    private void Update()
    {
        if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands))
        {
            handplane.startPos = (targetHandMiddle.position + targetHandAnchor.position)/2;
            handplane.alignVec = Vector3.Normalize(targetHandMiddle.position - targetHandAnchor.position);
            handplane.handVec= Vector3.Normalize(Vector3.Cross( Vector3.Normalize(targetThumb.position - targetHandAnchor.position), handplane.alignVec));
            handplane.offsetVec = Vector3.Normalize(Vector3.Cross(handplane.alignVec, handplane.handVec));
            foreach (var hm in m_OVRHandMenu)
            {
                hm.SetPlane(handplane);
                hm.UpdatePosHand();
            }

        }
        else if(OVRInput.IsControllerConnected(OVRInput.Controller.Touch))
        {
            //Controller Menu
            handplane.startPos = targetControllerAnchor.transform.position;
            handplane.handVec = Vector3.Normalize(targetControllerAnchor.transform.position - cameraRig.centerEyeAnchor.transform.position);
            handplane.alignVec = new Vector3(0, 1, 0);
            handplane.offsetVec = Vector3.Normalize(Vector3.Cross(handplane.alignVec, handplane.handVec));
            foreach (var hm in m_OVRHandMenu)
            {
                hm.SetPlane(handplane);
                hm.UpdatePosHand();
            }
           
        }

    }
}


public class CustomHandPlane
{
    public Vector3 offsetVec = Vector3.zero; // palm to thumb
    public Vector3 alignVec = Vector3.zero; // palm to mid finger
    public Vector3 handVec = Vector3.zero; // hand to eye
    public Vector3 startPos = Vector3.zero;

}
