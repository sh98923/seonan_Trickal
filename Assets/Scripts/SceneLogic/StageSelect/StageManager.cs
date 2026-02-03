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
    private Dictionary<int, bool> _unlockStatus
    {
        get { return GameManager.Instance.StageUnlockStatus; }
    }

    private Dictionary<int, int> _stageKeys = new Dictionary<int, int>();
    public Dictionary<int, int> StageKeys
    { 
        get { return _stageKeys; }
    }

    private List<int> _stageKeyOrder = new List<int>();

    private int _stageMaxCount = 0;
    public int StageMaxCount
    {
        get { return _stageMaxCount = _unlockStatus.Count; }
    }

    private int _unlockedStageCount = 0;
    public int UnlockedStageCount
    {
        get
        {
            int count = 0;
            foreach (bool unlocked in _unlockStatus.Values)
            {
                if (unlocked)
                {
                    count++;
                }
            }
            return count;
        }
    }

    private const int _firstIndex = 1;
    private int _stageStartKey;
    public int StageStartKey
    {
        get { return _stageStartKey; }
    }

    private int _firstStage = 0;

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
        int waveCnt = 0;
        int stage = _stageDatas[key].Stage;

        foreach (StageData data in _stageDatas.Values)
        {
            if (data.Stage == stage)
            {
                waveCnt++;
            }
        }

        return waveCnt;
    }

    public int GetStageStartKey(int stageNumber)
    {
        int startKey = 0;

        foreach (KeyValuePair<int, int> stagekey in _stageKeys)
        {
            if (stagekey.Value == stageNumber)
            {
                startKey = stagekey.Key;
            }
        }

        if (startKey == 0)
        {
            Debug.LogError($"Stage {stageNumber}에 해당하는 데이터가 없습니다.");
            return -1;
        }

        return startKey;
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

            if (_firstIndex == i)
            {
                _stageStartKey = data.Key;
                _firstStage = data.Stage;
            }

            if(data.Wave == 1)
            {
                _stageKeyOrder.Add(data.Key);
                _stageKeys.Add(data.Key, data.Stage);
            }

            _stageDatas.Add(data.Key, data);
        }
    }

    private void InitStageUnlocks()
    {
        if (_unlockStatus.Count > 0) return;

        foreach(KeyValuePair<int, int> stageKey in _stageKeys)
        {
            bool unlocked = (stageKey.Value == _firstStage);
            _unlockStatus[stageKey.Value] = unlocked;
        }

        // 스테이지별 최초 언락 상태 세팅 (1 스테이지만 해금)
        HashSet<int> registeredStages = new HashSet<int>();
        foreach (StageData data in _stageDatas.Values)
        {
            if (!registeredStages.Contains(data.Stage))
            {
                // 1 스테이지만 true, 나머지는 false
                bool unlocked = (data.Stage == _firstStage);
                _unlockStatus[data.Stage] = unlocked;
                registeredStages.Add(data.Stage);
            }
        }
    }

    // 언락 상태 조회
    public bool IsStageUnlocked(int stageNumber)
    { 
        if (_unlockStatus.TryGetValue(stageNumber, out bool unlocked))
        {
            return unlocked;
        }
        return false;
    }

    // 스테이지 클리어 시 다음 스테이지 언락 처리
    public bool UnlockNextStage(int currentStageKey, out int nextStageStartKey)
    {
        nextStageStartKey = -1;

        // 1. 현재 stageKey가 속한 Stage 번호
        int currentStage = _stageDatas[currentStageKey].Stage;

        // 2. 해당 Stage의 시작 key 찾기
        int currentStageStartKey = GetStageStartKey(currentStage);

        int index = _stageKeyOrder.IndexOf(currentStageStartKey);
        if (index < 0 || index + 1 >= _stageKeyOrder.Count)
            return false;

        // 3. 다음 스테이지 시작 key
        nextStageStartKey = _stageKeyOrder[index + 1];
        int nextStageNumber = _stageKeys[nextStageStartKey];

        // 4. 언락 처리
        _unlockStatus[nextStageNumber] = true;
        return true;
    }
    /*public bool UnlockNextStage(int stageKey)
    {
        int index = _stageKeyOrder.IndexOf(stageKey);
        int nextStageKey = _stageKeyOrder[index + 1];
        
        if (_unlockStatus.ContainsKey(_stageKeys[nextStageKey]))
        {
            _unlockStatus[_stageKeys[nextStageKey]] = true;
            return true;
        }

        return false;
    }*/

    public int HighestStageOutLineOn()
    {
        int highestStageKey = 0;

        foreach(KeyValuePair<int, bool> stageUnlock in _unlockStatus)
        {
            if(stageUnlock.Value)
            {
                highestStageKey = stageUnlock.Key;
            }
        }

        return highestStageKey;
    }
}
