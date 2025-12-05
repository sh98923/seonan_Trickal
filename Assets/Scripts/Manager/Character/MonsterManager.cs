using System.Collections.Generic;
using UnityEngine;

public struct MonsterData
{
    public int Key;
    public int SpawnLine;
    public float Hp;
    public float HpPerWave;
    public float HpGrowthRate;
    public float Atk;
    public float AtkPerWave;
    public float AtkGrowthRate;
    public float AtkRange;
    public float CriRate;
    public float[] AtkSpeed;
    public string EngName;
    public string PrefabPath;
    public string Layer;
    public string AtkType;
    public string Target;
    public string[] ProjectileKey;
    public string[] ActionImpact;
    public string[] FlashHit;
    public string[] ProjectileSpritePath;
    public bool[] IsRange;
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

    public string GetMonsterPrefab(int key)
    {
        return _monsterDatas[key].PrefabPath;
    }

    public CharacterData GetMonsterFullData(int key)
    {
        MonsterData monsterData = _monsterDatas[key];
        CharacterData data = new CharacterData(monsterData);

        return data;
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
            data.IsRange = new bool[1];
            data.AtkSpeed = new float[1];
            data.ProjectileKey = new string[1];
            data.ActionImpact = new string[1];
            data.FlashHit = new string[1];
            data.ProjectileSpritePath = new string[1];

            data.Key = int.Parse(colData[0]);
            data.EngName = colData[1];
            data.PrefabPath = colData[2];
            data.ProjectileKey[0] = colData[3];
            data.Layer = colData[4];
            data.AtkType = colData[5];
            data.Target = colData[6];

            data.ProjectileSpritePath[0] = colData[7];

            data.IsRange[0] = bool.Parse(colData[8]);

            data.ActionImpact[0] = colData[9];

            data.FlashHit[0] = colData[10];

            data.AtkSpeed[0] = float.Parse(colData[11]);

            data.SpawnLine = int.Parse(colData[12]);
            data.Hp = float.Parse(colData[13]);
            data.HpPerWave = float.Parse(colData[14]);
            data.HpGrowthRate = float.Parse(colData[15]);
            data.Atk = float.Parse(colData[16]);
            data.AtkPerWave = float.Parse(colData[17]);
            data.AtkGrowthRate = float.Parse(colData[18]);
            data.AtkRange = float.Parse(colData[19]);
            data.CriRate = float.Parse(colData[20]);

            _monsterDatas.Add(data.Key, data);
        }
    }
}