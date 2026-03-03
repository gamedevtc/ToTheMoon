using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestShop : MonoBehaviour
{
    [SerializeField] ShopManager shop;
    [SerializeField] SaveData save;
    Ship.playerShipSettings equipped = new Ship.playerShipSettings();
    [SerializeField] Ship.ColorOption activeColor;
    [SerializeField] Ship.BodyOption activeBody;
    [SerializeField] Ship.PrimaryOption activePrimary;
    [SerializeField] Ship.SecondaryOption activeSecondary;
    [SerializeField] Ship.UltimateOption activeUltimate;

    [SerializeField]private bool[] cPurch = new bool[8];
    [SerializeField]private bool[] bPurch = new bool[4];
    [SerializeField]private bool[] pPurch = new bool[4];
    [SerializeField]private bool[] sPurch = new bool[8];
    [SerializeField]private bool[] uPurch = new bool[4];

    private bool[] cEq = new bool[8];
    private bool[] bEq = new bool[4];
    private bool[] pEq = new bool[4];
    private bool[] sEq = new bool[8];
    private bool[] uEq = new bool[4];

    [SerializeField] GameObject[] cpButton = new GameObject[8];
    [SerializeField] GameObject[] bpButton = new GameObject[4];
    [SerializeField] GameObject[] ppButton = new GameObject[4];
    [SerializeField] GameObject[] spButton = new GameObject[8];
    [SerializeField] GameObject[] upButton = new GameObject[4];

    [SerializeField] GameObject[] ceButton = new GameObject[8];
    [SerializeField] GameObject[] beButton = new GameObject[4];
    [SerializeField] GameObject[] peButton = new GameObject[4];
    [SerializeField] GameObject[] seButton = new GameObject[8];
    [SerializeField] GameObject[] ueButton = new GameObject[4];

    [SerializeField] GameObject[] cEquipped = new GameObject[8];
    [SerializeField] GameObject[] bEquipped = new GameObject[4];
    [SerializeField] GameObject[] pEquipped = new GameObject[4];
    [SerializeField] GameObject[] sEquipped = new GameObject[8];
    [SerializeField] GameObject[] uEquipped = new GameObject[4];

    [SerializeField] ModifyMenu m;

    [SerializeField] GameObject creditCounter;
    Text credit;

    // Start is called before the first frame update
    void Start()
    {
        save.getActiveSettings(out equipped);
        credit = creditCounter.GetComponent<Text>();
        for (int i = 0; i < 8; i++)
        {
            if (i < cPurch.Length) cPurch[i] = false;
            if (i < bPurch.Length) bPurch[i] = false;
            if (i < pPurch.Length) pPurch[i] = false;
            if (i < sPurch.Length) sPurch[i] = false;
            if (i < uPurch.Length) uPurch[i] = false;

            if (i < cEq.Length) cEq[i] = false;
            if (i < bEq.Length) bEq[i] = false;
            if (i < pEq.Length) pEq[i] = false;
            if (i < sEq.Length) sEq[i] = false;
            if (i < uEq.Length) uEq[i] = false;

            if (i < ceButton.Length) ceButton[i].SetActive(false);
            if (i < cEquipped.Length) cEquipped[i].SetActive(false);
            if (i < beButton.Length) beButton[i].SetActive(false);
            if (i < bEquipped.Length) bEquipped[i].SetActive(false);
            if (i < peButton.Length) peButton[i].SetActive(false);
            if (i < pEquipped.Length) pEquipped[i].SetActive(false);
            if (i < seButton.Length) seButton[i].SetActive(false);
            if (i < sEquipped.Length) sEquipped[i].SetActive(false);
            if (i < ueButton.Length) ueButton[i].SetActive(false);
            if (i < uEquipped.Length) uEquipped[i].SetActive(false);
        }
        for (int i = 0; i < 28; i++)
        {
            if (0 <= i && i < 8)
            {
                cPurch[i] = shop.CheckUnlocked(i);
            }
            if (8 <= i && i < 12)
            {
                bPurch[i - 8] = shop.CheckUnlocked(i);
            }
            if (12 <= i && i < 16)
            {
                pPurch[i - 12] = shop.CheckUnlocked(i);
            }
            if (16 <= i && i < 24)
            {
                sPurch[i - 16] = shop.CheckUnlocked(i);
            }
            if (24 <= i && i < 28)
            {
                uPurch[i - 24] = shop.CheckUnlocked(i);
            }
            if(i == (int)equipped.activeColor)
            {
                cEq[i] = true;
            }
            if(i == (int)equipped.activeBody)
            {
                bEq[i - 8] = true;
            }
            if(i == (int)equipped.activePrimary)
            {
                pEq[i - 12] = true;
            }
            if(i == (int)equipped.activeSecondary)
            {
                sEq[i - 16] = true;
            }
            if(i == (int)equipped.activeUltimate)
            {
                uEq[i - 24] = true;
            }
        }

        activeColor = equipped.activeColor;
        activeBody = equipped.activeBody;
        activePrimary = equipped.activePrimary;
        activeSecondary = equipped.activeSecondary;
        activeUltimate = equipped.activeUltimate;
    }

    void CheckPurchase()
    {
        for (int i = 0; i < 8; i++)
        {
            if (i < cpButton.Length) cpButton[i].SetActive(!cPurch[i]);
            if (i < bpButton.Length) bpButton[i].SetActive(!bPurch[i]);
            if (i < ppButton.Length) ppButton[i].SetActive(!pPurch[i]);
            if (i < spButton.Length) spButton[i].SetActive(!sPurch[i]);
            if (i < upButton.Length) upButton[i].SetActive(!uPurch[i]);
        }
    }

    void CheckEquipped()
    {
        for (int i = 0; i < 8; i++)
        {
            if (cPurch[i])
            {
                ceButton[i].SetActive(!cEq[i]);
                cEquipped[i].SetActive(cEq[i]);
            }

            if (sPurch[i])
            {
                seButton[i].SetActive(!sEq[i]);
                sEquipped[i].SetActive(sEq[i]);
            }

            if (i < 4)
            {
                if (bPurch[i])
                {
                    beButton[i].SetActive(!bEq[i]);
                    bEquipped[i].SetActive(bEq[i]);
                }

                if (pPurch[i])
                {
                    peButton[i].SetActive(!pEq[i]);
                    pEquipped[i].SetActive(pEq[i]);
                }

                if (uPurch[i])
                {
                    ueButton[i].SetActive(!uEq[i]);
                    uEquipped[i].SetActive(uEq[i]);
                }
            }
        }
    }

    public void MakePurchase(int p)
    {
        int s = m.GetSelection();
        for (int i = 0; i < 28; i++)
        {
            if (p == i)
            {
                if (0 <= i && i < 8)
                {
                    cPurch[i] = shop.CheckUnlocked(i);
                }
                if (8 <= i && i < 12)
                {
                    bPurch[i - 8] = shop.CheckUnlocked(i);
                }
                if (12 <= i && i < 16)
                {
                    pPurch[i - 12] = shop.CheckUnlocked(i);
                }
                if (16 <= i && i < 24)
                {
                    sPurch[i - 16] = shop.CheckUnlocked(i);
                }
                if (24 <= i && i < 28)
                {
                    uPurch[i - 24] = shop.CheckUnlocked(i);
                }
            }
        }
    }

    public void EquipItem(int e)
    {
        int s = m.GetSelection();
        for (int i = 0; i < 8; i++)
        {
            switch (s)
            {
                case 0:
                    cEq[i] = false;
                    break;
                case 1:
                    if (i < 4) bEq[i] = false;
                    break;
                case 2:
                    if (i < 4) pEq[i] = false;
                    break;
                case 3:
                    sEq[i] = false;
                    break;
                case 4:
                    if (i < 4) uEq[i] = false;
                    break;
            }
            if (e == i)
            {
                switch (s)
                {
                    case 0:
                        cEq[i] = true;
                        break;
                    case 1:
                        if (i < 4) bEq[i] = true;
                        break;
                    case 2:
                        if (i < 4) pEq[i] = true;
                        break;
                    case 3:
                        sEq[i] = true;
                        break;
                    case 4:
                        if (i < 4) uEq[i] = true;
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int c = save.GetCurrency(); ;
        credit.text = $"{c}";
        CheckPurchase();
        CheckEquipped();
    }
}
