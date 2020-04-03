using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeToken : MonoBehaviour
{
    #region VARIABLES

    //Bool que indica si el token ya ha sido girado.
    [HideInInspector]
    public bool haveIFlipped;

    #endregion

    public void FlipToken()
    {
        haveIFlipped = true;
        //CAMBIAR ESTO POR UN TRIGGER EN VEZ DE LLAMAR AL PLAY
        GetComponent<Animator>().Play("LifeTokenFLip");
    }

    public void ResetToken()
    {
        haveIFlipped = false;
        GetComponent<Animator>().Play("LifeTokenReset");
    }

    public void ArmoredToken()
    {
        GetComponent<Animator>().Play("ArmoredToken");

    }

}
