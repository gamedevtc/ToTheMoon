using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        if (!player.activeInHierarchy)
        {
            player.SetActive(true);
        }
        if (!camera.activeInHierarchy)
        {
            camera.SetActive(true);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!player.activeInHierarchy)
        {
            player.SetActive(true);
        }
        if (!camera.activeInHierarchy)
        {
            camera.SetActive(true);
        }
    }
}
