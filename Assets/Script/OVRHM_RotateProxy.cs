using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRHM_RotateProxy : MonoBehaviour, IOVRHandMenu
{
    
    public Vector3 offsetFromHand;
    protected CustomHandPlane m_plane;

    [Space]
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public Transform DxRanchorpoint;
    public bool enblerotLine;
    public int lengthOfLineRenderer = 2;
    private LineRenderer m_lineRenderer;

    private void Awake()
    {
        
        if (m_lineRenderer == null)
        {
            m_lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        m_lineRenderer.widthMultiplier = 0.01f;
        m_lineRenderer.positionCount = lengthOfLineRenderer;
        
        Gradient gradient = new Gradient();
        float alpha = 1.0f;
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        m_lineRenderer.colorGradient = gradient;

    }
    
    void Update()
    {
        if (enblerotLine)
        {
            m_lineRenderer.SetPosition(0, transform.position);        
            m_lineRenderer.SetPosition(1, DxRanchorpoint.position);
        }
        
    }

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
        }
    }
}
