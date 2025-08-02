using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
    public int Key;
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

    private int _monsterStartKey;
    public int MonsterStartKey
    {
        get { return _monsterStartKey; }
    }

    private int _monsterCount = 0;
    public int MonsterCount
    {
        get { return _monsterCount; }
    }

    private bool _isMonsterFirstkey = false;

    private void Awake()
    {
        LoadCharacterData();
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
            data.Type = colData[1];
            data.EngName = colData[2];
            data.KrName = colData[3];
            data.PrefabPath = colData[4];
            data.Layer = colData[5];
            data.SpawnLine = int.Parse(colData[6]);
            data.Hp = float.Parse(colData[7]);
            data.Atk = float.Parse(colData[8]);
            data.AtkRange = float.Parse(colData[9]);
            data.AtkCoolTime = float.Parse(colData[10]);
            data.CriRate = float.Parse(colData[11]);

            _characterDatas.Add(data.Key, data);
        }
    }
}