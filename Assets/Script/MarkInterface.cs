using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;


namespace DxRextention
{
    public class MarkInterface : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IInteractableView))]
        private MonoBehaviour _interactableView;

        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private Color _normalColor = Color.red;
        [SerializeField]
        private Color _hoverColor = Color.blue;
        [SerializeField]
        private Color _selectColor = Color.green;
        [SerializeField]
        private Color _disabledColor = Color.black;
        
        public Color NormalColor
        {
            get
            {
                return _normalColor;
            }
            set
            {
                _normalColor = value;
            }
        }
        public Color HoverColor
        {
            get
            {
                return _hoverColor;
            }
            set
            {
                _hoverColor = value;
            }
        }
        public Color SelectColor
        {
            get
            {
                return _selectColor;
            }
            set
            {
                _selectColor = value;
            }
        }
        public Color DisabledColor
        {
            get
            {
                return _disabledColor;
            }
            set
            {
                _disabledColor = value;
            }
        }

        private IInteractableView InteractableView;
        private Material _material;
        protected bool _started = false;
        
        private RayInteractable _rayInteractable;
        private bool selected = false;

        private void Awake()
        {
            this.BeginStart(ref _started);
            this.AssertField(_renderer, nameof(_renderer));
            
            _rayInteractable = gameObject.GetComponent<RayInteractable>();
            InteractableView = _interactableView as IInteractableView;
            _material = _renderer.material;
            UpdateVisual();
            this.EndStart(ref _started);
        }
        
        protected virtual void OnEnable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged += UpdateVisualState;
                UpdateVisual();
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged -= UpdateVisualState;
            }
        }
        
        public void SetNormalColor(Color color)
        {
            _normalColor = color;
            UpdateVisual();
        }

        public void selectBox()
        {
            selected = true;
            _material.color = _selectColor;
        }

        public void unselectBox()
        {
            selected = false;
            _material.color = _normalColor;
            
        }

        private void UpdateVisual()
        {
            switch (InteractableView.State)
            {
                case InteractableState.Normal:
                    if(!selected)_material.color = _normalColor;
                    break;
                case InteractableState.Hover:
                    if(!selected)_material.color = _hoverColor;
                    break;
                case InteractableState.Select:
                    _material.color = _selectColor;
                    selected = true;
                    break;
                case InteractableState.Disabled:
                    if(!selected)_material.color = _disabledColor;
                    break;
            }
        }
        
        private void UpdateVisualState(InteractableStateChangeArgs args) => UpdateVisual();
        
        
    }
}
