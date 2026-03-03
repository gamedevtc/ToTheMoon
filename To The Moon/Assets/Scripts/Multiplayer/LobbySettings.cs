using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbySettings : MonoBehaviour
{
    [SerializeField] GameObject mainLobby;
    [SerializeField] LevelSettings levelSettings;
    [SerializeField] Button Coop;
    [SerializeField] Button ffa;
    [SerializeField] Slider AsterSlider;
    [SerializeField] Slider EnemiesSlider;
    [SerializeField] InputField asterCount;
    [SerializeField] Text enemiesCount;
    [SerializeField] Dropdown background;

    public bool isCoop = false;
    int asteroidsCount = 2;
    int enemyCount = 2;


    // Start is called before the first frame update
    void Start()
    {
        if (Coop != null || ffa != null)
        {

        Coop.interactable = true;
        ffa.interactable = true;
        }
        //Coop.image.color = Color.cyan;
    }

    private void Update()
    {
       
        asteroidsCount = (int)AsterSlider.value;
        enemyCount = (int)EnemiesSlider.value;

        asterCount.text = asteroidsCount.ToString();
        enemiesCount.text = enemyCount.ToString();
    }


    public void onCOOPClick()
    {
        Coop.interactable = false;
        ffa.interactable = true;
        isCoop = true;
        Coop.image.color = Color.cyan;
        ffa.image.color = Color.white;
        

    }

    public void onFFAClick()
    {
        Coop.interactable = true;
        ffa.interactable = false;
        isCoop = false;
        Coop.image.color = Color.white;
        ffa.image.color = Color.cyan;
    }

    [PunRPC]
    public void RPC_LevelSettings(int val)
    {
        //levelSettings.numOfEnemiesCustom[0] = enemyCount;
        switch (val)
        {
            case 0:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.DarkSpace;
                break;
            case 1:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.EarthSpace;
                break;
            case 2:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.GreenGalaxies;
                break;
            case 3:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.NaturalGalaxies;
                break;
            case 4:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.OrangeBlackHole;
                break;
            case 5:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.PinkGalaxies;
                break;
            case 6:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.PurpleSaturn;
                break;
            default:
                break;
        }

        levelSettings.asteroidCountCustom = asteroidsCount;
    }

    public void onReturnClick()
    {
        
        
        //levelSettings.numOfEnemiesCustom[0] = enemyCount;
        switch (background.value)
        {
            case 0:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.DarkSpace;
                break;
            case 1:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.EarthSpace;
                break;
            case 2:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.GreenGalaxies;
                break;
            case 3:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.NaturalGalaxies;
                break;
            case 4:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.OrangeBlackHole;
                break;
            case 5:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.PinkGalaxies;
                break;
            case 6:
                levelSettings.backgroundSettingsCustom = LevelInfo.levelBackground.PurpleSaturn;
                break;
            default:
                break;
        }





        levelSettings.asteroidCountCustom = asteroidsCount;

        levelSettings.SetLevelData(6);
        
        //foreach (Player player in PhotonNetwork.PlayerList)
        //{
        //    Hashtable hash = player.CustomProperties;
        //    hash["Skybox"] = background.value;
        //    player.SetCustomProperties(hash);
        //}

        //lmanage.RPC("RPC_LevelSettings", RpcTarget.All, background.value);

        mainLobby.SetActive(true);
        this.gameObject.SetActive(false);


    }





}
