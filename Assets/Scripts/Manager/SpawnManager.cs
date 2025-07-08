using System.Collections.Generic;
using UnityEngine;

public struct SpawnPosData
{
    public int Key;
    public Vector2 Ratio;
    public string Layer;
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private Dictionary<int, SpawnPosData> _spawnPlayerDatas = new Dictionary<int, SpawnPosData>();
    private Dictionary<int, SpawnPosData> _spawnMonsterDatas = new Dictionary<int, SpawnPosData>();

    private void Awake()
    {
        Instance = this;

        LoadSpawnPosData();
    }
    public SpawnPosData GetPlayerData(int key)
    {
        return _spawnPlayerDatas[key];
    }

    public SpawnPosData GetMonsterData(int key)
    {
        return _spawnPlayerDatas[key];
    }

    private void LoadSpawnPosData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/SpawnPointTable");

        string[] rowData = textAsset.text.Split("\r\n");

        float DataHalfSize = rowData.Length * 0.5f;

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1) return;
            if (rowData[i] == "") return;

            SpawnPosData data;
            data.Key = int.Parse(colData[0]);
            data.Ratio.x = float.Parse(colData[1]);
            data.Ratio.y = float.Parse(colData[2]);
            data.Layer = colData[3];

            if (DataHalfSize > i)
            { 
                _spawnPlayerDatas.Add(data.Key, data); 
            }
            else
            {
                _spawnMonsterDatas.Add(data.Key, data);
            }
        }
    }
}
