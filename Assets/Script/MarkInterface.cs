using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;


namespace DxRextention
{
    public class MarkInterface : MonoBehaviour
    {
        private RayInteractable _rayInteractable;

        private void Awake()
        {
            _rayInteractable = gameObject.GetComponent<RayInteractable>();
        }

        public void selectBox()
        {
            _rayInteractable.enabled = false;
        }

        public void unselectBox()
        {
            _rayInteractable.enabled = true;
        }
    }
}
