using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoom : MonoBehaviour {

    protected virtual void LateUpdate()
    {
       

        // This will scale the current transform based on a multi finger pinch gesture
        Lean.LeanTouch.ScaleObject(transform, Lean.LeanTouch.PinchScale);
    }
}
