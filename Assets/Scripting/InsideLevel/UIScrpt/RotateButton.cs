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

    public void UiManagerChangeDirection()
    {
        UIM.RotatePlayerInNewDirection(newDirection);
    }

}
