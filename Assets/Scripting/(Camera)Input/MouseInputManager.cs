using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : InputManager
{
    //VARIABLES

    Vector2Int screen;
    float mousePositionOnRotateStart;

    // EVENTOS

    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;

    //MÉTODOS

    private void Awake()
    {
        screen = new Vector2Int(Screen.width, Screen.height);
    }

    private void Update()
    {
        Vector3 mp = Input.mousePosition;
        bool mouseValid = (mp.y <= screen.y * 1.05f && mp.y >= screen.y * -0.05f &&
            mp.x <= screen.x * 1.05f && mp.x >= screen.x * -0.05f);
//Que porcentaje de offset para que el ratón sea valido para el movimiento(esta al 5% de los bordes de la pantalla)

        if (!mouseValid) return; // Valid?

        //MOVIMIENTO
        if (mp.y > screen.y * 0.95f)//Arriba
        {
            OnMoveInput?.Invoke(Vector3.forward);
        }
        else if(mp.y < screen.y * 0.05f)//Abajo
        {
            OnMoveInput?.Invoke(-Vector3.forward);
        }

        if (mp.x > screen.x * 0.95f)//Derecha
        {
            OnMoveInput?.Invoke(Vector3.right);
        }
        else if (mp.x < screen.x * 0.05f)//Izquierda
        {
            OnMoveInput?.Invoke(-Vector3.right);
        }

        //ROTACIÓN
        if (Input.GetMouseButtonDown(1))
        {
            mousePositionOnRotateStart = mp.x;
        }
        else if (Input.GetMouseButton(1))
        {
            if(mp.x < mousePositionOnRotateStart)
            {
                OnRotateInput?.Invoke(-1f);
            }
            else if(mp.x > mousePositionOnRotateStart)
            {
                OnRotateInput?.Invoke(1f);
            }
        }

        //ZOOM
        if(Input.mouseScrollDelta.y > 0)
        {
            OnZoomInput?.Invoke(-3f);
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            OnZoomInput?.Invoke(3f);
        }
    }

    
}
