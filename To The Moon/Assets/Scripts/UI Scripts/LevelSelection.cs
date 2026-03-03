using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] GameObject[] options = new GameObject[5];
    [SerializeField] int selection;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<options.Length; i++)
        {
            options[i].SetActive(false);
        }
        selection = -1;
    }
    public void ChangeSelection(int s)
    {
        selection = s;
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == selection)
            {
                options[i].SetActive(true);
            }
            else
            {
                options[i].SetActive(false);
            }
        }
        //if (selection >= 0)
        //{
        //    options[selection].SetActive(true);
        //}
    }
}
