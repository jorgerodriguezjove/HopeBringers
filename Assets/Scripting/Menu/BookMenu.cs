using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMenu : MonoBehaviour
{

    public Animator anim;
    // Start
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("isOpen?", true);
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetBool("isOpen?", false);
            
           
        }

    }
}
