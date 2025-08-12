using System.Collections.Generic;
using UnityEngine;

public struct PlayerStatData
{
    public int Key;
    public string EngName;
    public float Hp;
    public float Mp;
    public float Atk;
    public float SkillRate;
    public float Ultimate;
    public float CriRate;
    public float AtkRange;
    public float UltCoolTime;
    public bool CanUseUlt;
}

public class PlayerStatManager : Singleton<PlayerStatManager>
{
    private Dictionary<int, PlayerStatData> _playerStatDatas = new Dictionary<int, PlayerStatData>();

    private void Awake()
    {
        LoadPlayerStatData();
    }

    public PlayerStatData GetPlayerStatData(int key)
    {
        return _playerStatDatas[key];
    }

    private void LoadPlayerStatData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayerUpgradeTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if (colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            PlayerStatData data;
            data.Key = int.Parse(colData[0]);
            data.EngName = colData[1];
            data.Hp = float.Parse(colData[2]);
            data.Mp = float.Parse(colData[3]);
            data.Atk = float.Parse(colData[4]);
            data.SkillRate = float.Parse(colData[5]);
            data.Ultimate = float.Parse(colData[6]);
            data.CriRate = float.Parse(colData[7]);
            data.AtkRange = float.Parse(colData[8]);
            data.UltCoolTime = float.Parse(colData[9]);
            data.CanUseUlt = bool.Parse(colData[10]);

            _playerStatDatas.Add(data.Key, data);
        }
    }
}