using System.Collections.Generic;
using UnityEngine;

public struct PlayerActionData
{
    public int Key;
    public string EngName;
    public string[] ProjectileKey;
    public string[] ProjectileSpritePath;
    public string[] ActionImpact;
    public string[] Hittype;
    public string[] BuffEffect;
    public float[] ProjectileSpeed;
    public string[] ClipName;
    public bool[] IsRange;
    public bool[] IsEffectInFront;
    public bool IsRotationProjectile;
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
            data.ProjectileSpeed = new float[3];
            data.ProjectileKey = new string[3];
            data.ActionImpact = new string[3];
            data.Hittype = new string[3];
            data.BuffEffect = new string[3];
            data.ClipName = new string[3];
            data.ProjectileSpritePath = new string[3];

            data.Key = int.Parse(colData[0]);
            data.EngName = colData[1];

            data.ProjectileKey[0] = colData[2];
            data.ProjectileKey[1] = colData[3];
            data.ProjectileKey[2] = colData[4];

            data.ProjectileSpritePath[0] = colData[5];
            data.ProjectileSpritePath[1] = colData[6];
            data.ProjectileSpritePath[2] = colData[7];

            data.ActionImpact[0] = colData[8];
            data.ActionImpact[1] = colData[9];
            data.ActionImpact[2] = colData[10];

            data.Hittype[0] = colData[11];
            data.Hittype[1] = colData[12];
            data.Hittype[2] = colData[13];

            data.BuffEffect[0] = colData[14];
            data.BuffEffect[1] = colData[15];
            data.BuffEffect[2] = colData[16];

            data.ClipName[0] = colData[17];
            data.ClipName[1] = colData[18];
            data.ClipName[2] = colData[19];

            data.ProjectileSpeed[0] = float.Parse(colData[20]);
            data.ProjectileSpeed[1] = float.Parse(colData[21]);
            data.ProjectileSpeed[2] = float.Parse(colData[22]);

            data.IsRange[0] = bool.Parse(colData[23]);
            data.IsRange[1] = bool.Parse(colData[24]);
            data.IsRange[2] = bool.Parse(colData[25]);

            data.IsEffectInFront[0] = bool.Parse(colData[26]);
            data.IsEffectInFront[1] = bool.Parse(colData[27]);
            data.IsEffectInFront[2] = bool.Parse(colData[28]);

            data.IsRotationProjectile = bool.Parse(colData[29]);

            _playerAtkDatas.Add(data.Key, data);
        }
    }
}