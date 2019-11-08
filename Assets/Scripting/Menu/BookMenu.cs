using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class BookMenu : MonoBehaviour
{
    public CinemachineStateDrivenCamera bookCamera;
    public PlayableDirector DollyCameraOpen;
    public PlayableDirector DollyCameraClose;
    public Animator anim;
    bool isOpen;

    public GameObject canvasLevels;

    // Start
    void Start()
    {
        anim = GetComponent<Animator>();
        isOpen = anim.GetBool("isOpen?");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("isOpen?", true);
            canvasLevels.SetActive(true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetBool("isOpen?", false);
            canvasLevels.SetActive(false);
        }
    }
    void changeCamera()
    {
        if(isOpen)
        {
            DollyCameraOpen.Play();
            
        }
        if(!isOpen)
        {
            DollyCameraClose.Play();
            
        }
    }
}
