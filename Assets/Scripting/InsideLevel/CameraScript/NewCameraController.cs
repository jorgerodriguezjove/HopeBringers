using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewCameraController : MonoBehaviour
{
    #region VARIABLES

    [Header("MOVIMIENTO")]
    [SerializeField]
    private float movementSpeed;
    private float movementTime = 5f;
    [SerializeField]
    private float borderThickness;

    //Vector que guarda la posición y los cambios que se realizan al pulsar las teclas para mover la cámara
    Vector3 newPos;

    [Header("ROTACIÓN")]
    [SerializeField]
    private float rotationAmount;
    private float timeRotation = 5f;

    //Quaternion que guarda la nueva rotación al pulsar inputs.
    Quaternion newRotation;

    [Header("ZOOM")]
    [Tooltip("NO SE PUEDE CAMBIAR EN PLAY")]
    [SerializeField]
    private int zoomAmount;
    //Zoom amount tiene que ser un vector 3 para aplicar los cambios, pero en el editor es poco intuitivo. Lo que hago es que en el editor aparezca un int y ese int
    //se pone como -y & como z en el nuevo vector new Vector3 (x, -zoomAmount, zoomAmount);
    private Vector3 realZoomAmount;
    private float timeZoom = 5f;

    private enum cameraZoomLevel {close, far }
    private cameraZoomLevel currentCameraZoom;

    //Vector que guarda la posición de la cámara para el zoom
    private Vector3 newZoom;

    [Header ("LIMITAR ACCIONES")]

    //Bool que sirve para indicar si la cámara se puede mover y rotar. Se usa para no poder mover y rotar la cámara mientras se hace zoom;
    bool canMoveCamera = true;

    bool canRotateCamera = true;
    //Bool que sirve para indicar si la cámara puede hacer zoom. Se usa para no poder hacer zoom mientras se está rotando la cámara.
    bool canZoomCamera = true;

    [Header("MOUSE")]
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    private Vector3 mouseRotateStartPosition;
    private Vector3 mouseRotateCurrentPosition;


    [Header("SWIPE")]
    public float minSwipeLength = 200f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

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
        //Seteo los bounds de la cámara
        cameraBoundsSize = cameraBoundsRef.boundsSize;

        //Seteo el estado del zoom
        currentCameraZoom = cameraZoomLevel.close;

        //Seteo la posición, rotación y zoom iniciales
        newPos = transform.position;
        newRotation = transform.rotation;
        newZoom = myCamera.transform.localPosition;

        //Creo el vector zoom teniendo en cuenta lo que se ha puesto en el editor
        realZoomAmount = new Vector3(realZoomAmount.x, -zoomAmount, zoomAmount);
    }

    #region MOVEMENT

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - borderThickness && !deactivateBorderMovement)
        {
            newPos += (transform.forward * movementSpeed);
        }

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - borderThickness && !deactivateBorderMovement)
        {
            newPos += (transform.right * movementSpeed);
        }

        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= borderThickness && !deactivateBorderMovement)
        {
            newPos += (transform.forward * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= borderThickness && !deactivateBorderMovement)
        {
            newPos += (transform.right * -movementSpeed);

        }

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * movementTime);
    }

    void HandleMovementMouseInput()
    {
        //Click der down
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click Der");

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entryPoint;

            if (plane.Raycast(ray, out entryPoint))
            {
                dragStartPosition = ray.GetPoint(entryPoint);
            }
        }

        //Mantener click der
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Hold Click Der");

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entryPoint;

            if (plane.Raycast(ray, out entryPoint))
            {
                dragCurrentPosition = ray.GetPoint(entryPoint);

                newPos = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }

    #endregion

    #region ZOOM

    private void ZoomOut()
    {
        if (currentCameraZoom != cameraZoomLevel.far)
        {
            newZoom -= realZoomAmount;
            currentCameraZoom = cameraZoomLevel.far;
        }
    }

    private void ZoomIn()
    {
        if (currentCameraZoom != cameraZoomLevel.close)
        {
            newZoom += realZoomAmount;
            currentCameraZoom = cameraZoomLevel.close;
        }
    }

    #endregion

    void HandleRotationMouseInput()
    {
        //Click botón
        if (Input.GetMouseButtonDown(1))
        {
            mouseRotateStartPosition = Input.mousePosition;
        }

        //Mantener pulsado el botón
        if (Input.GetMouseButton(1))
        {
            mouseRotateCurrentPosition = Input.mousePosition;

            //Calculo la diferencia entre start y current
            Vector3 difference = mouseRotateStartPosition - mouseRotateCurrentPosition;

            //Reseteo la posición inicial a la current en el siguiente frame
            mouseRotateStartPosition = mouseRotateCurrentPosition;

            //Roto la cámara. Está en negativo para que gire al contrario que el ratón.
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }

        if (Input.GetMouseButtonUp(1))
        {
            //Hacer snap al ángulo multiplo de 45 más cercano
            newRotation = Quaternion.Euler(new Vector3(Vector3.up.x, Mathf.Round(newRotation.eulerAngles.y / 45) * 45, Vector3.up.z));
        }
    }

    private void Update()
    {
        //La cámara siempre mira hacia el focusPoint
        myCamera.transform.LookAt(gameObject.transform);

        //Movimiento de la cámara
        if (canMoveCamera)
        {
            HandleMovementInput();
            HandleMovementMouseInput();
        }

        //Input Rotación
        if (canRotateCamera)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                canZoomCamera = false;

                //Seteo la nueva rotación
                newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);

                canZoomCamera = true;

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                canZoomCamera = false;

                //Seteo la nueva rotación
                newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);

                canZoomCamera = true;
            }

            //Hago un lerp entre la rotación actual y la nueva.
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * timeRotation);

            //Rotación con ratón.
            HandleRotationMouseInput();

        }

        //Input Zoom
        if (canZoomCamera)
        {
            //Input Zoom
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                ZoomIn();
            }

            if (Input.GetKeyDown(KeyCode.X) || Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                ZoomOut();
            }
        }

        myCamera.transform.localPosition = Vector3.Lerp(myCamera.transform.localPosition, newZoom, Time.deltaTime * timeZoom);

        //Lock sobre un enemigo para seguirle mientras se mueve
        if (iscameraLockedOnEnemy)
        {
            transform.position = new Vector3(characterToFocus.transform.position.x, transform.transform.position.y, characterToFocus.transform.position.z);
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

    public void SetCameraMovable(bool _shouldmove)
    {
        canMoveCamera = _shouldmove;
        canZoomCamera = _shouldmove;

        if (_shouldmove)
        {
            iscameraLockedOnEnemy = false;
            characterToFocus = null;
        }
    }

    private GameObject characterToFocus;
    [SerializeField]
    int focusDuration;
    

    IEnumerator FocusCameraOnCharacter()
    {
        canZoomCamera = false;
        canMoveCamera = false;

        transform.DOMove
             (new Vector3(characterToFocus.transform.position.x, transform.position.y, characterToFocus.transform.position.z), focusDuration);

        yield return new WaitForSeconds(focusDuration);

        //Para que no vuelva a su posición anterior seteo la posición de la cámara.
        newPos = transform.position;

        if (lockCamera)
        {
            iscameraLockedOnEnemy = true;
            lockCamera = false;
        }

        if (!iscameraLockedOnEnemy)
        {
            canZoomCamera = true;
            canMoveCamera = true;
        }
    }

    //Focus es simplemente mover la cámara hasta el enemigo
    public void FocusCameraOnCharacter(GameObject _charaterToFocus)
    {
        iscameraLockedOnEnemy = false;
        characterToFocus = _charaterToFocus;
        StartCoroutine("FocusCameraOnCharacter");
    }

    public bool lockCamera;
    private bool iscameraLockedOnEnemy;


    //Lock es mover la cámara y que esta siga al enemigo
    public void LockCameraOnEnemy(GameObject _enemyToFocus)
    {
        iscameraLockedOnEnemy = false;
        lockCamera = true;
        characterToFocus = _enemyToFocus;
        FocusCameraOnCharacter(characterToFocus);
    }
    
       


}
