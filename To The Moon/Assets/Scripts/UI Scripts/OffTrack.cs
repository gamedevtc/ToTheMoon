using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffTrack : MonoBehaviour
{
    //list of all enemies and players in a match
    [SerializeField] GameObject[] enemies;
    [SerializeField] PlayerTracker[] play;

    //the canvas that holds the arrows and the arrow respectively
    [SerializeField] GameObject arrParent;
    [SerializeField] GameObject arrow;

    //arrays that hold all of the arrows for each player/enemy
    [SerializeField] GameObject[] offEnem;
    [SerializeField] GameObject[] offPlay;

    [SerializeField] GameObject player; //used for getting the distance between it and an OOB object. also for making sure the player isn't tracked with the radar.
    [SerializeField] Camera pCam; //used for math with camera
    int enemL;
    int playL;

    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        play = FindObjectsOfType<PlayerTracker>(true);
        enemL = 0;
        playL = 0;
        //if (!Camera.main)
        //{
        //    Camera find = player.GetComponentInParent<Camera>();
        //    if (find)
        //    {
        //        pCam = find;
        //    }
        //}
    }

    bool onScreen(Vector3 input)
    {
        return (input.z > 0 && input.x > 0 && input.x < Screen.width && input.y > 0 && input.y < Screen.height);
    }

    void resizeEnemies(int l)
    {
        for (int i = 0; i < offEnem.Length; i++)
        {
            offEnem[i].SetActive(false);
            GameObject.Destroy(offEnem[i]);
        }
        offEnem = new GameObject[l];
        for (int i = 0; i < l; i++)
        {
            offEnem[i] = Instantiate(arrow, arrParent.transform, false);
        }
    }

    void resizePlayers(int l)
    {
        for (int i = 0; i < offPlay.Length; i++)
        {
            offPlay[i].SetActive(false);
            GameObject.Destroy(offPlay[i]);
        }
        offPlay = new GameObject[l];
        for (int i = 0; i < l; i++)
        {
            offPlay[i] = Instantiate(arrow, arrParent.transform, false);
            offPlay[i].SetActive(false);
        }
    }


    void Update()
    {
        if (!pCam)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length > 0)
            {
                if (enemies.Length != enemL)
                {
                    resizeEnemies(enemies.Length);
                    enemL = enemies.Length;
                }
            }

            for (int i = 0; i < enemies.Length; i++)
            {

                if (enemies[i].activeSelf)
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(enemies[i].transform.position);
                    float dist = Vector3.Distance(player.transform.position, enemies[i].transform.position);
                    if (onScreen(screenPos))
                    {
                        offEnem[i].SetActive(false);
                    }
                    if (!onScreen(screenPos))
                    {

                        if (screenPos.z < 0)
                        {
                            screenPos *= -1;
                        }
                        Vector3 center = new Vector3(Screen.width, Screen.height, 0) / 2;
                        screenPos -= center;

                        float ang = Mathf.Atan2(screenPos.y, screenPos.x);
                        ang -= 90 * Mathf.Deg2Rad;

                        float cos = Mathf.Cos(ang);
                        float sin = -Mathf.Sin(ang);

                        screenPos = center + new Vector3(sin * 150, cos * 150, 0);

                        float m = cos / sin;

                        Vector3 screenBounds = center * 0.9f;

                        if (cos > 0)
                        {
                            screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
                        }
                        else
                        {
                            screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
                        }

                        if (screenPos.x > screenBounds.x)
                        {
                            screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                        }
                        else if (screenPos.x < -screenBounds.x)
                        {
                            screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                        }

                        offEnem[i].SetActive(true);
                        offEnem[i].transform.localPosition = screenPos;
                        offEnem[i].transform.localRotation = Quaternion.Euler(0, 0, ang * Mathf.Rad2Deg + 135);
                    }
                }
            }
        }
        else
        {
            play = FindObjectsOfType<PlayerTracker>(true);
            if (play.Length != playL)
            {
                resizePlayers(play.Length);
                playL = play.Length;
            }

            for (int i = 0; i < play.Length; i++)
            {
                if (play[i].ship == player || !play[i].ship.activeSelf)
                {
                    offPlay[i].SetActive(false);
                }
                if (play[i].ship.activeSelf && play[i].ship != player)
                {
                    Vector3 screenPos = pCam.WorldToScreenPoint(play[i].ship.transform.position);
                    float dist = Vector3.Distance(player.transform.position, play[i].ship.transform.position);
                    if (onScreen(screenPos))
                    {
                        offPlay[i].SetActive(false);
                    }
                    if (!onScreen(screenPos))
                    {

                        if (screenPos.z < 0)
                        {
                            screenPos *= -1;
                        }
                        Vector3 center = new Vector3(Screen.width, Screen.height, 0) / 2;
                        screenPos -= center;

                        float ang = Mathf.Atan2(screenPos.y, screenPos.x);
                        ang -= 90 * Mathf.Deg2Rad;

                        float cos = Mathf.Cos(ang);
                        float sin = -Mathf.Sin(ang);

                        screenPos = center + new Vector3(sin * 150, cos * 150, 0);

                        float m = cos / sin;

                        Vector3 screenBounds = center * 0.9f;

                        if (cos > 0)
                        {
                            screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
                        }
                        else
                        {
                            screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
                        }

                        if (screenPos.x > screenBounds.x)
                        {
                            screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                        }
                        else if (screenPos.x < -screenBounds.x)
                        {
                            screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                        }
                        offPlay[i].SetActive(true);
                        offPlay[i].transform.localPosition = screenPos;
                        offPlay[i].transform.localRotation = Quaternion.Euler(0, 0, ang * Mathf.Rad2Deg + 135);
                    }
                }
            }
        }
    }
}
