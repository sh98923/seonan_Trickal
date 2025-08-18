using System.Collections.Generic;
using UnityEngine;

public struct PlayerAttackData
{
    public int Key;
    public string EngName;
    public string ProjectileKey;
    public string[] ProjectileSpritePath;
    public string[] AtkImpact;
    public float[] AtkSpeed;
    public bool[] IsRange;
    public bool[] IsEffectInFront;
}

public class PlayerAttackManager : Singleton<PlayerAttackManager>
{

    private Dictionary<int, PlayerAttackData> _playerAttackDatas = new Dictionary<int, PlayerAttackData>();
    
    private void Awake()
    {
        LoadPlayerAttackData();
    }

    public PlayerAttackData GetplayerAttackData(int key)
    {
        return _playerAttackDatas[key];
    }

    private void LoadPlayerAttackData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayerAttackTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if (colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            PlayerAttackData data;
            data.IsRange = new bool[3];
            data.IsEffectInFront = new bool[3];
            data.AtkSpeed = new float[3];
            data.AtkImpact = new string[3];
            data.ProjectileSpritePath = new string[3];

            data.Key = int.Parse(colData[0]);
            data.EngName = colData[1];
            data.ProjectileKey = colData[2];

            data.ProjectileSpritePath[0] = colData[3];
            data.ProjectileSpritePath[1] = colData[4];
            data.ProjectileSpritePath[2] = colData[5];

            data.AtkImpact[0] = colData[6];
            data.AtkImpact[1] = colData[7];
            data.AtkImpact[2] = colData[8];

            data.AtkSpeed[0] = float.Parse(colData[9]);
            data.AtkSpeed[1] = float.Parse(colData[10]);
            data.AtkSpeed[2] = float.Parse(colData[11]);

            data.IsRange[0] = bool.Parse(colData[12]);
            data.IsRange[1] = bool.Parse(colData[13]);
            data.IsRange[2] = bool.Parse(colData[14]);

            data.IsEffectInFront[0] = bool.Parse(colData[15]);
            data.IsEffectInFront[1] = bool.Parse(colData[16]);
            data.IsEffectInFront[2] = bool.Parse(colData[17]);

            _playerAttackDatas.Add(data.Key, data);
        }
    }
}