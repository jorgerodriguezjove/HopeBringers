using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class DragonSceneScript : MonoBehaviour
{

    public CinemachineStateDrivenCamera dragonCamera;
    public PlayableDirector DollyfollowCharacters;
    public PlayableDirector DollyCameraDragon;
    public PlayableDirector DollyCameraClose;
    public Animator dragonanim;
    //bool isFlying;
    //bool isRawring;
    //bool isWalking;
    //bool isFiring;

    //public GameObject canvasLevels;

    // Start
    void Start()
    {
        dragonanim = GetComponent<Animator>();
        dragonanim.SetBool("Fly Idle", true);

        //isFlying = dragonanim.GetBool("Fly idle");
    }

    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            dragonanim.SetBool("Fly Idle", false);

        }
        if (Input.GetKeyDown("2"))
        {
            dragonanim.SetBool("Cast Spell", true);

        }
        if (Input.GetKeyDown("3"))
        {
            dragonanim.SetBool("Walk Forward", true);
            DollyCameraDragon.Play();
        }
        if (Input.GetKeyDown("4"))
        {
            dragonanim.SetBool("Walk Forward", false);
            dragonanim.SetBool("Fire Breath Attack", true);

            //DollyCameraDragon.Stop();
        }
    }
    //void changeCamera()
    //{
    //    if (isFlying)
    //    {
    //        DollyfollowCharacters.Play();

    //    }
    //    if (isRawring)
    //    {
    //        DollyCameraDragon.Play();

    //    }
    //    if (isWalking)
    //    {
    //        DollyCameraClose.Play();

    //    }
    //}

    //public void ShowButtons()
    //{

    //    canvasLevels.SetActive(true);
    //}

    //public void HideButtons()
    //{

    //    canvasLevels.SetActive(false);
    //}
}
