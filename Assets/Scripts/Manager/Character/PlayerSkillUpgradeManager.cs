using System;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerSkillUpgradeData
{
    public int Key;
    public float[] Duration;
    public float[] EffectValue;  
    public float DotDamageRate;
}

public class PlayerSkillUpgradeManager : Singleton<PlayerSkillUpgradeManager>
{
    private Dictionary<int, PlayerSkillUpgradeData> _playerSkilldatas = new Dictionary<int, PlayerSkillUpgradeData>();

    private void Awake()
    {
        LoadPlayerSkillUpgradeData();
    }

    public PlayerSkillUpgradeData GetPlayerSkillUpgradeData(int key)
    {
        return _playerSkilldatas[key];
    }

    private void LoadPlayerSkillUpgradeData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayerSkillUpgradeTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if (colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            PlayerSkillUpgradeData data;
            data.Duration = new float[3];
            data.EffectValue = new float[3];

            data.Key = int.Parse(colData[0]);

            data.Duration[0] = float.Parse(colData[1]);
            data.Duration[1] = float.Parse(colData[2]);
            data.Duration[2] = float.Parse(colData[3]);

            data.EffectValue[0] = float.Parse(colData[4]);
            data.EffectValue[1] = float.Parse(colData[5]);
            data.EffectValue[2] = float.Parse(colData[6]);

            data.DotDamageRate = float.Parse(colData[7]);

            _playerSkilldatas.Add(data.Key, data);
        }
    }
}
