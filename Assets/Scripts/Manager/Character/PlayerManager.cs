using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public int UpgradeKey;
    public int SpawnLine;
    public int UpgradeCost;
    public int MaxLevel;
    public int[] ProjectilePool;
    public string EngName;
    public string KrName;
    public string CharacterSpritePath;
    public string CharacterPrefabPath;
    public string Layer;
    public string AttackType;
    public string ProjectilePath;
    public string Target;
    public string[] ProjectileSpritePath;
    public bool[] IsRange;
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
            data.IsRange = new bool[3];
            data.ProjectilePool = new int[3];
            data.ProjectileSpritePath = new string[3];

            data.Key = int.Parse(colData[0]);
            data.UpgradeKey = int.Parse(colData[1]);
            data.EngName = colData[2];
            data.KrName = colData[3];
            data.CharacterSpritePath = colData[4];
            data.CharacterPrefabPath = colData[5];
            data.ProjectilePath = colData[6];

            data.ProjectileSpritePath[0] = colData[7];
            data.ProjectileSpritePath[1] = colData[8];
            data.ProjectileSpritePath[2] = colData[9];

            data.IsRange[0] = bool.Parse(colData[10]);
            data.IsRange[1] = bool.Parse(colData[11]);
            data.IsRange[2] = bool.Parse(colData[12]);

            data.ProjectilePool[0] = int.Parse(colData[13]);
            data.ProjectilePool[1] = int.Parse(colData[14]);
            data.ProjectilePool[2] = int.Parse(colData[15]);

            data.Layer = colData[16];
            data.AttackType = colData[17];
            data.Target = colData[18];
            data.SpawnLine = int.Parse(colData[19]);
            data.UpgradeCost = int.Parse(colData[20]);
            data.MaxLevel = int.Parse(colData[21]);

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