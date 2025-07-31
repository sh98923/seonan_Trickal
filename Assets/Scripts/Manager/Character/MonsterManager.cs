using System.Collections.Generic;
using UnityEngine;

public struct MonsterData
{
    public int Key;
    public float HpPerWave;
    public float HpGrowthRate;
    public float AtkPerWave;
    public float AtkGrowthRate;
    public string Type;
    public string WeaponPath;
}

public class MonsterManager : Singleton<MonsterManager>
{
    private Dictionary<int, MonsterData> _monsterDatas = new Dictionary<int, MonsterData>();

    private void Awake()
    {
        LoadMonsterDatas();
    }

    public MonsterData GetMonsterData(int key)
    {
        return _monsterDatas[key];
    }

    private void LoadMonsterDatas()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/MonsterTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (rowData[i] == "") continue;
            if (colData.Length <= 1) continue;

            MonsterData data;

            data.Key = int.Parse(colData[0]);
            data.HpPerWave = float.Parse(colData[1]);
            data.HpGrowthRate = float.Parse(colData[2]);
            data.AtkPerWave = float.Parse(colData[3]);
            data.AtkGrowthRate = float.Parse(colData[4]);
            data.Type = colData[5];
            data.WeaponPath = colData[6];

            _monsterDatas.Add(data.Key, data);
        }
    }
}