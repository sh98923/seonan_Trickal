using System.Collections.Generic;
using UnityEngine;

public struct StageData
{
    public int Key;
    public int Map;
    public int Stage;
    public int Wave;
    public int WaveCoin;
    public int MaxWave;
}

public class StageManager : Singleton<StageManager>
{
    private Dictionary<int, StageData> _stageDatas = new Dictionary<int, StageData>();

    private readonly int _firstStageKey = 1;
    private int _stageStartKey;
    public int StageStartKey
    {
        get { return _stageStartKey; }
    }

    private void Awake()
    {
        LoadStageData();
    }

    public StageData GetStageData(int key)
    {
        return _stageDatas[key];
    }

    public int GetWaveCount(int key)
    {
        int stage = _stageDatas[key].Stage;
        List<StageData> results = new List<StageData>();

        foreach (StageData data in _stageDatas.Values)
        {
            if (data.Stage == stage)
            {
                results.Add(data);
            }
        }

        return results.Count;
    }

    public int GetStageStartKey(int stageNumber)
    {
        int minKey = int.MaxValue;

        foreach (StageData data in _stageDatas.Values)
        {
            if (data.Stage == stageNumber && data.Key < minKey)
            {
                minKey = data.Key;
            }
        }

        if (minKey == int.MaxValue)
        {
            Debug.LogError($"Stage {stageNumber}에 해당하는 데이터가 없습니다.");
            return -1;
        }

        return minKey;
    }

    private void LoadStageData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/StageTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if(colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            StageData data;
            data.Key = int.Parse(colData[0]);
            data.Map = int.Parse(colData[1]);
            data.Stage = int.Parse(colData[2]);
            data.Wave = int.Parse(colData[3]);
            data.WaveCoin = int.Parse(colData[4]);
            data.MaxWave = int.Parse(colData[5]);

            if (_firstStageKey == i)
            {
                _stageStartKey = data.Key;
            }

            _stageDatas.Add(data.Key, data);
        }
    }
}