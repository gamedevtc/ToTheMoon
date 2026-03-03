using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviourPunCallbacks
{
    float time = 3;
    public  Text text;

    //public GameObject test;
    public override void OnEnable()
    {
        base.OnEnable();

        time = 5;

    }

   
    private void Update()
    {
        time -= Time.deltaTime;
        text.text = "Respawning in: " + time.ToString("F1");

        //if (time <= 0.000001)
        //{
        //    test.SetActive(true);
        //}

    }




}
