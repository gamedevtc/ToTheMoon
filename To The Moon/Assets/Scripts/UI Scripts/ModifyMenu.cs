using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Selections
{
    Color = 0,
    Body,
    Primary,
    Secondary,
    Ultimate,
    Default
}
public class ModifyMenu : MonoBehaviour
{
    [SerializeField] GameObject[] options = new GameObject[5];
    [SerializeField] GameObject[] cText = new GameObject[8];
    [SerializeField] GameObject[] bText = new GameObject[4];
    [SerializeField] GameObject[] pText = new GameObject[4];
    [SerializeField] GameObject[] sText = new GameObject[8];
    [SerializeField] GameObject[] uText = new GameObject[4];
    [SerializeField] GameObject optionReturn;
    [SerializeField] GameObject hangarReturn;
    [SerializeField] GameObject partSelection;
    Selections current = Selections.Default;
    bool[] opened = new bool[5];
    bool[] optionText = new bool[8];
    // Start is called before the first frame update
    void Start()
    {
        OptionUpdate();
        optionReturn.SetActive(false);
    }
    void OptionUpdate()
    {
        for (int i = 0; i < 5; i++)
        {
            opened[i] = false;
            if ((int)current == i)
            {
                //Debug.Log((int)current);
                opened[i] = true;
            }
        }
    }
    void TextUpdate()
    {
        for (int i = 0; i < 8; i++)
        {
            if (optionText[i])
            {
                switch (current)
                {
                    case Selections.Color:
                        if(cText[i])cText[i].SetActive(true);
                        break;
                    case Selections.Body:
                        if (bText[i]) bText[i].SetActive(true);
                        break;
                    case Selections.Primary:
                        if (pText[i]) pText[i].SetActive(true);
                        break;
                    case Selections.Secondary:
                        if (sText[i]) sText[i].SetActive(true);
                        break;
                    case Selections.Ultimate:
                        if (uText[i]) uText[i].SetActive(true);
                        break;
                }
            }
            else
            {
                if (i<cText.Length) cText[i].SetActive(false);
                if (i<bText.Length) bText[i].SetActive(false);
                if (i<pText.Length) pText[i].SetActive(false);
                if (i<sText.Length) sText[i].SetActive(false);
                if (i<uText.Length) uText[i].SetActive(false);

            }
        }
    }
    public int GetSelection()
    {
        return (int)current;
    }
    public void OpenText(int s)
    {
        for (int i = 0; i < 8; i++)
        {
            optionText[i] = false;
            if (s == i)
            {
                optionText[i] = true;
            }
        }
    }
    public void SelectionChange(int s)
    {
        OpenText(-1);
        current = (Selections)s;
    }
    // Update is called once per frame
    void Update()
    {
        OptionUpdate();
        bool sel = true;
        for (int i = 0; i < 5; i++)
        {
            options[i].SetActive(opened[i]);
            if (options[i].activeSelf)
            {
                sel = false;
            }
        }
        optionReturn.SetActive(!sel);
        hangarReturn.SetActive(sel);
        partSelection.SetActive(sel);

        TextUpdate();
    }
}
