/*
 * Written by: Ayse Zhong, 2023.7
 *
 * @DxRtransformcontroller
 * brief: This is the transform controller to fit the DxR view. It use the bool to control the update when the controller is selected
 * Support both hand grab and distance grab.
 * 
 * @Member
 * - Anchor Box: when selected solely, only position will change;
 * - Diagonal Box(Dynamic box): when selected solely, only size will change;
 * 
 * @Method
 * - SetAnchor/DySelect() 
 * 1. Set the toggle to open the update portal
 * 
 * - Toggle_transform()
 * 1. Set the transform box visability
 * 
 *
 */

using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using Oculus.Interaction.Input;


namespace DxRextention
{
    public class DxRtransformcontroller : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] public Transform boxanchor;
        [SerializeField, Interface(typeof(IInteractableView)), Tooltip("Anchor interactable View Distance Grab")]
        private MonoBehaviour _interactableView_Dis;
        [SerializeField, Interface(typeof(IInteractableView)), Tooltip("Anchor interactable View Grab")]
        private MonoBehaviour _interactableView_Grab;
        private IInteractableView InteractableView_d;
        private IInteractableView InteractableView_g;
        
        [Space]
        [SerializeField] public Transform boxdy;
        [SerializeField, Interface(typeof(IInteractableView)), Tooltip("Dynamic interactable View Distance Grab")]
        private MonoBehaviour _dyinteractableView_Dis;
        [SerializeField, Interface(typeof(IInteractableView)), Tooltip("Dynamic interactable View Grab")]
        private MonoBehaviour _dyinteractableView_Grab;
        private IInteractableView dyInteractableView_d;
        private IInteractableView dyInteractableView_g;
        
        [Space]
        [SerializeField] public Transform DxRview;
        
        private bool Show_controll_toggle = true; // show the controll box
        private float _diagonlenth;
        protected bool _started;
        private Vector3 _relativepos;
        //if all the box move
        private bool _anchorSelect;
        private bool _dySelect;
        private float xy_proportion;

        private Vector3 BoundSize;
        private Vector3 startsize;

        private float resizeconstant;


        //Only the x difference of the box matters.

        private void Awake()
        {
            //DistanceInteractor = _distanceInteractor as IDistanceInteractor;
            InteractableView_d = _interactableView_Dis as IInteractableView;
            InteractableView_g = _interactableView_Grab as IInteractableView;
            dyInteractableView_d = _dyinteractableView_Dis as IInteractableView;
            dyInteractableView_g = _dyinteractableView_Grab as IInteractableView;
            
        }

        void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(_interactableView_Dis, nameof(_interactableView_Dis));
            this.AssertField(_interactableView_Grab, nameof(_interactableView_Grab));
            this.AssertField(_dyinteractableView_Dis, nameof(_dyinteractableView_Dis));
            this.AssertField(_dyinteractableView_Grab, nameof(_dyinteractableView_Grab));

            PrepareDxRTransform();
            this.EndStart(ref _started);
            SetEnableTransform(true);

            Toggle_transform();//close the block at the start
        }

        private void PrepareDxRTransform()
        {
            // Get the bounding box of DxRview
            Bounds bound = GetChildrenbound(DxRview);
            xy_proportion = bound.extents.y / bound.extents.x;
            
            // Set the proportion of boxdy
            float templenth = Fixtransform();
            // then reset the DxRsize
            float matchlength = bound.max.x - bound.min.x;

            resizeconstant = templenth / matchlength;
            
            // set the parent to avoid conflict
            DxRview.parent.transform.localScale = DxRview.parent.transform.localScale * resizeconstant;
            _diagonlenth = (boxanchor.position - boxdy.position).magnitude;
            _relativepos = boxdy.position - boxanchor.position;
            
            //update the Boundsize
            bound = GetChildrenbound(DxRview);
            BoundSize = bound.extents * 2;
            startsize = BoundSize;
        }
        
        //Test Only, this function is for the size match for DxR to avoid switching data's problem
        public void resizeTo()
        {
            DxRview.parent.transform.localScale = DxRview.parent.transform.localScale / resizeconstant;
        }
        public void resizeBack()
        {
            DxRview.parent.transform.localScale = DxRview.parent.transform.localScale * resizeconstant;
        }

        public void SetAnchorSelect(bool state)
        {
            _anchorSelect = state;
        }

        public void SetDySelect(bool state)
        {
            _dySelect = state;
        }

        public Vector3 GetBoundSize()
        {
            return BoundSize;
        }

        public bool Getshowstate()
        {
            return Show_controll_toggle;
        }
        

        protected void SetEnableTransform(bool state)
        {
            // When disable. minus the function
            // if (_started)
            // {
            //     InteractableView_d.WhenStateChanged += AnchorUpdateMoveState;
            //     InteractableView_g.WhenStateChanged += AnchorUpdateMoveState;
            //     dyInteractableView_d.WhenStateChanged += DynaUpdateMoveState;
            //     dyInteractableView_g.WhenStateChanged += DynaUpdateMoveState;
            // }
            boxanchor.gameObject.SetActive(state);
            boxdy.gameObject.SetActive(state);
        }
        

        public void Toggle_transform()
        {
            if (Show_controll_toggle)
            {
                SetEnableTransform(false);
                Show_controll_toggle = false;
            }
            else
            {
                SetEnableTransform(true);
                Show_controll_toggle = true;
            }

        }

        void LateUpdate()
        {
            if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands))
            {
                if (_anchorSelect && !_dySelect)
                {
                    Fixtransform();
                    UpdateOnlyMove();
                }
                else if (_anchorSelect && _dySelect)
                {
                    Fixtransform();
                    UpdateDynamic();

                }
                else if (!_anchorSelect && _dySelect)
                {
                    Fixtransform();
                    UpdateScale();

                }
            }
            else
            {
                Fixtransform();
                UpdateDynamic();
            }

        }

        private void UpdateScale()
        {
            _relativepos = boxdy.position - boxanchor.position;
            float scale = _relativepos.magnitude / _diagonlenth;
            DxRview.localScale = new Vector3(scale, scale, scale);
            //Bounds bound = GetChildrenbound(DxRview);
            BoundSize = scale * startsize;

        }

        private void UpdateOnlyMove()
        {
            DxRview.position = boxanchor.position;
            boxdy.position = boxanchor.position + _relativepos;
        }

        private void UpdateDynamic()
        {
            DxRview.position = boxanchor.position;
            _relativepos = boxdy.position - boxanchor.position;
            float scale = _relativepos.magnitude / _diagonlenth;
            DxRview.localScale = new Vector3(scale, scale, scale);
            BoundSize = scale * startsize;
        }
        
        //return the x difference
        private float Fixtransform()
        {
            boxanchor.rotation = new Quaternion(0, 0, 0, 0);
            boxdy.rotation = new Quaternion(0, 0, 0, 0);
            float templenth = Mathf.Abs(boxanchor.localPosition.x - boxdy.localPosition.x);
            boxdy.localPosition = boxanchor.localPosition + new Vector3(templenth, templenth * xy_proportion, 0);
            return templenth;
        }

        private Bounds GetChildrenbound(Transform parent)
        {
            Bounds bound = new Bounds(parent.position, Vector3.zero);
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                bound.Encapsulate(renderer.bounds);
            }

            return bound;
        }
        
        // Already Implement in wrapper
        //--------------------------------------
        // private void AnchorUpdateMoveState(InteractableStateChangeArgs args)
        // {
        //     switch (args.NewState)
        //     {
        //         case InteractableState.Normal:
        //             _anchorSelect = false;
        //             break;
        //         case InteractableState.Hover:
        //             break;
        //         case InteractableState.Select:
        //             _anchorSelect = true;
        //             break;
        //         case InteractableState.Disabled:
        //             break;
        //     }
        // }
        //
        //
        // private void DynaUpdateMoveState(InteractableStateChangeArgs args)
        // {
        //     switch (args.NewState)
        //     {
        //         case InteractableState.Normal:
        //             _dySelect = false;
        //             break;
        //         case InteractableState.Hover:
        //             break;
        //         case InteractableState.Select:
        //             _dySelect = true;
        //             break;
        //         case InteractableState.Disabled:
        //             break;
        //     }
        // }
        //
        
        
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
}