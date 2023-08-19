using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOVRHandMenu
{
    void SetPlane(CustomHandPlane plane);
    void UpdatePosHand();
    void SetVisibility(bool state);
    void ToggleVisibility();

}
