using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class ShopAttachment : Ship
{
    //[SerializeField] SHOPCHEATMENU
    [SerializeField] bool cheatMenuIsOpen = false;
    [SerializeField] KeyCode cheatMenuKey = KeyCode.BackQuote;
    [SerializeField] KeyCode GiveMoneyKey = KeyCode.Alpha1;

    [SerializeField] ColorOption shopColor;
    [SerializeField] BodyOption shopBody;
    [SerializeField] PrimaryOption shopPrimary;
    [SerializeField] SecondaryOption shopSecondary;
    [SerializeField] UltimateOption shopUltimate;

    bool fade = false;
    bool afade = false;
    float time = 1.5f;
    float aTime = 1.0f;

    private void Update()
    {
        if (Input.GetKeyDown(cheatMenuKey))
        {
            cheatMenuIsOpen = !cheatMenuIsOpen;
        }
        if (cheatMenuIsOpen)
        {
            if (Input.GetKeyDown(GiveMoneyKey))
            {
                setCredits(500);
            }
        }
        if (fade)
        {
            shipColors[(int)shopColor].SetFloat("Timer", time);
            time -= Time.deltaTime;
            if (time <=-0.1f)
            {
                fade = false;
                aTime = 1.0f;
                time = 1.5f;
            }
        }
        if (afade)
        {
            holoMat.SetFloat("Alpha", aTime);
            aTime -= Time.deltaTime;
            if (aTime <= 0)
            {
                HoloOff();
                holoMat.SetFloat("Alpha", 1.0f);

            }
        }
    }

    //send cheat data to cheat menu when available
    public bool IsCheat()
    {
        return cheatMenuIsOpen;
    }

    //Add multiplayer function for saving this data to Photon player custom data or dictionary in gamemanager?
    public void savePhotonPlayerData()
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["Skybox"] = 0;
        hash["Color"] = (int)activeSettings.activeColor;
        hash["Body"] = (int)activeSettings.activeBody;
        hash["Prim"] = (int)activeSettings.activePrimary;
        hash["Sec"] = (int)activeSettings.activeSecondary;
        hash["Ult"] = (int)activeSettings.activeUltimate;

        //hash.Add("Color", (int)activeSettings.activeColor);
        //hash.Add("Body", (int)activeSettings.activeBody);
        //hash.Add("Prim", (int)activeSettings.activePrimary);
        //hash.Add("Sec", (int)activeSettings.activeSecondary);
        //hash.Add("Ult", (int)activeSettings.activeUltimate);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        
    }

    playerShipSettings createSaveStruct()
    {
        playerShipSettings temp;
        temp.activeColor = activeSettings.activeColor;
        temp.activeBody = activeSettings.activeBody;
        temp.activePrimary = activeSettings.activePrimary;
        temp.activeSecondary = activeSettings.activeSecondary;
        temp.activeUltimate = activeSettings.activeUltimate;
        return temp;
    }

    //Preview
    public void previewColor(ColorOption preview)
    {
        shopColor = preview;
        bodyLink.setColor(shipColors[(int)shopColor]);
        PrimaryLink.setColor(shipColors[(int)shopColor]);
        SecondaryLink.setColor(shipColors[(int)shopColor]);
        UltimateLink.setColor(shipColors[(int)shopColor]);
    }
    public void previewBody(BodyOption preview)
    {
        bodyLink.setActiveBody(preview);
        bodyLink.setColor(shipColors[(int)activeSettings.activeColor]);
        fade = true;
        afade = true;
        shopBody = preview;
    }
    public void previewHoloBody(BodyOption preview)
    {
        bodyLink.setHoloBody(preview);
        fade = false;
        afade = false;
        aTime = 1.0f;
        holoMat.SetFloat("Alpha", aTime);
        shopBody = preview;
    }
    public void previewPrimary(PrimaryOption preview)
    {
        PrimaryLink.setActiveWeapons((int)preview);
        PrimaryLink.setColor(shipColors[(int)activeSettings.activeColor]);
        afade = true;
        shopPrimary = preview;
    }
    public void previewHoloPrimary(PrimaryOption preview)
    {
        PrimaryLink.setHoloWeapons((int)preview);
        afade = false;
        aTime = 1.0f;
        holoMat.SetFloat("Alpha", aTime);
        shopPrimary = preview;
    }
    public void previewSecondary(SecondaryOption preview)
    {
        SecondaryLink.setActiveWeapons((int)preview);
        SecondaryLink.setColor(shipColors[(int)activeSettings.activeColor]);
        afade = true;
        shopSecondary = preview;
    }
    public void previewHoloSecondary(SecondaryOption preview)
    {
        SecondaryLink.setHoloWeapons((int)preview);
        afade = false;
        aTime = 1.0f;
        holoMat.SetFloat("Alpha", aTime);
        shopSecondary = preview;
    }
    public void previewUltimate(UltimateOption preview)
    {
        UltimateLink.setActiveWeapons((int)preview);
        UltimateLink.setColor(shipColors[(int)activeSettings.activeColor]);
        afade = true;
        shopUltimate = preview;
    }
    public void previewHoloUltimate(UltimateOption preview)
    {
        UltimateLink.setHoloWeapons((int)preview);
        fade = false;
        afade = false;
        aTime = 1.0f;
        holoMat.SetFloat("Alpha", aTime);
        shopUltimate = preview;
    }

    public void HoloOff()
    {
        bodyLink.HoloOff();
        PrimaryLink.HoloOff();
        SecondaryLink.HoloOff();
        UltimateLink.HoloOff();
    }

    //Confirms
    public void confirmColor()
    {
        activeSettings.activeColor = shopColor;
        saveScript.saveShipPrefs(createSaveStruct());
    }
    public void confirmBody()
    {
        activeSettings.activeBody = shopBody;
        saveScript.saveShipPrefs(createSaveStruct());
    }
    public void confirmPrimary()
    {
        activeSettings.activePrimary = shopPrimary;
        saveScript.saveShipPrefs(createSaveStruct());
    }
    public void confirmSecondary()
    {
        activeSettings.activeSecondary = shopSecondary;
        saveScript.saveShipPrefs(createSaveStruct());
    }
    public void confirmUltimate()
    {
        activeSettings.activeUltimate = shopUltimate;
        saveScript.saveShipPrefs(createSaveStruct());
    }

    //reset
    public void resetToActiveColor()
    {
        if (shopColor != activeSettings.activeColor)
        {
            previewColor(activeSettings.activeColor);
        }
    }
    public void resetToActiveBody()
    {
        bodyLink.setActiveBody(activeSettings.activeBody);
    }
    public void resetToActivePrimary()
    {
        PrimaryLink.setActiveWeapons((int)activeSettings.activePrimary);
    }
    public void resetToActiveSecondary()
    {
        SecondaryLink.setActiveWeapons((int)activeSettings.activeSecondary);
    }
    public void resetToActiveUltimate()
    {
        UltimateLink.setActiveWeapons((int)activeSettings.activeUltimate);
    }

    public void resetAllToActiveParts()
    {
        resetToActiveColor();
        resetToActiveBody();
        resetToActivePrimary();
        resetToActiveSecondary();
        resetToActiveUltimate();
        saveScript.saveShipPrefs(activeSettings);
    }
}
