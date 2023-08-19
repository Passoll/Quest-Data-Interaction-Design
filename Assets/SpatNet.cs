using System;
using System.Collections;
using System.Collections.Generic;
using DxR;
using UnityEngine;

public class SpatNet : MonoBehaviour
{

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DxRextention.MarkInterface>() != null)
        {
            other.gameObject.SetActive(false);
        }
       
    }
}
