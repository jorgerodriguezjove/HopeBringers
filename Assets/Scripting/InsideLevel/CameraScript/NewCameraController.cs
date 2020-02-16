using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewCameraController : MonoBehaviour
{
    #region VARIABLES

    [Header("POSICIÓN DE CÁMARA")]

    //Distancia desde el focusPoint hasta la cámara.
    [SerializeField]
    float distanceFromCenterToCamera;

    //Diferencia de altura entre las cámaras y el punto central.
    //Este número también implica la rotación de la cámara (lo picada que esta).
    [SerializeField]
    float heightOffset;

    //Distancia que se suma al hacer zoomOut
    [SerializeField]
    float zoomDifference;

    [Header("CONTROLES DE CÁMARA")]
    [SerializeField]
    private float panSpeed;
    [SerializeField]
    private float borderThickness;
    [SerializeField]
    private float timeRotation;
    [SerializeField]
    private float timeZoom;

    //Rotación de la cámara
    private enum cameraRotationPosition {north, east, south, west }
    [SerializeField]
    private cameraRotationPosition currentRotationPosition;

    private enum cameraZoomLevel {close, far }
    private cameraZoomLevel currentCameraZoom;

    //Vector que guarda la posición y los cambios que se realizan al pulsar las teclas para mover la cámara
    Vector3 pos;

    //Quaternion en el que se guarda la rotación que tiene que usar según si está en north, east...
    Quaternion referenceQuaternionToAddRotation;

    //Bool que sirve para indicar si la cámara se puede mover y rotar. Se usa para no poder mover y rotar la cámara mientras se hace zoom;
    [SerializeField]
    bool canMoveAndRotateCamera = true;
    //Bool que sirve para indicar si la cámara puede hacer zoom. Se usa para no poder hacer zoom mientras se está rotando la cámara.
    [SerializeField]
    bool canZoomCamera = true;

    [Header("REFERENCIAS")]

    [SerializeField]
    GameObject myCamera;

    [SerializeField]
    CameraBounds cameraBoundsRef;
    Vector3 cameraBoundsSize;

    [Header("DEBUG")]
    [SerializeField]
    bool deactivateBorderMovement;

    #endregion

    private void Start()
    {
        cameraBoundsSize = cameraBoundsRef.boundsSize;

        //Inicializo el punto central y el diámetro
        StartCoroutine("SetZoomCoroutine");

        currentRotationPosition = cameraRotationPosition.north;

        currentCameraZoom = cameraZoomLevel.close;
    }

    #region ROTATION

    IEnumerator RotateCameraRightCoroutine()
    {
        canZoomCamera = false;

        CalculateReferenceRotation();
        gameObject.transform.DORotate((referenceQuaternionToAddRotation * Quaternion.Euler(0, 90, 0)).eulerAngles, timeRotation);

        if ((int)currentRotationPosition > 2)
        {
            currentRotationPosition = cameraRotationPosition.north;
        }

        else
        {
            currentRotationPosition += 1;
        }

        yield return new WaitForSeconds(timeRotation);

        canZoomCamera = true;
    }

    IEnumerator RotateCameraLeftCoroutine()
    {
        canZoomCamera = false;

        CalculateReferenceRotation();
        gameObject.transform.DORotate((referenceQuaternionToAddRotation * Quaternion.Euler(0, -90, 0)).eulerAngles, timeRotation);

        if ((int)currentRotationPosition < 1)
        {
            currentRotationPosition = cameraRotationPosition.west;
        }

        else
        {
            currentRotationPosition -= 1;
        }

        yield return new WaitForSeconds(timeRotation);

        canZoomCamera = true;
    }

    //Calcula el transform.rotation que tendría el gameObject en cada punto (norte,sur...) para usarlo como referencia y sumarle o restarle 90º.
    //Esto hace que en caso de darle varias veces al botón de rotar no tome la rotación del objeto mientras esta griando si no directametne en la que va a acabar.
    private void CalculateReferenceRotation()
    {
        if (currentRotationPosition == cameraRotationPosition.north)
            referenceQuaternionToAddRotation = new Quaternion(0,0,0,1);

        if (currentRotationPosition == cameraRotationPosition.east)
            referenceQuaternionToAddRotation = new Quaternion(0, 0.7f, 0, 0.7f);

        if (currentRotationPosition == cameraRotationPosition.south)
            referenceQuaternionToAddRotation = new Quaternion(0, 1, 0, 0);

        if (currentRotationPosition == cameraRotationPosition.west)
            referenceQuaternionToAddRotation = new Quaternion(0,0.7f, 0, -0.7f);
    }
    #endregion

    #region ZOOM

    private void ZoomOut()
    {
        if (currentCameraZoom != cameraZoomLevel.far)
        {
            distanceFromCenterToCamera += zoomDifference;
            heightOffset += zoomDifference;
            StartCoroutine("SetZoomCoroutine");
            currentCameraZoom = cameraZoomLevel.far;
        }
    }

    private void ZoomIn()
    {
        if (currentCameraZoom != cameraZoomLevel.close)
        {
            distanceFromCenterToCamera -= zoomDifference;
            heightOffset -= zoomDifference;
            StartCoroutine("SetZoomCoroutine");
            currentCameraZoom = cameraZoomLevel.close;
        }
    }

    IEnumerator SetZoomCoroutine()
    {
        canMoveAndRotateCamera = false;

        if (currentRotationPosition == cameraRotationPosition.north)
        {
            myCamera.transform.DOMove(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + heightOffset, gameObject.transform.position.z - distanceFromCenterToCamera), timeZoom);
        }

        else if (currentRotationPosition == cameraRotationPosition.east)
        {
            myCamera.transform.DOMove(new Vector3(gameObject.transform.position.x - distanceFromCenterToCamera, gameObject.transform.position.y + heightOffset, gameObject.transform.position.z), timeZoom);
        }

        else if (currentRotationPosition == cameraRotationPosition.south)
        {
            myCamera.transform.DOMove(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + heightOffset, gameObject.transform.position.z + distanceFromCenterToCamera), timeZoom);
        }

        else if (currentRotationPosition == cameraRotationPosition.west)
        {
            myCamera.transform.DOMove(new Vector3(gameObject.transform.position.x + distanceFromCenterToCamera, gameObject.transform.position.y + heightOffset, gameObject.transform.position.z), timeZoom);
        }

        yield return new WaitForSeconds(timeZoom);

        canMoveAndRotateCamera = true;
    }

    #endregion

    private void Update()
    {
        //La cámara siempre mira hacia el focusPoint
        myCamera.transform.LookAt(gameObject.transform);

        //Seteo la posición actual
        pos = transform.position;

        //Ifs que hacen que la cámara se mueva correctamente independientemente de la rotación de la cámara y modifican la posición.
        #region MOVEMENT

        if (canMoveAndRotateCamera)
        {
            if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - borderThickness && !deactivateBorderMovement)
            {
                if (currentRotationPosition == cameraRotationPosition.north)
                    pos.z += panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.east)
                    pos.x += panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.south)
                    pos.z -= panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.west)
                    pos.x -= panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - borderThickness && !deactivateBorderMovement)
            {
                if (currentRotationPosition == cameraRotationPosition.north)
                    pos.x += panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.east)
                    pos.z -= panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.south)
                    pos.x -= panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.west)
                    pos.z += panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= borderThickness && !deactivateBorderMovement)
            {
                if (currentRotationPosition == cameraRotationPosition.north)
                    pos.z -= panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.east)
                    pos.x -= panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.south)
                    pos.z += panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.west)
                    pos.x += panSpeed * Time.deltaTime;

            }

            if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= borderThickness && !deactivateBorderMovement)
            {
                if (currentRotationPosition == cameraRotationPosition.north)
                    pos.x -= panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.east)
                    pos.z += panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.south)
                    pos.x += panSpeed * Time.deltaTime;
                if (currentRotationPosition == cameraRotationPosition.west)
                    pos.z -= panSpeed * Time.deltaTime;
            }
        }
        #endregion

        //Actualiza la posición
        transform.position = pos;

        //Input Rotación
        if (canMoveAndRotateCamera)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StopCoroutine("RotateCameraRightCoroutine");
                StartCoroutine("RotateCameraRightCoroutine");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StopCoroutine("RotateCameraLeftCoroutine");
                StartCoroutine("RotateCameraLeftCoroutine");
            }
        }

        //Input Zoom
        if (canZoomCamera)
        {
            //Input Zoom
            if (Input.GetKeyDown(KeyCode.Z))
            {
                ZoomIn();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                ZoomOut();
            }
        }
    }

    private void LateUpdate()
    {
        LockPositionInBounds();
    }

    //Mantengo el focuspoint dentro del área de juego
    private void LockPositionInBounds()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, cameraBoundsRef.transform.position.x - cameraBoundsSize.x/2, cameraBoundsRef.transform.position.x + cameraBoundsSize.x/2),
            transform.position.y,
            Mathf.Clamp(transform.position.z, cameraBoundsRef.transform.position.z - cameraBoundsSize.z/2, cameraBoundsRef.transform.position.z + cameraBoundsSize.z/2)
            );
    }
}
