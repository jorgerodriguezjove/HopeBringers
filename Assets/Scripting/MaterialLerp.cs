using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialLerp : MonoBehaviour
{
    public Material mat1;
    public Material mat2;
    public Material mat3;
    public Material mat4;
    public Material mat5;
    public Material currentmaterial;
    public float lerptime;
    private float q;
    private float w;
    private float e;
    private float r;
    private bool Qchange = false;
    private bool Wchange = false;
    private bool Echange = false;
    private bool Rchange = false;

    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        rend.material = mat1;
    }

    void Update()
    {
        

        if (Input.GetKey(KeyCode.Q))
        {
            Qchange = true;
        }


        if (Qchange == true)
        {
            
            rend.material.Lerp(mat1, mat2, q);
            q += Time.deltaTime / lerptime;
        }

        if (Input.GetKey(KeyCode.W))
        {

            Wchange = true;

        }

        if (Wchange == true)
        {
            rend.material.Lerp(mat2, mat3, w);
            w += Time.deltaTime / lerptime;
        }

        if (Input.GetKey(KeyCode.E))
        {

            Echange = true;

        }

        if (Echange == true)
        {
            rend.material.Lerp(mat3, mat4, e);
            e += Time.deltaTime / lerptime;
        }

        if (Input.GetKey(KeyCode.R))
        {

            Rchange = true;

        }

        if (Rchange == true)
        {
            rend.material.Lerp(mat4, mat5, r);
            r += Time.deltaTime / lerptime;
        }

    }
}
