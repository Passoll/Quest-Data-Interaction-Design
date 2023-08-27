using System.Collections;
using System.Collections.Generic;
using DxRextention;
using UnityEngine;

public class Spatula : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform anchorpoint;

    public void ToggleVisibility()
    {
        if (!gameObject.activeSelf)
        {
            transform.position = anchorpoint.position;
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
