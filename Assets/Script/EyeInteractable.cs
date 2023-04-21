using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    [SerializeField] private UnityEvent<GameObject> OnObjectHover;
    [SerializeField] private Material OnHoverActiveMat;
    [SerializeField] private Material OnHoverInActiveMat;

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        if (IsHovered)
        {
            _meshRenderer.material = OnHoverActiveMat;
            OnObjectHover?.Invoke(gameObject);
        }
        else
        {
            _meshRenderer.material = OnHoverInActiveMat;
        }
    }
}
