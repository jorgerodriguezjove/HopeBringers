using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
    #region VARIABLES

    ////Número de esquinas/posiciones de cámara
    //int  numberOfCorners = 4;

    //Distancia en x y z desde el centro del cuadrado hasta las posiciones de la cámara
    [SerializeField]
    int squareSideLength;

    ////Diágonal del cuadrado = Diámetro de circulo que inscribe al cuadrado. Se calcula en el start
    //float diameter;

    //Diferencia de altura entre las cámaras y el punto central.
    //Este número también implica la rotación de la cámara (lo picada que esta).
    [SerializeField]
    int heightOffset;


    [Header("Move Controls")]
    [SerializeField]
    private float panSpeed;
    [SerializeField]
    private float borderThickness;

    private enum cameraRotationPosition {north, east, south, west }
    private cameraRotationPosition currentRotationPosition;

    Vector3 pos;

    ////Lista con las esquinas del cuadrado = posiciones de la cámara.
    //List<Vector3> corners = new List<Vector3>();

    [SerializeField]
    GameObject myCamera;

    [SerializeField]
    CameraBounds cameraBoundsRef;
    Vector3 cameraBoundsSize;

    #endregion

    private void Start()
    {
        cameraBoundsSize = cameraBoundsRef.boundsSize;

        //Inicializo el punto central y el diámetro
        myCamera.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + heightOffset, gameObject.transform.position.z - squareSideLength);
        //diameter = squareSideLength * Mathf.Sqrt(2);

        currentRotationPosition = cameraRotationPosition.north;

        ////Creo el cuadrado
        //CreateSquare();
    }



    private void CreateSquare()
    {
        ////Cámara abajo que mira hacia el norte
        //corners.Add(new Vector3(centerPoint.position.x, centerPoint.position.y + heightOffset, centerPoint.position.z - squareSideLength));

        ////Cámara izquierda que mira hacia el este
        //corners.Add(new Vector3(centerPoint.position.x - squareSideLength, centerPoint.position.y + heightOffset, centerPoint.position.z));

        ////Cámara arriba que mira hacia el sur
        //corners.Add(new Vector3(centerPoint.position.x, centerPoint.position.y + heightOffset, centerPoint.position.z + squareSideLength));

        ////Cámara derecha que mira hacia el oeste
        //corners.Add(new Vector3(centerPoint.position.x + squareSideLength, centerPoint.position.y + heightOffset, centerPoint.position.z ));

        //for (int i = 0; i < corners.Count; i++)
        //{
        //    Instantiate(new GameObject(), corners[i], centerPoint.transform.rotation, centerPoint);
        //}
    }

    public void RotateCameraRight()
    {
        myCamera.transform.RotateAround(gameObject.transform.position, Vector3.up, 90);

        if ((int)currentRotationPosition > 2)
        {
            currentRotationPosition = cameraRotationPosition.north;
        }

        else
        {
            currentRotationPosition += 1;
        }
    }

    public void RotateCameraLeft()
    {
        myCamera.transform.RotateAround(gameObject.transform.position, Vector3.down, 90);

        if ((int)currentRotationPosition < 1)
        {
            currentRotationPosition = cameraRotationPosition.west;
        }

        else
        {
            currentRotationPosition -= 1;
        }
    }

    private void Update()
    {
        Debug.Log((int)currentRotationPosition);

        myCamera.transform.LookAt(gameObject.transform);

        pos = transform.position;

        //Movimiento
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - borderThickness)
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

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - borderThickness)
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

        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <=  borderThickness)
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
       
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= borderThickness)
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
       
        transform.position = pos;






        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCameraRight();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCameraLeft();
        }
    }

    private void LateUpdate()
    {

        LockPositionInBounds();
    }


    private void LockPositionInBounds()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, cameraBoundsRef.transform.position.x - cameraBoundsSize.x/2, cameraBoundsRef.transform.position.x + cameraBoundsSize.x/2),
            transform.position.y,
            Mathf.Clamp(transform.position.z, cameraBoundsRef.transform.position.z - cameraBoundsSize.z/2, cameraBoundsRef.transform.position.z + cameraBoundsSize.z/2)
            );
    }
}
