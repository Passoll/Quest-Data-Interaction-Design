using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myOVRinterface : MonoBehaviour
{
    [SerializeField]
    public OVRManager MyManager;
    
  
    
    public void ChangePassthroughState()
    {
        MyManager.isInsightPassthroughEnabled = !MyManager.isInsightPassthroughEnabled;
        Debug.Log("successfully change the passthrough state");
    }

    private void Update()
    {
        
    }
}
