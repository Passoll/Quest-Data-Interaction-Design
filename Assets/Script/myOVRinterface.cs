using System;
using System.Collections;
using System.Collections.Generic;
using DxR;
using UnityEngine;
using Oculus.Interaction.Input;


public class myOVRinterface : MonoBehaviour
{
    
    public OVRManager MyManager;
    public IController myController;
    public Vis myDxR;
    //public GameObject Axis;

    public enum Datatype
    {
        trajectory,
        scatter,
    }

    public Datatype currenttype = Datatype.scatter;
    
    public void ChangePassthroughState()
    {
        MyManager.isInsightPassthroughEnabled = !MyManager.isInsightPassthroughEnabled;
        Debug.Log("successfully change the passthrough state");
    }

    public void SwitchData()
    {
        // switch
        ChangeDataViz();
        myDxR.UpdateVisSpecsFromTextSpecs();
        Debug.Log("change the data");
    }

    private void Awake()
    {
       
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


    void Update()
    {
  
    }
}
