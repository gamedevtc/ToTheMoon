using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PlayerNameSave : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] bool isSelf;
    [SerializeField] string playerName;
    [SerializeField] string fileName = "PlayerNameSave.txt";
    [SerializeField] string defaultName = "Player#";
    [SerializeField] int defaultRangeMin = 100000;
    [SerializeField] int defaultRangeMax = 999999;

    private void Start()
    {
        if (isSelf)
        {
            inputField = GetComponent<TMP_InputField>();
        }
        if (!File.Exists(fileName))
        {
            createDefaultName();
            saveName();
        }

        readName();
        if (inputField)
        {
            inputField.text = playerName;
        }

        if (GameManagerBase.Instance)
        {
            if (GameManagerBase.Instance.isMulti())
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
            }
        }
    }

    public void OnEndEdit()
    {
        playerName = inputField.text;
        saveName();
        if (GameManagerBase.Instance)
        {
            if (GameManagerBase.Instance.isMulti())
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
            }
        }
    }

    void createDefaultName()
    {
        string name = defaultName;
        int playerNum = Random.Range(defaultRangeMin, defaultRangeMax);
        name += playerNum.ToString();
        playerName = name;
    }

    void saveName()
    {
        var writer = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate));
        writer.WriteLine(playerName);
        writer.Close();
    }

    void readName()
    {
        var reader = new StreamReader(File.Open(fileName, FileMode.Open));
        playerName = reader.ReadLine();
        reader.Close();
    }

    public string getPlayerName()
    {
        if (playerName != PhotonNetwork.LocalPlayer.NickName)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
        }
        return PhotonNetwork.LocalPlayer.NickName;
    }
}
