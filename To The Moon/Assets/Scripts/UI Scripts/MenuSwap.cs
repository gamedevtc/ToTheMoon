using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwap : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject options;
    [SerializeField] OptionsMenu oMenu;
    bool optionsUp = false;
    bool mainUp = true;
    void Start()
    {
        options.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void ToOptions()
    {
        optionsUp = true;
    }

    public void ToMain()
    {
        optionsUp = false;
        oMenu.MakeSelection(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!optionsUp)
        {
            if(!mainUp)
            {
                options.SetActive(false);
                mainMenu.SetActive(true);
                mainUp = true;
            }
        }
        if(optionsUp)
        {
            if(mainUp)
            {
                options.SetActive(true);
                mainMenu.SetActive(false);
                mainUp = false;
            }
        }
    }
}
