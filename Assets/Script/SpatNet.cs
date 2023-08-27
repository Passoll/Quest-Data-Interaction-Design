using System;
using System.Collections;
using System.Collections.Generic;
using DxR;
using UnityEngine;

public class SpatNet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var markinterface = other.GetComponent<DxRextention.MarkInterface>();
        if ( markinterface != null)
        {
            markinterface.selectBox();
        }
    }
}
