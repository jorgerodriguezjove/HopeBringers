using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [HideInInspector]
    protected Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    protected virtual void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}
