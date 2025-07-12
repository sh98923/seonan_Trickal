using System.Collections.Generic;
using UnityEngine;

public struct WaveData
{
    public int Key;
    public int WaveKey;
    public int MonsterKey;
    public string Name;
    public int Count;
    public int SpawnLine;
}

public class WaveManager : Singleton<WaveManager>
{
    private Dictionary<int, WaveData> _waveDatas = new Dictionary<int, WaveData>();

    private void Awake()
    {
        LoadWaveData();
    }

    public Dictionary<int, WaveData> GetWaveMonster(int waveKey)
    {
        Dictionary<int, WaveData> result = new Dictionary<int, WaveData>();

        foreach (KeyValuePair<int, WaveData> pair in _waveDatas)
        {
            if (pair.Value.WaveKey == waveKey)
            {
                result.Add(pair.Key, pair.Value);
            }
        }

        return result;
    }

    private void LoadWaveData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/WaveTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1; i <  rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if (colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            WaveData data;
            data.Key = int.Parse(colData[0]);
            data.WaveKey = int.Parse(colData[1]);
            data.MonsterKey = int.Parse(colData[2]);
            data.Name = colData[3];
            data.Count = int.Parse(colData[4]);
            data.SpawnLine = int.Parse(colData[5]);

            _waveDatas.Add(data.Key, data);
        }
    }
}