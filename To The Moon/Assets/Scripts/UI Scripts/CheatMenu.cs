using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
    [SerializeField] GameObject cheatMenu;
    [SerializeField] GameObject cheatList;
    [SerializeField] Player4Base player;
    [SerializeField] ShopAttachment dummy;

    [SerializeField] GameObject FPS;
    

    bool dumCheat;
    //[SerializeField] MultiplayerMove m_player;
    Player4Base.HUDData data = new Player4Base.HUDData();
    Text cheatText;
    // Start is called before the first frame update
    void Start()
    {
        //if (GameManagerBase.Instance.isMulti())
        //{
        //    data = new Player4Base.HUDData();
        //    data.currBoost = m_player.GetBoost();
        //    data.maxBoost = m_player.GetMaxBoost();
        //    data.currHealth = m_player.GetHealth();
        //    data.maxHealth = m_player.GetMaxHealth();
        //    data.currUlt = m_player.GetUltCharge();
        //    data.maxUlt = m_player.GetMaxUlt();
        //    data.GodMode = false;
        //    data.InfiniteBoost = false;
        //    data.cheatMenuIsOpen = false;

        //}
        //else
        //{
        if (player)
        {
            player.updateHUD(out data);
        }
        else
        {
            dumCheat = dummy.IsCheat();
        }
        cheatMenu.SetActive(false);
        cheatText = cheatList.GetComponent<Text>();
    }

    string CheckCheat(bool c)
    {
        if (c)
        {
            return "ON";
        }
        else
        {
            return "OFF";
        }
    }

    void Update()
    {
        //if (GameManagerBase.Instance.isMulti())
        //{
        //    data.currBoost = m_player.GetBoost();
        //    data.maxBoost = m_player.GetMaxBoost();
        //    data.currHealth = m_player.GetHealth();
        //    data.maxHealth = m_player.GetMaxHealth();
        //    data.currUlt = m_player.GetUltCharge();
        //    data.maxUlt = m_player.GetMaxUlt();
        //    data.GodMode = false;
        //    data.InfiniteBoost = false;
        //    data.cheatMenuIsOpen = false;

        //}
        //else
        //{
        if (player)
        {
            player.updateHUD(out data);
            //check to see if cheat menu should be active or not
            cheatMenu.SetActive(data.cheatMenuIsOpen);
            //lay out text of each cheat
            cheatText.text = "1- God Mode: ";
            cheatText.text += CheckCheat(data.GodMode) + "\n";
            cheatText.text += "2- Inf. Boost: ";
            cheatText.text += CheckCheat(data.InfiniteBoost) + "\n";
            cheatText.text += "3- Next Wave\n";
            cheatText.text += "4- Damage Player\n";
            cheatText.text += "5- Fill Ultimate\n";
            cheatText.text += "6- Disable escape check: " + CheckCheat(data.canEscape) + "\n";
            cheatText.text += "7- Show AI Pathing: " + CheckCheat(GameManagerBase.Instance.getDebug_showAICollision()) + "\n";
            cheatText.text += "8- Show AI States: " + CheckCheat(GameManagerBase.Instance.getDebug_showAIStates()) + "\n";
            cheatText.text += "9- Kill Wave\n";
            cheatText.text += "0- Show Coliders\n";
            float _fps;
            _fps = 1 / Time.unscaledDeltaTime;
          //  FPS.GetComponent<Text>().text = "FPS: "+ _fps.ToString("F0");
            //show if cheat is active or not through an "ON" or "OFF" text
            return;
        }
        else
        {
            dumCheat = dummy.IsCheat();
            cheatMenu.SetActive(dumCheat);
            cheatText.text = "1- Give 500 credits";
        }
    }
}
