using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public int CharacterKey;
    public int UpgradeKey;
    public string SpritePath;
    public int CardUpgradeCost;
    public int MaxLevel;
    public float Mp;
    public float SkillRate;
    public float Ultimate;
    public float UltCoolTime;
    public bool CanUseUlt;
}

public class PlayerManager : Singleton<PlayerManager>
{
    private Dictionary<int, PlayerData> _playerDatas = new Dictionary<int, PlayerData>();
    
    private int _playerStartKey;
    public int PlayerStartKey
    {
        get { return _playerStartKey; }
    }

    private int _playerCount = 0;
    public int PlayerCount
    {
        get { return _playerCount; }
    }

    private bool _isPlayerFirstkey = false;

    private void Awake()
    {
        LoadPlayerDatas();
    }

    public PlayerData GetPlayerData(int key)
    {
        return _playerDatas[key]; 
    }

    private void LoadPlayerDatas()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayableTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (rowData[i] == "") continue;
            if (colData.Length <= 1) continue;

            PlayerData data;

            data.Key = int.Parse(colData[0]);
            data.CharacterKey = int.Parse(colData[1]);
            data.UpgradeKey = int.Parse(colData[2]);
            data.SpritePath = colData[3];
            data.CardUpgradeCost = int.Parse(colData[4]);
            data.MaxLevel = int.Parse(colData[5]);
            data.Mp = float.Parse(colData[6]);
            data.SkillRate = float.Parse(colData[7]);
            data.Ultimate = float.Parse(colData[8]);
            data.UltCoolTime = float.Parse(colData[9]);
            data.CanUseUlt = bool.Parse(colData[10]);

            if(!_isPlayerFirstkey)
            {
                _isPlayerFirstkey = true;
                _playerStartKey = data.Key;
            }

            _playerDatas.Add(data.Key, data);
            _playerCount++;
        }
    }
}