using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class EnvironmentChooser : MonoBehaviour
{
    [SerializeField] public bool EnableFog;
    [SerializeField] public bool RandomEnvironment;
    [SerializeField] public int ChosenEnvironment;

    [SerializeField] GameObject[] EnvironmentObject;
    int pickedNum;
    [SerializeField] MatchSettings mm;
    

    // Start is called before the first frame update
    void Start()
    {
        mm = FindObjectOfType<MatchSettings>();
        EnableFog = mm.foggy;

        pickedNum = Random.Range(0, EnvironmentObject.Length);

        for (int i = 0; i < EnvironmentObject.Length; i++)
        {
            EnvironmentObject[i].SetActive(false);
            if ((RandomEnvironment == false && i == ChosenEnvironment) || (RandomEnvironment == true && i == pickedNum))
            {
                EnvironmentObject[i].SetActive(true);
                if (EnableFog)
                {
                    Volume v = EnvironmentObject[i].transform.GetChild(0).GetComponent<Volume>();
                    VolumeProfile vp = v.sharedProfile;
                    vp.TryGet<Fog>(out var fog);
                    fog.enabled.overrideState = true;
                    fog.enabled.value = true;
                }
                else
                {
                    Volume v = EnvironmentObject[i].transform.GetChild(0).GetComponent<Volume>();
                    VolumeProfile vp = v.sharedProfile;
                    vp.TryGet<Fog>(out var fog);
                    fog.enabled.overrideState = false  ;
                    fog.enabled.value = false;
                }
            }
        }

    }


}
