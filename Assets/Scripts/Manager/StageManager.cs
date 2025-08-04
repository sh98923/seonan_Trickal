using System.Collections.Generic;
using UnityEditor.SceneManagement;
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

    // 스테이지 번호 단위로 언락 여부 관리 (Stage → unlocked)
    private Dictionary<int, bool> _stageUnlockStatus = new Dictionary<int, bool>();

    private int _stageCount = 0;
    public int StageCount
    {
        get { return _stageCount = _stageUnlockStatus.Count; }
    }

    private readonly int _firstStageKey = 1;
    private int _stageStartKey;
    public int StageStartKey
    {
        get { return _stageStartKey; }
    }

    private void Awake()
    {
        LoadStageData();
        InitStageUnlocks();
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

    public int GetMapBGKey(int key)
    {
        return _stageDatas[key].Map;
    }

    private void LoadStageData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/StageTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if (colData.Length <= 1) continue;
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

    private void InitStageUnlocks()
    {
        // 스테이지별 최초 언락 상태 세팅 (1 스테이지만 해금)
        HashSet<int> registeredStages = new HashSet<int>();
        foreach (StageData data in _stageDatas.Values)
        {
            if (!registeredStages.Contains(data.Stage))
            {
                // 1 스테이지만 true, 나머지는 false
                bool unlocked = (data.Stage == 2);
                _stageUnlockStatus[data.Stage] = unlocked;
                registeredStages.Add(data.Stage);
            }
        }
    }

    // 언락 상태 조회
    public bool IsStageUnlocked(int stageNumber)
    {
        if (_stageUnlockStatus.TryGetValue(stageNumber, out bool unlocked))
        {
            return unlocked;
        }
        return false;
    }

    // 스테이지 클리어 시 다음 스테이지 언락 처리
    public void UnlockNextStage(int clearedStage)
    {
        int nextStage = clearedStage + 1;
        if (_stageUnlockStatus.ContainsKey(nextStage))
        {
            _stageUnlockStatus[nextStage] = true;
        }
    }
}
