using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Photon.Pun;


public class HealthBar : MonoBehaviour
{
    #region UI bars
    [SerializeField] private Slider healthSliderL;
    [SerializeField] private Slider healthSliderR;
    [SerializeField] private Image healthFillL;
    [SerializeField] private Image healthFillR;
    [SerializeField] private Slider boostSlider;
    [SerializeField] private Image boostFill;
    [SerializeField] private Slider ultSlider;
    [SerializeField] private Image ultFill;
    #endregion

    #region reticle images and UI bar color shifting
    [SerializeField] private Image reticleBorder;
    [SerializeField] private Image reticleDot;
    [SerializeField] private Image reticleCircle;
    [SerializeField] private Image reticleMarkers;
    [SerializeField] private Color ultReady;
    [SerializeField] private Color ultCharging;
    [SerializeField] private Color boostVisible;
    bool inUse;
    float shiftTime;
    #endregion
    #region Enemy Counter and Timer
    AIManager.HUDWaveData waveData = new AIManager.HUDWaveData();
    [SerializeField] GameObject enemyText;
    Text enemyCounter;
    #endregion
    #region OOB Warning
    [SerializeField] GameObject bounds;
    [SerializeField] Text boundWarning;
    #endregion
    #region Low Health Indicator
    [SerializeField] GameObject lowHealth;
    [SerializeField] GameObject lowBorder;
    Image borderImage;
    float lowTime =0.0f;
    Color borderColor;
    #endregion
    #region Damage Indicator
    [SerializeField] GameObject hitEffect;
    bool hit=false;
    float effectTime=0.0f;
    int prevHealth;
    #endregion
    #region Player data and GameManager
    [SerializeField] GameObject enemyContainer;
    [SerializeField] Player4Base player;
    Player4Base.HUDData data = new Player4Base.HUDData();
    #endregion

    // [SerializeField] PhotonView view;

    void Start()
    {
        bounds.SetActive(false);
        lowHealth.SetActive(false);
        borderImage = lowBorder.GetComponent<Image>();
        borderColor = borderImage.color;
        borderColor.a = 0.0f;
        borderImage.color = borderColor;
        lowBorder.SetActive(false);
        hitEffect.SetActive(false);
        
        player.updateHUD(out data);
        
        //healthBar.
        SetMaxHealth((int)data.maxHealth);
        //boostBar.
        SetMaxBoost(data.maxBoost);
        ultSlider.maxValue = 100;
        ultSlider.value = 0;
        enemyCounter = enemyText.GetComponent<Text>();
        //boundWarning = boundText.GetComponent<Text>();
        //if(multiplayer) set timer Container active;
        boostVisible = boostFill.color;
        waveData = new AIManager.HUDWaveData();
        data = new Player4Base.HUDData();
    }

    void SetMaxHealth(int health)
    {
        healthSliderL.maxValue = health;
        healthSliderR.maxValue = health;
    }
    void SetHealth(int health)
    {
        healthSliderL.value = health;
        healthSliderR.value = health;
    }

    void SetMaxBoost(float boost)
    {
        boostSlider.maxValue = boost;
        boostSlider.value = boost;

    }
    void SetBoost(float boost)
    {
        boostSlider.value = boost;
    }
    void SetUlt(float c, float m)
    {
        float per = c / (float)m;
        if(per>1)
        {
            per = 1;
        }
        ultSlider.value = (int)(per*100);
    }

    int HealthPercent()
    {
        return (int)((healthSliderL.value / healthSliderL.maxValue) * 100);
    }

    // Update is called once per frame
    void Update()
    {
        prevHealth = (int)healthSliderL.value;
        if(data.maxHealth != healthSliderL.maxValue)
        {
            healthSliderL.maxValue = data.maxHealth;
        }
        if (data.maxBoost != boostSlider.maxValue)
        {
            boostSlider.maxValue = data.maxBoost;
        }
        
        player.updateHUD(out data);
       
        //healthBar.
        SetHealth((int)data.currHealth);
        //boostBar.
        SetBoost(data.currBoost);
        //ultBar.
        SetUlt(data.currUlt, data.maxUlt);
        if(boostSlider.value != boostSlider.maxValue)
        {
            inUse = true;
        }
        else
        {
            inUse = false;
        }
        if(inUse && boostVisible.a < 1)
        {
            shiftTime += Time.deltaTime * 5;
            boostVisible.a = shiftTime;
            if(boostVisible.a >1)
            {
                boostVisible.a = 1.0f;
                shiftTime = 1;
            }
            boostFill.color = boostVisible;
        }
        if(!inUse && boostVisible.a > 0)
        {
            shiftTime -= Time.deltaTime * 5;
            boostVisible.a = shiftTime;
            if (boostVisible.a < 0)
            {
                boostVisible.a = 0.0f;
                shiftTime = 0;
            }
            boostFill.color = boostVisible;
        }
        if(ultSlider.value == 100)
        {
            reticleBorder.color = ultReady;
            //reticleDot.color = ultReady;
            //reticleCircle.color = ultReady;
            //reticleMarkers.color = ultReady;
            ultFill.color = ultReady;
        }
        else
        {
            reticleBorder.color = ultCharging;
            //reticleDot.color = ultCharging;
            //reticleCircle.color = ultCharging;
            //reticleMarkers.color = ultCharging;
            ultFill.color = ultCharging;
        }
        if(prevHealth > healthSliderL.value)
        {
            hit = true;
            effectTime = .25f;
        }
        hitEffect.SetActive(hit);
        if(effectTime > 0.0f)
        {
            effectTime -= Time.deltaTime;
        }
        if(effectTime<=0.0f && hit)
        {
            hit = false;
        }
        int p = HealthPercent();
        if (p <= 25 && GameManagerBase.Instance.getState() == GameManagerBase.gameState.Running)
        {
            lowTime += Time.deltaTime;
            lowHealth.SetActive(true);
            lowBorder.SetActive(true);
            borderColor.a = Mathf.Abs(Mathf.Sin(lowTime))/2;
            borderImage.color = borderColor;
        }
        if(p>25)
        {
            if(lowHealth.activeSelf)
            {
                lowHealth.SetActive(false);
                lowBorder.SetActive(false);
                borderColor.a = 0.0f;
                lowTime = 0.0f;
                borderImage.color = borderColor;
            }
        }
        
        if (AIManager.Instance)
        {
            AIManager.Instance.updateHUDData(out waveData);
            if (waveData.currentWave >= waveData.totalWaves)
            {
                enemyCounter.text = "Final Wave\n"; //just to track what wave the player is on 
            }
            else
            {
                enemyCounter.text = "Wave " + waveData.currentWave + "\n"; //just to track what wave the player is on 
            }
            enemyCounter.text += "Enemies Left: " + waveData.currentWaveRemainingEnemies + " / " + waveData.currentWaveTotalEnemies;
        }
        else //this will make the enemy counter become a score tracker for multiplayer
        {
            enemyCounter.text = "Free for All\n";
            enemyCounter.text += "Kills: " + "not enough:";
        }
        
        bounds.SetActive(data.escaping);
        if(bounds.activeSelf)
        {
            boundWarning.text = "WARINING\n Leaving Battlefield\n Termination in \n";
            boundWarning.text += data.escapeTime.ToString("f2");
        }
    }
}
