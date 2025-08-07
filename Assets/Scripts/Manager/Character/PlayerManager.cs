using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public int UpgradeKey;
    public string EngName;
    public string KrName;
    public string SpritePath;
    public string PrefabPath;
    public string Layer;
    public string AttackType;
    public string ProjectilePath;
    public int SpawnLine;
    public int UpgradeCost;
    public int MaxLevel;
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

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (rowData[i] == "") continue;
            if (colData.Length <= 1) continue;

            PlayerData data;

            data.Key = int.Parse(colData[0]);
            data.UpgradeKey = int.Parse(colData[1]);
            data.EngName = colData[2];
            data.KrName = colData[3];
            data.SpritePath = colData[4];
            data.PrefabPath = colData[5];
            data.Layer = colData[6];
            data.AttackType = colData[7];
            data.ProjectilePath = colData[8];
            data.SpawnLine = int.Parse(colData[9]);
            data.UpgradeCost = int.Parse(colData[10]);
            data.MaxLevel = int.Parse(colData[11]);

            if (!_isPlayerFirstkey)
            {
                _isPlayerFirstkey = true;
                _playerStartKey = data.Key;
            }

            _playerDatas.Add(data.Key, data);
            _playerCount++;
        }
    }
}