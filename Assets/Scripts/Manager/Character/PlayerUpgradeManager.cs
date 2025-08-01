using System.Collections.Generic;
using UnityEngine;

public struct PlayerUpgradeData
{
    public int Key;
    public string Name;
    public int MaxLevel;
    public float Hp;
    public float Mp;
    public float Atk;
    public float SkillRate;
    public float Ultimate;
    public float CriRate;
    public float AtkRange;
    public float AtkCoolTime;
    public float UltCoolTime;
    public bool CanUseUlt;
}

public class PlayerUpgradeManager : Singleton<PlayerUpgradeManager>
{
    private Dictionary<int, PlayerUpgradeData> _playerUpgradeDatas = new Dictionary<int, PlayerUpgradeData>();

    private readonly int _upgradeStepCount = 4;

    private void Awake()
    {
        LoadPlayerStatData();
    }

    public PlayerUpgradeData GetPlayerStatData(int key)
    {
        return _playerUpgradeDatas[key];
    }

    private void LoadPlayerStatData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayerUpgradeTable");

        string[] rowDatas = textAsset.text.Split("\r\n");

        for(int i = 1; i < rowDatas.Length; i++)
        {
            string[] colDatas = rowDatas[i].Split(',');

            if (colDatas.Length <= 1) continue;
            if (rowDatas[i] == "") continue;

            PlayerUpgradeData data;
            data.Key = int.Parse(colDatas[0]);
            data.Name = colDatas[1];
            data.MaxLevel = int.Parse(colDatas[2]);
            data.Hp = float.Parse(colDatas[3]);
            data.Mp = float.Parse(colDatas[4]);
            data.Atk = float.Parse(colDatas[5]);
            data.SkillRate = float.Parse(colDatas[6]);
            data.Ultimate = float.Parse(colDatas[7]);
            data.CriRate = float.Parse(colDatas[8]);
            data.AtkRange = float.Parse(colDatas[9]);
            data.AtkCoolTime = float.Parse(colDatas[10]);
            data.UltCoolTime = float.Parse(colDatas[11]);
            data.CanUseUlt = bool.Parse(colDatas[12]);

            _playerUpgradeDatas.Add(data.Key, data);
        }
    }
}