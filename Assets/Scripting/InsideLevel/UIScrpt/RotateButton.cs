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

    [Header("REFERENCIAS")]
    UIManager UIM;

    MeshRenderer myMeshRenderer;

    private void Start()
    {
        UIM = FindObjectOfType<UIManager>();
        myMeshRenderer = GetComponent<MeshRenderer>();
        initialMaterial = myMeshRenderer.material;
    }

    private void OnMouseDown()
    {
        UIM.RotatePlayerInNewDirection(newDirection);
        myMeshRenderer.material = initialMaterial;
    }

    private void OnMouseEnter()
    {
        myMeshRenderer.material = highlightedMat;   
    }

    private void OnMouseExit()
    {
        myMeshRenderer.material = initialMaterial;
    }

    public void UiManagerChangeDirection()
    {
        Debug.Log("Funciono");
        UIM.RotatePlayerInNewDirection(newDirection);
    }

}
