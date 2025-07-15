using System.Collections.Generic;
using UnityEngine;

public struct PlayerStatData
{
    public int Key;
    public int StatKey;
    public float Hp;
    public float Mp;
    public float Atk;
    public float BaseSkill;
    public float Ultimate;
    public float CriRate;
    public float Range;
    public float AtkCoolTime;
    public float UltCoolTime;
    public bool CanUseUlt;
    public bool MaxLevel;
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
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayerCharacterStatTable");

        string[] rowDatas = textAsset.text.Split("\r\n");

        for(int i = 1; i < rowDatas.Length; i++)
        {
            string[] colDatas = rowDatas[i].Split(',');

            if (colDatas.Length <= 1) continue;
            if (rowDatas[i] == "") continue;

            PlayerStatData data;
            data.Key = int.Parse(colDatas[0]);
            data.StatKey = int.Parse(colDatas[1]);
            data.Hp = float.Parse(colDatas[2]);
            data.Mp = float.Parse(colDatas[3]);
            data.Atk = float.Parse(colDatas[4]);
            data.BaseSkill = float.Parse(colDatas[5]);
            data.Ultimate = float.Parse(colDatas[6]);
            data.CriRate = float.Parse(colDatas[7]);
            data.Range = float.Parse(colDatas[8]);
            data.AtkCoolTime = float.Parse(colDatas[9]);
            data.UltCoolTime = float.Parse(colDatas[10]);
            data.CanUseUlt = bool.Parse(colDatas[11]);
            data.MaxLevel = bool.Parse(colDatas[12]);

            _playerStatDatas.Add(data.Key, data);
        }
    }
}
