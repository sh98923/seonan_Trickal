using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public string Name;
    public string PrefabPath;
    public string SpritePath;
    public string SpawnLine;
    public float Hp;
    public float Atk;
    public float Mp;
    public float CriRate;
    public float Range;
    public float Coefficient;
    public float LimitLevel;
    public float BaseSkill;
    public float Ultimate;
}

public struct MonsterData
{
    public int Key;
    public string Name;
    public string PrefabPath;
    public string SpawnLine;
    public float Hp;
    public float Atk;
    public float Mp;
    public float CriRate;
    public float Range;
}

public class CharacterManager : Singleton<CharacterManager>
{
    private Dictionary<int, PlayerData> _playerDatas = new Dictionary<int, PlayerData>();
    private Dictionary<int, MonsterData> _monsterDatas = new Dictionary<int, MonsterData>();

    private readonly int _firstPlayerKey = 1;
    private int _playerStartKey;
    public int PlayerStartKey
    {
        get { return _playerStartKey; }
    }

    private readonly int _firstMonsterKey = 1;
    private int _monsterStartKey;
    public int MonsterStartKey
    {
        get { return _monsterStartKey; }
    }


    private void Awake()
    {
        LoadPlayerData();
        LoadMonsterData();
    }

    public PlayerData GetPlayerData(int key)
    {
        return _playerDatas[key];
    }

    public int GetPlayerDataCount()
    {
        return _playerDatas.Count;
    }

    public MonsterData GetMonsterData(int key)
    {
        return _monsterDatas[key];
    }

    public int GetMonsterDataCount()
    {
        return _monsterDatas.Count;
    }

    private void LoadPlayerData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayableTable");
        
        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1) return;
            if (rowData[i] == "") return;

            PlayerData data;
            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.PrefabPath = colData[2];
            data.SpritePath = colData[3];
            data.SpawnLine = colData[4];
            data.Hp = float.Parse(colData[5]);
            data.Atk = float.Parse(colData[6]);
            data.Mp = float.Parse(colData[7]);
            data.CriRate = float.Parse(colData[8]);
            data.Range = float.Parse(colData[9]);
            data.Coefficient = float.Parse(colData[10]);
            data.LimitLevel = float.Parse(colData[11]);
            data.BaseSkill = float.Parse(colData[12]);
            data.Ultimate = float.Parse(colData[13]);
            
            if (_firstPlayerKey == i)
            {
                _playerStartKey = data.Key;
            }

            _playerDatas.Add(data.Key, data);
        }
    }

    private void LoadMonsterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/MonsterTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1) return;
            if (rowData[i] == "") return;

            MonsterData data;
            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.PrefabPath = colData[2];
            data.SpawnLine = colData[3];
            data.Hp = float.Parse(colData[4]);
            data.Atk = float.Parse(colData[5]);
            data.Mp = float.Parse(colData[6]);
            data.CriRate = float.Parse(colData[7]);
            data.Range = float.Parse(colData[8]);

            if (_firstMonsterKey == i)
            {
                _monsterStartKey = data.Key;
            }

            _monsterDatas.Add(data.Key, data);
        }
    }
}