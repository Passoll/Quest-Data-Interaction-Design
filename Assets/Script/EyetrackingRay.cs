using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyetrackingRay : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1.0f;

    [SerializeField] private float rayWidth = 0.01f;

    [SerializeField] private LayerMask layersToInclude;

    [SerializeField] private Color rayColorDefaultState = Color.yellow;

    [SerializeField] private Color rayColorHoverState = Color.red;

    private LineRenderer _lineRenderer;
    private List<EyeInteractable> _eyeInteractables = new List<EyeInteractable>();

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay()
    {
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = rayWidth;
        _lineRenderer.endWidth = rayWidth;
        _lineRenderer.startColor = rayColorDefaultState;
        _lineRenderer.endColor = rayColorHoverState;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1,
            new Vector3(transform.position.x, transform.position.y, transform.position.z + rayDistance));

    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;
        if (Physics.Raycast(transform.position, rayCastDirection, out hit, Mathf.Infinity, layersToInclude))
        {
            Unselect();
            _lineRenderer.startColor = rayColorHoverState;
            _lineRenderer.endColor = rayColorHoverState;
            EyeInteractable eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
            eyeInteractable.IsHovered = true;
            _eyeInteractables.Add(eyeInteractable);
        }
        else
        {
            _lineRenderer.startColor = rayColorDefaultState;
            _lineRenderer.endColor = rayColorDefaultState;
            Unselect(true);
        }

        //throw new NotImplementedException();
    }

    void Unselect(bool clear = false)
    {
        foreach (var interable in _eyeInteractables)
        {
            interable.IsHovered = false;
        }

        if (clear)
        {
            _eyeInteractables.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

