using System.Collections.Generic;
using UnityEngine;

public struct PlayerActionData
{
    public int Key;
    public string EngName;
    public string ProjectileKey;
    public string[] ProjectileSpritePath;
    public string[] ActionImpact;
    public string[] BuffEffect;
    public float[] AtkSpeed;
    public float[] Duration;
    public float EffectValue;
    public float DotDamageRate;
    public string[] ClipName;
    public bool[] IsRange;
    public bool[] IsEffectInFront;
}

public class PlayerActionManager : Singleton<PlayerActionManager>
{
    private Dictionary<int, PlayerActionData> _playerAtkDatas = new Dictionary<int, PlayerActionData>();
    
    private void Awake()
    {
        LoadPlayerActionData();
    }

    public PlayerActionData GetPlayerActionData(int key)
    {
        return _playerAtkDatas[key];
    }

    private void LoadPlayerActionData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayerActionTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if (colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            PlayerActionData data;
            data.IsRange = new bool[3];
            data.IsEffectInFront = new bool[3];
            data.AtkSpeed = new float[3];
            data.Duration = new float[3];
            data.ActionImpact = new string[3];
            data.BuffEffect = new string[3];
            data.ClipName = new string[3];
            data.ProjectileSpritePath = new string[3];

            data.Key = int.Parse(colData[0]);
            data.EngName = colData[1];
            data.ProjectileKey = colData[2];

            data.ProjectileSpritePath[0] = colData[3];
            data.ProjectileSpritePath[1] = colData[4];
            data.ProjectileSpritePath[2] = colData[5];

            data.ActionImpact[0] = colData[6];
            data.ActionImpact[1] = colData[7];
            data.ActionImpact[2] = colData[8];

            data.BuffEffect[0] = colData[9];
            data.BuffEffect[1] = colData[10];
            data.BuffEffect[2] = colData[11];

            data.AtkSpeed[0] = float.Parse(colData[12]);
            data.AtkSpeed[1] = float.Parse(colData[13]);
            data.AtkSpeed[2] = float.Parse(colData[14]);

            data.Duration[0] = float.Parse(colData[15]);
            data.Duration[1] = float.Parse(colData[16]);
            data.Duration[2] = float.Parse(colData[17]);

            data.EffectValue = float.Parse(colData[18]);
            data.DotDamageRate = float.Parse(colData[19]);

            data.ClipName[0] = colData[20];
            data.ClipName[1] = colData[21];
            data.ClipName[2] = colData[22];

            data.IsRange[0] = bool.Parse(colData[23]);
            data.IsRange[1] = bool.Parse(colData[24]);
            data.IsRange[2] = bool.Parse(colData[25]);

            data.IsEffectInFront[0] = bool.Parse(colData[26]);
            data.IsEffectInFront[1] = bool.Parse(colData[27]);
            data.IsEffectInFront[2] = bool.Parse(colData[28]);

            _playerAtkDatas.Add(data.Key, data);
        }
    }
}