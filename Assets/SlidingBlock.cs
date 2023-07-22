using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/* Written by: Ayse Zhong, 2023.7
 *
 * @SlidingBlock
 * brief: This is for single sliding block, contain the interatable, call the menu and interaction in the range
 * Create a range and pass it to the slidermangager, the range is a proportion to adapt the scale and move; 
 *
 * 1.When select : Update the range in manager, but do not need to add to instance
 * 2.When release : add the the instance.
 *
 * 
 * @Method
 * - OnSliderSelect()
 * 1. Create an instance when the sliding box is select
 * 
 * - ValidateSlider()
 * 1. if it move out the range, keep the new instance, else delete it
 * 
 * - onMove()
 * 1.Update the range
 * 
 */

namespace DxRextention
{
    public class SlidingBlock : MonoBehaviour
    {
        private bool Valid = false;
        private bool Selected = false;
        private Transform anchor; // calculate the pos
        public Transform startanchor; // the positon keep stay
        
        public GameObject Plane;
        public GameObject Menu;
        public SliderManager _sliderManager;
        public DxRtransformcontroller _DxRtransformcontroller;

        [Range(0, 1), Tooltip("valid threshold")]
        public float threshold;

        public enum TAxis
        {
            X,
            Y,
            Z
        }

        [Tooltip("Axis")] public TAxis BoxAxis;

        protected bool _started = false;
        

        [SerializeField, Tooltip("The range plane state")]
        private bool _rangeState = false;
        private float _rangeMark;// the proportion of the axis
        
        private float _tempAxisLength;
        private GameObject tempslider;

        public bool RangeState
        {
            get { return _rangeState; }
            set { _rangeState = value; }
        }

        void Start()
        {
            this.BeginStart(ref _started);
            Menu.SetActive(false);
            Plane.SetActive(false);
            anchor = _sliderManager.anchor;
            
            transform.position = startanchor.position;
            this.EndStart(ref _started);
        }
        
        private void LateUpdate()
        {
            FixAxis();
            if (Selected)
            {
                //Update Position
                float axis_cut = Vector3.Distance(transform.position, anchor.position);
                _rangeMark = axis_cut / _tempAxisLength;
                
                //Update Range
                SliderRange range = new SliderRange(_rangeMark, _rangeState);
                _sliderManager.UpdateCurrentRange(gameObject, range, BoxAxis);
            }
        }

        // Call when the selection event
        public void OnSliderSelect()
        {
            Selected = true;
            switch (BoxAxis)
            {
                case TAxis.X:
                    _tempAxisLength = _DxRtransformcontroller.GetBoundSize().x;
                    break;
                case TAxis.Y:
                    _tempAxisLength = _DxRtransformcontroller.GetBoundSize().y;
                    break;
                case TAxis.Z:
                    _tempAxisLength = _DxRtransformcontroller.GetBoundSize().z;
                    break;
            }
         
            if (Valid == false)
            {
                Menu.SetActive(true);
                Plane.SetActive(true);
                tempslider = Instantiate(gameObject, _sliderManager.transform);
            }
        }

        //Call each time release the Selection
        //If valid then keep, if no, then disappear
        public void ValidateSlider()
        {
            Selected = false;
            float axis_cut = Vector3.Distance(transform.position, anchor.position);
            if (Valid == false)
            {
                // ---- Valid creation more than boundary and less than max count 
                SliderRange range = new SliderRange(_rangeMark, _rangeState);
                if (axis_cut / _tempAxisLength > threshold && _sliderManager.Addslider(BoxAxis, gameObject, range))
                {
                    Valid = true;
                }
                else
                {
                    //----- Invalid creation
                    Destroy(tempslider);
                    tempslider = null;
                    _sliderManager.RemoveSliderCheck(BoxAxis);//After destroy we need to kicknull
                    
                    Valid = false;
                    Menu.SetActive(false);
                    Plane.SetActive(false);
                    transform.position = startanchor.position;
                }
            }
            else
            {
                // ----- delete the boundary
                if (axis_cut / _tempAxisLength < threshold)
                {
                    Valid = false;
                    Destroy(gameObject);
                    _sliderManager.RemoveSliderCheck(BoxAxis);
                }
            }

        }
        

        //brief: Invert the range, call by a button
        public void InvertRange()
        {
            //TOD1
            _rangeState = !_rangeState;
            SliderRange range = new SliderRange(_rangeMark, _rangeState);
            _sliderManager.UpdateCurrentRange(gameObject, range, BoxAxis);
 
        }
        

        private void FixAxis()
        {
            Vector3 anchor = this.anchor.position;
            switch (BoxAxis)
            {
                case TAxis.X:
                    float x = Mathf.Min(Mathf.Max(gameObject.transform.position.x, anchor.x),
                        anchor.x + _DxRtransformcontroller.GetBoundSize().x);
                    gameObject.transform.position = new Vector3(x, anchor.y, anchor.z);
                    gameObject.transform.rotation = Quaternion.identity;
                    break;
                case TAxis.Y:
                    float y = Mathf.Min(Mathf.Max(gameObject.transform.position.y, anchor.y),
                        anchor.y + _DxRtransformcontroller.GetBoundSize().y);
                    gameObject.transform.position = new Vector3(anchor.x, y, anchor.z);
                    gameObject.transform.rotation = Quaternion.identity;
                    break;
                case TAxis.Z:
                    float z = Mathf.Min(Mathf.Max(gameObject.transform.position.z, anchor.z),
                        anchor.z + _DxRtransformcontroller.GetBoundSize().z);
                    gameObject.transform.position = new Vector3(anchor.x, anchor.y, z);
                    gameObject.transform.rotation = Quaternion.identity;
                    break;
            }
        }
    }
}


