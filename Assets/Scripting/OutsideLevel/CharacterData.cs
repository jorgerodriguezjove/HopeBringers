using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    //Referencia a la unidad que representa la figura
    [SerializeField]
    public PlayerUnit myUnit;

    private void OnMouseDown()
    {
        GameManager.Instance.unitsForCurrentLevel.Add(myUnit);
    }
}
