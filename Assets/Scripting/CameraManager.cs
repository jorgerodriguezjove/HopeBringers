using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{//Se podría hacer con scriptable object
    [Header("Camera Positioning")]
    public Vector2 cameraOffset = new Vector2(10f, 14f);
    public float lookAtOffset = 2f;

    [Header("Move Controls")]
    public float intOutSpeed = 5f;
    public float lateralSpeed = 5f;
    public float rotateSpeed = 45f;

    [Header("Move Bounds")]
    public Vector2 minBounds, maxBounds;

    [Header("Zoom Controls")]
    public float zoomSpeed = 4f;
    public float zoomNearLimit = 2f;
    public float farZoomLimit = 16f;
    public float startingZoom = 5f;

    IZoomStrategy zoomStrategy;
    Vector3 frameMove;
    float frameRotate;
    float frameZoom;
    Camera cam;

    //Métodos

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(0f, Mathf.Abs(cameraOffset.y), -Mathf.Abs(cameraOffset.x));
        //Mathf.Abs para asegurar que la cámara no se va debajo del tablero
        //-Mathf porque si la cámara es negativa en la Z cuando mire hacia delante mire hacia el camera focus
        zoomStrategy = new OrtographicZoomStrategy(cam, startingZoom); //asignamos la cámara y el zoom inicial
        cam.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
        //Asignamos un offset de altura respecto al camera focus para dar una vista más natural(que no mire al suelo)     
    }
    
}
