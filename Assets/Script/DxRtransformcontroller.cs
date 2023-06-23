using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using Oculus.Interaction.Input;

public class DxRtransformcontroller : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public Transform boxanchor;
    [SerializeField, Interface(typeof(IInteractableView)), Tooltip("Anchor interactable View")]
    private MonoBehaviour _interactableView;
    private IInteractableView InteractableView;
    [SerializeField]
    public Transform boxdy;
    [SerializeField, Interface(typeof(IInteractableView)), Tooltip("Anchor interactable View")]
    private MonoBehaviour _dyinteractableView;
    private IInteractableView dyInteractableView;
    [SerializeField]
    public Transform DxRview;
    

    private float _diagonlenth;
    protected bool _started;

    private Vector3 _relativepos;
    //if all the box move
    private bool _anchorSelect;
    private bool _dySelect;
    
    private void Awake()
    {
        //DistanceInteractor = _distanceInteractor as IDistanceInteractor;
        InteractableView = _interactableView as IInteractableView;
        dyInteractableView = _dyinteractableView as IInteractableView;
    }
    
    void Start()
    {
        this.BeginStart(ref _started);
        _diagonlenth = (boxanchor.position - boxdy.position).magnitude;
        _relativepos =  boxdy.position - boxanchor.position;
        this.EndStart(ref _started);
    }

    // Update is called once per frame
   
    
    protected virtual void OnEnable()
    {
        if (_started)
        {
            InteractableView.WhenStateChanged += AnchorUpdateMoveState;
            dyInteractableView.WhenStateChanged += DynaUpdateMoveState;
        }
    }

    protected virtual void OnDisable()
    {
        if (_started)
        {
            InteractableView.WhenStateChanged -= AnchorUpdateMoveState;
            dyInteractableView.WhenStateChanged -= DynaUpdateMoveState;
        }
    }
    private void AnchorUpdateMoveState(InteractableStateChangeArgs args)
    {
        switch (InteractableView.State)
        {
            case InteractableState.Normal:
                _anchorSelect = false;
                break;
            case InteractableState.Hover:
                break;
            case InteractableState.Select:
                _anchorSelect = true;
                break;
            case InteractableState.Disabled:
                break;
        }
    }
    private void DynaUpdateMoveState(InteractableStateChangeArgs args)
    {
        switch (dyInteractableView.State)
        {
            case InteractableState.Normal:
                _dySelect = false;
                break;
            case InteractableState.Hover:
                break;
            case InteractableState.Select:
                _dySelect = true;
                break;
            case InteractableState.Disabled:
                break;
        }
    }
    
    void LateUpdate()
    {
        if (_anchorSelect && !_dySelect )
        {
            UpdateOnlyMove();
        }
        else if (_anchorSelect && _dySelect)
        {
            UpdateDynamic();
        }
        else if ( !_anchorSelect && _dySelect)
        {
            UpdateScale();
        }

    }
    
    private void UpdateScale()
    {
        _relativepos =  boxdy.position - boxanchor.position;
        float scale = _relativepos.magnitude / _diagonlenth;
        DxRview.localScale = new Vector3(scale, scale, scale);
        
    }

    private void UpdateOnlyMove()
    {
        DxRview.position = boxanchor.position;
        boxdy.position = boxanchor.position + _relativepos;
    }

    private void UpdateDynamic()
    {
        DxRview.position = boxanchor.position;
        float scale = (boxanchor.position - boxdy.position).magnitude / _diagonlenth;
        DxRview.localScale = new Vector3(scale, scale, scale);
        _relativepos =  boxdy.position - boxanchor.position;
    }
    
    // private void HandleStateChanged(InteractorStateChangeArgs args)
    // {
    //     switch (args.NewState)
    //     {
    //         //clear
    //         case InteractorState.Normal:
    //             if (args.PreviousState != InteractorState.Disabled)
    //             {
    //                 //InteractableUnset();
    //             }
    //
    //             break;
    //         case InteractorState.Hover:
    //             if (args.PreviousState == InteractorState.Normal)
    //             {
    //                 //InteractableSet(DistanceInteractor.DistanceInteractable);
    //             }
    //             break;
    //     }
    //
    //     if (args.NewState == InteractorState.Select
    //         || args.NewState == InteractorState.Disabled
    //         || args.PreviousState == InteractorState.Disabled)
    //     {
    //         _shouldmove = false;
    //     }
    //     else if (args.NewState == InteractorState.Hover)
    //     {
    //         _shouldmove = true;
    //     }
    //     else if (args.NewState == InteractorState.Normal)
    //     {
    //         //_shouldDrawLine = _visibleDuringNormal;
    //     }
    // }


   
}
