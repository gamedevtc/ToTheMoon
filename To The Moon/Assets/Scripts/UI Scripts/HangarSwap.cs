using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarSwap : MonoBehaviour
{
    [SerializeField] GameObject hangarScreen;
    [SerializeField] GameObject levelScreen;
    [SerializeField] GameObject customScreen;
    [SerializeField] GameObject hangarCam;
    [SerializeField] GameObject levelCam;
    [SerializeField] GameObject customCam;
    bool hUp = true;
    bool lUp = false;
    bool cUp = false;
    // Start is called before the first frame update
    void Start()
    {
        levelScreen.SetActive(false);
        customScreen.SetActive(false);
    }
    public void ToHangar()
    {
        if (lUp || cUp)
        {
            lUp = cUp = false;
        }
        hUp = true;
    }

    public void ToLevels()
    {
        if (hUp || cUp)
        {
            hUp = cUp = false;
        }
        lUp = true;
    }

    public void ToCustom()
    {
        if (lUp || hUp)
        {
            lUp = hUp = false;
        }
        cUp = true;
    }
    // Update is called once per frame
    void Update()
    {
        hangarScreen.SetActive(hUp);
        hangarCam.SetActive(hUp);
        levelScreen.SetActive(lUp);
        customScreen.SetActive(cUp);
        levelCam.SetActive(lUp);
        customCam.SetActive(cUp);
    }
}
