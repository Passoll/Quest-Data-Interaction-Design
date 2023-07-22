using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;


public class SlidingDispatcher : Grabbable
{
    public Vector3 alongaxis;

    public List<GameObject> planeInstance;
    [SerializeField] private int MaxPlaneNumber;
    
    public GameObject sliderblock;
    
    public override void ProcessPointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                //EndTransform();
                break;
            case PointerEventType.Unselect:
                //EndTransform();
                break;
            case PointerEventType.Cancel:
                //EndTransform();
                break;
        }

        base.ProcessPointerEvent(evt);

        switch (evt.Type)
        {
            case PointerEventType.Select:
                //BeginTransform();
                break;
            case PointerEventType.Unselect:
                //BeginTransform();
                break;
            case PointerEventType.Move:
                //UpdateTransform();
                break;
        }
    }

    public void ProduceGrabPlane()
    {
        GameObject newObj = Instantiate(sliderblock, gameObject.transform.position, Quaternion.identity);
        Grabbable grabbable2 = newObj.GetComponent<Grabbable>();
        HandGrabInteractable grabbable = newObj.GetComponent<HandGrabInteractable>();
        if (grabbable == null)
        {
            grabbable = newObj.AddComponent<HandGrabInteractable>();
        }

        //grabbable.;

    }
    
    
    
    

}
