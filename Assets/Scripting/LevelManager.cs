using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    #region VARIABLES

    //Personaje actualmente seleccionado.
    //La dejo en serialize para ver si funciona correctamente.
    [SerializeField]
    public PlayerUnit selectedCharacter;

    //Tiles que actualmente están dispoibles para el movimiento de la unidad seleccionada
    [SerializeField]
    List<IndividualTiles> tilesAvailableForMovement = new List<IndividualTiles>();

    //De momento se guarda aquí pero se podría contemplar que cada personaje tuviese un tiempo distinto.
    float timeForMovementAnimation = 0.2f;

    //Posición a la que tiene que moverse la unidad actualmente
    Vector3 currentTileVectorToMove;

    //Tile en el que ha empezado a moverse el personaje seleccionado. Se usa para volver a ponerlo donde estaba.
    IndividualTiles previousCharacterTile;

    //REFERENCIAS

    //Referencia al Tile Manager
    private TileManager TM;

    #endregion

    #region INIT

    private void Start()
    {
        TM = FindObjectOfType<TileManager>();
    }

    #endregion

    //Al clickar sobre una unidad del jugador se llama a esta función
    public void SelectUnit(int movementUds, PlayerUnit selectedUnit)
    {
        tilesAvailableForMovement.Clear();
        selectedCharacter = selectedUnit;
        tilesAvailableForMovement = TM.checkAvailableTilesForMovement(movementUds, selectedUnit);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DeSelectUnit();
        }
    }

    public void DeSelectUnit()
    {
        if (selectedCharacter != null)
        {
            //Si el personaje ya se ha movido lo vuelvo a poner donde estaba.
            if (selectedCharacter.hasMoved)
            {
                selectedCharacter.gameObject.transform.position = new Vector3(previousCharacterTile.transform.position.x, previousCharacterTile.transform.position.y + 1,previousCharacterTile.transform.position.z);

                selectedCharacter.myCurrentTile = previousCharacterTile;

                selectedCharacter.hasMoved = false;

                SelectUnit(selectedCharacter.movementUds, selectedCharacter);
            }

            //Si no se ha movido lo deselecciono.
            else 
            {
                //Deselecciono
                for (int i = 0; i < tilesAvailableForMovement.Count; i++)
                {
                    tilesAvailableForMovement[i].ColorDeselect();
                }
                tilesAvailableForMovement.Clear();
                selectedCharacter = null;
            }
        }
    }

    public void MoveUnit(IndividualTiles tileToMove)
    {
        for (int i = 0; i < tilesAvailableForMovement.Count; i++)
        {
            if (tileToMove == tilesAvailableForMovement[i])
            {
                previousCharacterTile = selectedCharacter.myCurrentTile;

                TM.CalculatePathForMovementCost(tileToMove.tileX, tileToMove.tileZ);
                StartCoroutine("MovingUnitAnimation");
                selectedCharacter.myCurrentTile = tileToMove;
            }
        }
    }


    IEnumerator MovingUnitAnimation ()
    {
        //Animación de movimiento
        for (int j = 1; j < TM.currentPath.Count; j++)
        {
            currentTileVectorToMove = new Vector3(TM.currentPath[j].transform.position.x, TM.currentPath[j].transform.position.y +1, TM.currentPath[j].transform.position.z);

            selectedCharacter.gameObject.transform.DOMove(currentTileVectorToMove, timeForMovementAnimation);
            yield return new WaitForSeconds(timeForMovementAnimation);
        }

        //Al terminar de moverse se deseleccionan los tiles
        for (int i = 0; i < tilesAvailableForMovement.Count; i++)
        {
            tilesAvailableForMovement[i].ColorDeselect();
        }

        tilesAvailableForMovement.Clear();
        selectedCharacter.hasMoved = true;
    }

    //Al deseleccionar la unidad acordarse de avisar al TileManager de que ha cambiado

}

