using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject controlText;
    [SerializeField] GameObject settingText;
    [SerializeField] GameObject soundText;
    [SerializeField] GameObject creditText;
    int select = 0;

    private void Start()
    {
        controlText.SetActive(false);
        settingText.SetActive(false);
        soundText.SetActive(false);
        creditText.SetActive(false);
    }

    public void MakeSelection(int sel)
    {
        select = sel;
    }

    // Update is called once per frame
    void Update()
    {
        if(select == 1)
        {
            settingText.SetActive(true);
        }
        if (select == 2)
        {
            soundText.SetActive(true);
        }
        if (select == 3)
        {
            controlText.SetActive(true);
        }
        if (select == 4)
        {
            creditText.SetActive(true);
        }

        if (select != 1)
        {
            settingText.SetActive(false);
        }
        if (select != 2)
        {
            soundText.SetActive(false);
        }
        if (select != 3)
        {
            controlText.SetActive(false);
        }
        if (select != 4)
        {
            creditText.SetActive(false);
        }
    }
}
