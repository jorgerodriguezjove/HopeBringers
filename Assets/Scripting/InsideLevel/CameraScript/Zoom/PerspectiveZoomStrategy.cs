using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveZoomStrategy : IZoomStrategy
{
    Vector3 normalizedCameraPosition; 
    float currentZoomLevel;
    float currentCameraRotation;


    float myFarZoomLimit;
    float myMaXZoomCamera;
    float myMinZoomCamera;


    public PerspectiveZoomStrategy(Camera cam, Vector3 offset, float startingZoom, float farZoomLimit, float _maxAngleCamera, float _myMinZoomCamera)
    {
        normalizedCameraPosition = new Vector3(0f, Mathf.Abs(offset.y), -Mathf.Abs(offset.x)).normalized;
        //Posición de la cámara normalizada en magnitud1
        currentZoomLevel = startingZoom;

        myFarZoomLimit = farZoomLimit;
        myMaXZoomCamera = _maxAngleCamera;
        myMinZoomCamera = _myMinZoomCamera;

        PositionCamera(cam);
    }

    private void PositionCamera(Camera cam)//Posición de la cámara en relación al Zoom
    {
        cam.transform.localPosition = normalizedCameraPosition * currentZoomLevel;

        cam.transform.localEulerAngles = new Vector3(currentCameraRotation, cam.transform.localRotation.eulerAngles.y, cam.transform.localRotation.eulerAngles.z); 
    }

    public void ZoomIn(Camera cam, float delta, float nearZoomLimit)
    {
        if (currentZoomLevel <= nearZoomLimit) return;
        currentZoomLevel = Mathf.Max(currentZoomLevel - delta, nearZoomLimit);

        //Rotación
        currentCameraRotation = Mathf.Lerp(myMinZoomCamera, myMaXZoomCamera, currentZoomLevel/ myFarZoomLimit);

        PositionCamera(cam);
    }

    public void ZoomOut(Camera cam, float delta, float farZoomLimit)
    {
        if (currentZoomLevel >= farZoomLimit) return;
        currentZoomLevel = Mathf.Min(currentZoomLevel + delta, farZoomLimit);

        //Rotación
        currentCameraRotation = Mathf.Lerp(myMinZoomCamera, myMaXZoomCamera, currentZoomLevel / myFarZoomLimit);

        PositionCamera(cam);
    }
}
