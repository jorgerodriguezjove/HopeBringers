using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateButton : MonoBehaviour
{
    [SerializeField]
     public PlayerUnit.FacingDirection newDirection;

    UIManager UIM;

    private void Start()
    {
        UIM = FindObjectOfType<UIManager>();
    }

    private void OnMouseDown()
    {
        UIM.RotatePlayerInNewDirection(newDirection);
    }

    public void UiManagerChangeDirection()
    {
        Debug.Log("Funciono");
        UIM.RotatePlayerInNewDirection(newDirection);
    }

}
