using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRHM_MenuButton : MonoBehaviour, IOVRHandMenu
{
    public Vector3 offsetFromHand;
    protected CustomHandPlane m_plane;

    public void SetPlane(CustomHandPlane plane)
    {
        m_plane = plane;
    }

    public void SetVisibility(bool state)
    {
        gameObject.SetActive(state);
    }
    
    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void UpdatePosHand()
    {
        if (gameObject.activeSelf)
        {
            transform.position = m_plane.startPos + m_plane.offsetVec * offsetFromHand.x 
                                                  + m_plane.alignVec * offsetFromHand.y 
                                                  + m_plane.handVec * offsetFromHand.z;
            transform.rotation = Quaternion.LookRotation(m_plane.handVec, m_plane.alignVec);
        }
       
    }
    
    
    
    
}
