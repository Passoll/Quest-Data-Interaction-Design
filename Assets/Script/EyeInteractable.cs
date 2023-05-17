using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
//[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    
    //[field: SerializeField]
    public bool IsHovered;
    
    [field: SerializeField]
    public bool IsSelected { get; set; }

    [SerializeField] private UnityEvent<GameObject> OnObjectHover;
    [SerializeField] private UnityEvent<GameObject> OnObjectSelected;
    [SerializeField] private Material OnHoverActiveMat ;
    [SerializeField] private Material OnIdleMat;
    [SerializeField] private Material OnSelectedMat;

    private MeshRenderer _meshRenderer;
    private Transform _originalAnchor;
    private TextMeshPro statusText;
        
    private void Start()
    {
        if (OnHoverActiveMat == null && OnIdleMat == null )
        {
            Material acMaterial = Resources.Load("Materials/acMat", typeof(Material)) as Material;
            Material inMaterial = Resources.Load("Materials/inMat", typeof(Material)) as Material;
            OnHoverActiveMat = acMaterial;
            OnIdleMat = inMaterial;
        }
        
        _meshRenderer = GetComponent<MeshRenderer>();
        statusText = GetComponentInChildren<TextMeshPro>();
        _originalAnchor = transform.parent;

    }

    public void Hover(bool state)
    {
        IsHovered = state;
    }
    public void Select(bool state, Transform anchor = null)
    {
        IsSelected = state;
        if(anchor) transform.SetParent(anchor);
        if(!IsSelected) transform.SetParent(_originalAnchor);
    }
    
    
    private void Update()
    {
        // if (IsSelected)
        // {
        //     OnObjectSelected?.Invoke(gameObject);
        //     _meshRenderer.material = OnSelectedMat;
        //     statusText.text = "Here Select";
        // }
        if (IsHovered)
        {
            _meshRenderer.material = OnHoverActiveMat;
            OnObjectHover?.Invoke(gameObject);
            statusText.text = "Here Hover";
        }
        // if (!IsSelected && !IsSelected)
        // {
        //     statusText.text = "idle";
        //     _meshRenderer.material = OnIdleMat;
        // }
    }
}
