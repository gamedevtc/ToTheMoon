using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Customization : MonoBehaviour
{
    [SerializeField] private GameObject weapon1;
    [SerializeField] private GameObject weapon2;
    [SerializeField] private GameObject primaryWeaponPos;
    [SerializeField] private GameObject secondaryWeaponPos;
    [SerializeField] private int primaryWeapon = 0;
    [SerializeField] private int secondaryWeapon = 0;
    private GameObject placeHoldPrim;
    private GameObject placeHoldSecond;

    private void Start()
    {
        readData();
        if (primaryWeapon == 0)
        {
            placeHoldPrim = Instantiate(weapon1, primaryWeaponPos.transform);
        }
        else
        {
            placeHoldPrim = Instantiate(weapon2, primaryWeaponPos.transform);
        }

        if (secondaryWeapon == 0)
        {
            placeHoldSecond = Instantiate(weapon1, secondaryWeaponPos.transform);
        }
        else
        {
            placeHoldSecond = Instantiate(weapon2, secondaryWeaponPos.transform);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            saveData();
        }
    }

    public void primaryChange()
    {
        if (primaryWeapon == 1)
        {
            primaryWeapon = 0;
            Destroy(placeHoldPrim);
            placeHoldPrim = Instantiate(weapon1, primaryWeaponPos.transform);
        }
        else
        {
            primaryWeapon++;
            Destroy(placeHoldPrim);
            placeHoldPrim = Instantiate(weapon2, primaryWeaponPos.transform);
        }

        saveData();
    }

    public void secondaryChange()
    {
        if (secondaryWeapon == 1)
        {
            secondaryWeapon = 0;
            Destroy(placeHoldSecond);
            placeHoldSecond = Instantiate(weapon1, secondaryWeaponPos.transform);
        }
        else
        {
            secondaryWeapon++;
            Destroy(placeHoldSecond);
            placeHoldSecond = Instantiate(weapon2, secondaryWeaponPos.transform);
        }

        saveData();
    }

    public void saveData()
    {
        var writer = new StreamWriter(File.Open("test.txt", FileMode.OpenOrCreate));
        writer.WriteLine(primaryWeapon);
        writer.WriteLine(secondaryWeapon);
        writer.Close();
       
    }

    public void readData() 
    {
        var reader = new StreamReader(File.Open("test.txt", FileMode.Open));
        primaryWeapon = int.Parse(reader.ReadLine());
        secondaryWeapon = int.Parse(reader.ReadLine());
        reader.Close();

    }
}
