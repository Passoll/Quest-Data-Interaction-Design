using System;
using System.Collections;
using System.Collections.Generic;
using DxR;
using DxRextention;
using Oculus.Interaction;
using UnityEngine;
using Oculus.Interaction.Input;

namespace DxRextention
{
    public class myOVRinterface : MonoBehaviour
    {
        [Header("OVR"), Space]
        public OVRManager MyManager;
        public Material SkyboxMat;
        public Camera centerEye;
        
        [Header("DxR"), Space]
        public DxRtransformcontroller myController;
        public Vis myDxR;
        public SliderManager myslider;
        
        [Header("Control"), Space]
        public OVRHM_RotateProxy rot;
        
        [Header("select"), Space]
        public Spatula spat;
        public MultiBox box;
        public List<RayInteractor> RayInteractors;
        public Transform Boxanchor;
        
        
        private List<GameObject> MultiboxList = new List<GameObject>();

        public enum Datatype
        {
            trajectory,
            scatter,
        }

        public Datatype currenttype = Datatype.scatter;

        private void Awake()
        {
            
            if (!spat.anchorpoint)
            {
                spat.anchorpoint = Boxanchor;
            }
            //Trick: set away and use the clone
            box.gameObject.SetActive(true);
            box.transform.position = new Vector3(10000, 10000, 10000);
            
            SetPassthroughState(false);
        }

        public void SetPassthroughState(bool state)
        {
            if (state)
            {
                centerEye.clearFlags = CameraClearFlags.SolidColor;
                centerEye.backgroundColor = new Color(0, 0, 0, 0);
                RenderSettings.skybox = null;
                MyManager.isInsightPassthroughEnabled = true;
            }
            else
            {
                centerEye.clearFlags = CameraClearFlags.Skybox;
                RenderSettings.skybox = SkyboxMat;
                MyManager.isInsightPassthroughEnabled = false;
            }

        }
        
        public void ChangePassthroughState()
        {
            if (!MyManager.isInsightPassthroughEnabled)
            {
                centerEye.clearFlags = CameraClearFlags.SolidColor;
                centerEye.backgroundColor = new Color(0, 0, 0, 0);
                RenderSettings.skybox = null;
            }
            else
            {
                centerEye.clearFlags = CameraClearFlags.Skybox;
                RenderSettings.skybox = SkyboxMat;
            }

            MyManager.isInsightPassthroughEnabled = !MyManager.isInsightPassthroughEnabled;
          
            Debug.Log("successfully change the passthrough state");
        }
    
        public void SwitchData()
        {
            // switch viz 
            myslider.SetSliderState(true);//FIx the XYZ missing
            
            ChangeDataViz();
            myController.resizeTo();
            myDxR.UpdateVisSpecsFromTextSpecs();
            myController.resizeBack();//Test only to avoid size change
            
            myslider.InitializeSlider();
            Debug.Log("change the data");
        }
        
        private void ChangeDataViz()
        {
            if (currenttype == Datatype.trajectory) currenttype = Datatype.scatter;
            else currenttype = Datatype.trajectory;
            
            switch (currenttype)
            {
                case Datatype.scatter:
                    myDxR.visSpecsURL = "scatterplot3D.json";
                    myDxR.Renderline = false;
                    break;
                case Datatype.trajectory:
                    myDxR.visSpecsURL = "t_3dtrajectory_reduce.json";
                    myDxR.Renderline = true;
                    //myDxR
                    break;
            }
        }


        //-----------------------------------Transform
        public void ToggleTransform()
        {
            myController.Toggle_transform();
            myslider.SetSliderState(!myController.Getshowstate());
        }
        
        public void ToggleRotateProxy()
        {
            rot.ToggleVisibility();
        }

        //-----------------------------------Selection
        public void EnableMultibox()
        {
            if (MultiboxList.Count == 0)
            {
                ResetSelectMode();
            }
            GameObject newbox = Instantiate(box.gameObject, Boxanchor.position, Boxanchor.rotation);
            MultiboxList.Add(newbox);
        }
        
        public void EnableSpatMode()
        {
            ResetSelectMode();
            spat.SetVisibility(true);
        }

        public void EnableRayMode()
        {
            ResetSelectMode();
            foreach (var Ray in RayInteractors)
            {
                Ray.gameObject.SetActive(true);
            }
        }

        public void ResetMode()
        {
            ResetSelectMode();
        }
        
        private void ResetSelectMode()
        {
            //ray
            foreach (var Ray in RayInteractors)
            {
                Ray.gameObject.SetActive(false);
            }
            //spat
            if (spat.gameObject.activeSelf)
            {
                spat.SetVisibility(false);
            }
            //multibox
            if (MultiboxList.Count > 0)
            {
                foreach(var box in MultiboxList)
                {
                    Destroy(box);
                }
                MultiboxList.Clear();
            }
            //slider
            myslider.resetslider(); //reset all the mark

        }


    }

}
