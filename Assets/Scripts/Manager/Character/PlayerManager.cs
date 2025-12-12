using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public int AtkKey;
    public int SkillUpgradeKey;
    public int StatUpgradeKey;
    public int SpawnLine;
    public int UpgradeCost;
    public int MaxLevel;
    public string EngName;
    public string KrName;
    public string CharacterSpritePath;
    public string CharacterPrefabPath;
    public string Layer;
    public string AtkType;
    public string Target;
    public bool IsDeployed;
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
            data.StatUpgradeKey = int.Parse(colData[1]);
            data.AtkKey = int.Parse(colData[2]);
            data.SkillUpgradeKey = int.Parse(colData[3]);
            data.EngName = colData[4];
            data.KrName = colData[5];
            data.CharacterSpritePath = colData[6];
            data.CharacterPrefabPath = colData[7];
            data.Layer = colData[8];
            data.AtkType = colData[9];
            data.Target = colData[10];
            data.SpawnLine = int.Parse(colData[11]);
            data.UpgradeCost = int.Parse(colData[12]);
            data.MaxLevel = int.Parse(colData[13]);
            data.IsDeployed = false;

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