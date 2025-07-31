using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerData
{
    public int Key;
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

public class PlayerManager : MonoBehaviour
{
    private Dictionary<int, PlayerData> _playerDatas = new Dictionary<int, PlayerData>();

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
            data.UpgradeKey = int.Parse(colData[1]);
            data.SpritePath = colData[2];
            data.CardUpgradeCost = int.Parse(colData[3]);
            data.MaxLevel = int.Parse(colData[4]);
            data.Mp = float.Parse(colData[5]);
            data.SkillRate = float.Parse(colData[6]);
            data.Ultimate = float.Parse(colData[7]);
            data.UltCoolTime = float.Parse(colData[8]);
            data.CanUseUlt = bool.Parse(colData[9]);

            _playerDatas.Add(data.Key, data);
        }
    }
}