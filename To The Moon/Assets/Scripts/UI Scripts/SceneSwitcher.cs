using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    private GameManager manage;

    //private void Start()
    //{
    //    manage = FindObjectOfType<GameManager>();
    //}
    public void OnClick_ChangeScene(string scene)
    {
        
           // manage.Sync = false;
       
        SceneManager.LoadScene(scene);

    }

    //public void IsMultiplayer(bool multi)
    //{
    //    FindObjectOfType<GameManager>().setMulti(multi);
    //}

}
