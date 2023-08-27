using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var markinterface = other.GetComponent<DxRextention.MarkInterface>();
        if ( markinterface != null)
        {
           markinterface.selectBox();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var markinterface = other.GetComponent<DxRextention.MarkInterface>();
        if ( markinterface != null)
        {
            markinterface.unselectBox();
        }
    }

}
