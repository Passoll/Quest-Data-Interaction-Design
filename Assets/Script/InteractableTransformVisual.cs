using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class InteractableTransformVisual : MonoBehaviour
 {
        [SerializeField, Interface(typeof(IInteractableView))]
        private MonoBehaviour _interactableView;
        private IInteractableView InteractableView { get; set; }

        [SerializeField] private Transform _scaleTransform;
        
        [SerializeField,Optional,Interface(typeof(IinteractableAnimationValue))] private MonoBehaviour _animationObj;
        private IinteractableAnimationValue AnimationObj { get; set; }
        
        [Serializable]
        public class ScaleState
        {
            public Vector3 scale = Vector3.one;
            public Vector3 position = Vector3.one;
            public float animatevalue;
            
            public AnimationCurve TransCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            public float AnimateTime = 0.1f;
        }

        [SerializeField]
        private ScaleState _normalState = new ScaleState() {  };
        [SerializeField]
        private ScaleState _hoverState = new ScaleState() {  };
        [SerializeField]
        private ScaleState _selectState = new ScaleState() {  };
        [SerializeField]
        private ScaleState _disabledState = new ScaleState() { };

        private Transform _currentTransform;
        private float _currentvalue = 0;
        
        private ScaleState _target;
        private Coroutine _routine = null;
        private static readonly YieldInstruction _waiter = new WaitForEndOfFrame();

        protected bool _started = false;

        protected virtual void Awake()
        {
            InteractableView = _interactableView as IInteractableView;
            AnimationObj = _animationObj as IinteractableAnimationValue;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);

            this.AssertField(InteractableView, nameof(InteractableView));
            _currentTransform = _scaleTransform;
            
            if (AnimationObj != null)
            {
                _currentvalue = AnimationObj.Getanimationvalue();
            }
            
            
            UpdateVisual();
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                UpdateVisual();
                InteractableView.WhenStateChanged += UpdateVisualState;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged -= UpdateVisualState;
            }
        }

        private void UpdateVisualState(InteractableStateChangeArgs args)
        {
            UpdateVisual();
        }

        protected virtual void UpdateVisual()
        {
            ScaleState target = TransformForState(InteractableView.State);
            if (target != _target)
            {
                _target = target;
                CancelRoutine();
                _routine = StartCoroutine(ChangeTransform(target));
            }
        }

        private ScaleState TransformForState(InteractableState state)
        {
            switch (state)
            {
                case InteractableState.Select:
                    return _selectState;
                case InteractableState.Hover:
                    return _hoverState;
                case InteractableState.Normal:
                    return _normalState;
                case InteractableState.Disabled:
                    return _disabledState;
                default:
                    return _normalState;
            }
        }

        private IEnumerator ChangeTransform(ScaleState targetState)
        {
            Transform startTransform = _currentTransform;
            float startvalue = _currentvalue;
            float timer = 0f;
            do
            {
                timer += Time.deltaTime;
                float normalizedTimer = Mathf.Clamp01(timer / targetState.AnimateTime);
                float t = targetState.TransCurve.Evaluate(normalizedTimer);
                // calculate the middle value
                Vector3 position = Vector3.Lerp(startTransform.localPosition, targetState.position, t);
                Vector3 scale = Vector3.Lerp(startTransform.localScale, targetState.scale, t);
                float value = Mathf.Lerp(startvalue, targetState.animatevalue, t); 
                SetTransform(position, scale, value);

                yield return _waiter;
            }
            while (timer <= targetState.AnimateTime);
        }

        private void SetTransform(Vector3 location, Vector3 scale, float value)
        {
            _currentTransform.localPosition = location;
            _currentTransform.localScale = scale;
            _currentvalue = value;
            
            //Set the true obj
            _scaleTransform.localPosition = _currentTransform.localPosition;
            _scaleTransform.localScale = _currentTransform.localScale;
            if (AnimationObj != null)
            {
                AnimationObj.Setanimationvalue(_currentvalue);
            }
            
        }
        private void CancelRoutine()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
        }
        
    }