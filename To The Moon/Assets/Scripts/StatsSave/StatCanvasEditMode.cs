using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatCanvasEditMode : MonoBehaviour
{
    [SerializeField] Text StatTextBlock;

    [SerializeField]string favBody;
    [SerializeField]string favPrim;
    [SerializeField]string favSec;
    [SerializeField]string favUlt;
    void Start()
    {
        updateTextBlock();
    }


    void setFavs()
    {
        switch (StatSaveManager.Instance.saveStatData.favoriteBody)
        {
            case Ship.BodyOption.Body1:
                favBody = "Body 1";
                break;
            case Ship.BodyOption.Body2:
                favBody = "Body 2";
                break;
            case Ship.BodyOption.Body3:
                favBody = "Body 3";
                break;
            case Ship.BodyOption.Body4:
                favBody = "Body 4";
                break;
        }

        switch (StatSaveManager.Instance.saveStatData.favoritePrim)
        {
            case Ship.PrimaryOption.FusionBlaster:
                favPrim = "Fusion Blaster";
                break;
            case Ship.PrimaryOption.FusionBlasterMkII:
                favPrim = "Fusion Blaster MKII";
                break;
            case Ship.PrimaryOption.PlasmaAccelerator:
                favPrim = "Plasma Accelerator";
                break;
            case Ship.PrimaryOption.BurstLaser:
                favPrim = "Burst Laser";
                break;
        }

        switch (StatSaveManager.Instance.saveStatData.favoriteSec)
        {
            case Ship.SecondaryOption.Missiles:
                favSec = "Missiles";
                break;
            case Ship.SecondaryOption.ThermalMissiles:
                favSec = "Thermal Missiles";
                break;
            case Ship.SecondaryOption.PositronBallista:
                favSec = "Positron Ballista";
                break;
            case Ship.SecondaryOption.PulseCannon:
                favSec = "Pulse Cannon";
                break;
            case Ship.SecondaryOption.FusionBlasterHeavy:
                favSec = "Fusion Blaster Heavy";
                break;
            case Ship.SecondaryOption.FusionBlasterMkIIHeavy:
                favSec = "Fusion Blaster Heavy MKII";
                break;
            case Ship.SecondaryOption.PlasmaAcceleratorHeavy:
                favSec = "Plasma Accelerator Heavy";
                break;
            case Ship.SecondaryOption.BurstLaserHeavy:
                favSec = "Burst Laser Heavy";
                break;
        }

        switch (StatSaveManager.Instance.saveStatData.favoriteUlt)
        {
            case Ship.UltimateOption.MineLauncher:
                favUlt = "Mine Launcher";
                break;
            case Ship.UltimateOption.MissileSilo:
                favUlt = "Missile Silo";
                break;
            case Ship.UltimateOption.DevastatorBeam:
                favUlt = "Devastator Deam";
                break;
            case Ship.UltimateOption.EMP:
                favUlt = "EMP";
                break;
        }
    }
    // Update is called once per frame
    public void updateTextBlock()
    {
        setFavs();
        StatTextBlock.text = "Time Played: ";
        StatTextBlock.text += StatSaveManager.Instance.saveStatData.TimePlayed + "\n";
        StatTextBlock.text += "Kills: ";
        StatTextBlock.text += StatSaveManager.Instance.saveStatData.Kills + "\n";
        StatTextBlock.text += "Deaths: ";
        StatTextBlock.text += StatSaveManager.Instance.saveStatData.Deaths + "\n";
        StatTextBlock.text += "Wins: ";
        StatTextBlock.text += StatSaveManager.Instance.saveStatData.Wins + "\n";
        StatTextBlock.text += "Losses: ";
        StatTextBlock.text += StatSaveManager.Instance.saveStatData.Lose + "\n";
        StatTextBlock.text += "Favorite Body: ";
        StatTextBlock.text += favBody + "\n";
        StatTextBlock.text += "Favorite Primary: ";
        StatTextBlock.text += favPrim + "\n";
        StatTextBlock.text += "Favorite Secondary: ";
        StatTextBlock.text += favSec + "\n";
        StatTextBlock.text += "Favorite Ultimate: ";
        StatTextBlock.text += favUlt + "\n";

    }
}
