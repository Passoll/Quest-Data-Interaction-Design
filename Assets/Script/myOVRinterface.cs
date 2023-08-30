using System;
using System.Collections;
using System.Collections.Generic;
using DxR;
using DxRextention;
using UnityEngine;
using Oculus.Interaction.Input;

namespace DxRextention
{
    public class myOVRinterface : MonoBehaviour
    {
        
        public OVRManager MyManager;
        public DxRtransformcontroller myController;
        public Vis myDxR;
        public Material SkyboxMat;
        public Spatula spat;
        public SliderManager myslider;
        public OVRHM_RotateProxy rot;
        
        [Space]
        public MultiBox box;
        private List<GameObject> MultiboxList = new List<GameObject>();
        
        public Transform Boxanchor;


        public enum Datatype
        {
            trajectory,
            scatter,
        }

        public Datatype currenttype = Datatype.scatter;

        private void Awake()
        {
            //Trick: set away and use the clone
            box.gameObject.SetActive(true);
            box.transform.position = new Vector3(10000, 10000, 10000);
            
        }

        public void ChangePassthroughState()
        {
            MyManager.isInsightPassthroughEnabled = !MyManager.isInsightPassthroughEnabled;
            if(MyManager.isInsightPassthroughEnabled) RenderSettings.skybox = null;
            else RenderSettings.skybox = SkyboxMat;
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

        public void ToggleTransform()
        {
            myController.Toggle_transform();
            myslider.SetSliderState(!myController.Getshowstate());
        }

        public void ToggleSpatMode()
        {
            spat.ToggleVisibility();
            myslider.resetslider();
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

        public void ToggleRotateProxy()
        {
            rot.ToggleVisibility();
        }

        public void Addmultibox()
        {
            GameObject newbox = Instantiate(box.gameObject, Boxanchor.position, Boxanchor.rotation);
            MultiboxList.Add(newbox);
            //box.gameObject.SetActive(true);
        }


    }

}
