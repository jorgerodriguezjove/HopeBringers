using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateButton : MonoBehaviour
{
    [Header("DIRECTION")]
    [SerializeField]
     public PlayerUnit.FacingDirection newDirection;

    [Header("MATERIALS")]
    [SerializeField]
    private Material highlightedMat;
    private Material initialMaterial;

    //[Header("COLORS")]
    //[SerializeField]
    //private Color32 highlightedColor;
    //private Color32 initialColor;

    [Header("REFERENCIAS")]
    UIManager UIM;

    MeshRenderer myMeshRenderer;

    private void Start()
    {
        UIM = FindObjectOfType<UIManager>();
        myMeshRenderer = GetComponent<MeshRenderer>();
        initialMaterial = myMeshRenderer.material;

        //initialColor = myMeshRenderer.material.color;

    }

    private void OnMouseDown()
    {
        UIM.RotatePlayerInNewDirection(newDirection);
        myMeshRenderer.material = initialMaterial;

        //myMeshRenderer.material.SetColor("Init", initialColor);
    }

    private void OnMouseEnter()
    {                
        myMeshRenderer.material = highlightedMat;

        //myMeshRenderer.material.SetColor("Highlighted", highlightedColor);
    }

    private void OnMouseExit()
    {
        myMeshRenderer.material = initialMaterial;

        //myMeshRenderer.material.SetColor("Init", initialColor);
    }

    public void UiManagerChangeDirection()
    {
        Debug.Log("Funciono");
        UIM.RotatePlayerInNewDirection(newDirection);
    }   
}
