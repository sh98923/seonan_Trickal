/*using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int Key;
    public int StatKey;
    public string Name;
    public string EngName;
    public string PrefabPath;
    public string SpritePath;
    public string Layer;
    public int SpawnLine;
    public int CardCost;
}

public struct MonsterData
{
    public int Key;
    public string Name;
    public string PrefabPath;
    public string Layer;
    public int SpawnLine;
    public float Hp;
    public float HpPerWave;
    public float HpGrowthRate;
    public float Atk;
    public float AtkPerWave;
    public float AtkGrowthRate;
    public float AtkCoolTime;
    public float CriRate;
    public float Range;
    public string Type;
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
            data.StatKey = int.Parse(colData[1]);
            data.Name = colData[2];
            data.EngName = colData[3];
            data.PrefabPath = colData[4];
            data.SpritePath = colData[5];
            data.Layer = colData[6];
            data.SpawnLine = int.Parse(colData[7]);
            data.CardCost = int.Parse(colData[8]);

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
            data.Layer = colData[3];
            data.SpawnLine = int.Parse(colData[4]);
            data.Hp = float.Parse(colData[5]);
            data.HpPerWave = float.Parse(colData[6]);
            data.HpGrowthRate = float.Parse(colData[7]);
            data.Atk = float.Parse(colData[8]);
            data.AtkPerWave = float.Parse(colData[9]);
            data.AtkGrowthRate = float.Parse(colData[10]);
            data.AtkCoolTime = float.Parse(colData[11]);
            data.CriRate = float.Parse(colData[12]);
            data.Range = float.Parse(colData[13]);
            data.Type = colData[14];

            if (_firstMonsterKey == i)
            {
                _monsterStartKey = data.Key;
            }

            _monsterDatas.Add(data.Key, data);
        }
    }
}*/

using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
    public int Key;
    public int UnitKey;
    public string Type;
    public string EngName;
    public string KrName;
    public string PrefabPath;
    public string Layer;
    public int SpawnLine;
    public float Hp;
    public float Atk;
    public float AtkRange;
    public float AtkCoolTime;
    public float CriRate;
}

public class CharacterManager : Singleton<CharacterManager>
{
    private Dictionary<int, CharacterData> _characterDatas = new Dictionary<int, CharacterData>();

    private int _playerStartKey;
    public int PlayerStartKey
    {
        get { return _playerStartKey; }
    }

    private int _monsterStartKey;
    public int MonsterStartKey
    {
        get { return _monsterStartKey; }
    }

    private int _playerCount = 0;
    public int PlayerCount
    {
        get { return _playerCount; }
    }

    private int _monsterCount = 0;
    public int MonsterCount
    {
        get { return _monsterCount; }
    }

    private bool _isPlayerFirstkey = false;
    private bool _isMonsterFirstkey = false;

    private void Awake()
    {
        LoadCharacterData();
        SetCharacterCount();
    }

    private void SetCharacterCount()
    {
        for(int i = 0; i < _characterDatas.Count; i++)
        {
            if (_characterDatas[i].Type == "Player")
            {
                _playerCount++;
            }
            else if (_characterDatas[i].Type == "Monster")
            {
                _monsterCount++;
            }
        }
    }

    public CharacterData GetCharacterData(int key)
    {
        return _characterDatas[key];
    }

    private void LoadCharacterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/CharacterTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1; i <  rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1) return;
            if (rowData[i] == "") return;

            CharacterData data;

            data.Key = int.Parse(colData[0]);
            data.UnitKey = int.Parse(colData[1]);
            data.Type = colData[2];
            data.EngName = colData[3];
            data.KrName = colData[4];
            data.PrefabPath = colData[5];
            data.Layer = colData[6];
            data.SpawnLine = int.Parse(colData[7]);
            data.Hp = float.Parse(colData[8]);
            data.Atk = float.Parse(colData[9]);
            data.AtkRange = float.Parse(colData[10]);
            data.AtkCoolTime = float.Parse(colData[11]);
            data.CriRate = float.Parse(colData[12]);

            if(!_isPlayerFirstkey && data.Type == "Player")
            {
                _isPlayerFirstkey = true;
                _playerStartKey = data.Key;
            }

            if (!_isMonsterFirstkey && data.Type == "Monster")
            {
                _isMonsterFirstkey = true;
                _monsterStartKey = data.Key;
            }

            _characterDatas.Add(data.UnitKey, data);
        }
    }
}