using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    CharacterScene,
    CollectionScene,
    InGameScene,
    LobbyScene,
    StageSelectScene
}

public class GameManager : Singleton<GameManager>
{
    public enum StageExitType
    {
        Exit,
        Retry,
        Next
    }

    public Dictionary<int, bool> StageUnlockStatus = new Dictionary<int, bool>();

    private List<PlayerData> _spawnablePlayerDatas = new List<PlayerData>();
    public List<PlayerData> SpawnablePlayerDatas
    {
        get { return SetSpawnablePlayerData(); }
    }

    private Dictionary<int, PlayerUnitData> _deckUnitDatas = new Dictionary<int, PlayerUnitData>();
    public Dictionary<int, PlayerUnitData> DeckUnitDatas
    {
        get { return _deckUnitDatas; }
    }

    private const int _maxDeckUnitCount = 6;
    public int MaxDeckUnitCount
    {
        get { return _maxDeckUnitCount; }
    }

    private int _mapBGKey;
    public int MapBGKey
    {
        get { return _mapBGKey; }
    }

    private int _waveCount = -1;
    public int WaveCount
    {
        get { return _waveCount; }
    }

    private int _stageKey = -1;
    public int StageKey
    {
        get { return _stageKey; }
    }

    private const int _deckCharacterCount = 6;

    private bool _openDeckPanel;
    public bool OpenDeckPanel 
    {
        get {  return _openDeckPanel; }
        set { _openDeckPanel = value; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private List<PlayerData> SetSpawnablePlayerData()
    {
        _spawnablePlayerDatas.Clear();

        foreach (KeyValuePair<int, PlayerUnitData> deckUnitData in _deckUnitDatas)
        {
            _spawnablePlayerDatas.Add(deckUnitData.Value.playerData);
        }

        return _spawnablePlayerDatas;
    }

    private bool IsAllowedScene(params SceneName[] sceneNames)
    {
        string curSceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (curSceneName == sceneNames[i].ToString())
                return true;
        }

        return false;
    }

    public bool IsInDeck(PlayerData data)
    {
        for (int i = 0; i < _spawnablePlayerDatas.Count; i++)
        {
            if (_spawnablePlayerDatas[i].Key == data.Key)
            {
                return true;
            }
        }

        return false;
    }

    public void ExitStage(StageExitType exitType)
    {
        switch (exitType)
        {
            case StageExitType.Retry:
                SceneManager.LoadScene("InGameScene");
                break;
            case StageExitType.Next:
                ProceedToNextStage();
                break;
        }

        SceneManager.LoadScene("StageSelectScene");

        if(exitType == StageExitType.Retry)
        {
            SceneManager.LoadScene("InGameScene");
        }
    }

    public void OnStageEnd(StageResult.ResultType resultType)
    {
        if (resultType != StageResult.ResultType.Victory)
            return;

        if (StageManager.Instance.UnlockNextStage(_stageKey, out int nextStageKey))
        {
            _stageKey = nextStageKey; // 여기서 변경
            _waveCount = StageManager.Instance.GetWaveCount(_stageKey);

            Debug.Log($"다음 스테이지 해제 및 이동: {_stageKey}");
        }
    }

    public void ProceedToNextStage()
    {
        _openDeckPanel = true;
    }

    public void ResetDeckPanelState()
    {
        _openDeckPanel = false; // 열고 나면 다시 false
    }

    public void SetStageKey(int stageKey)
    {
        // _stagekey = 1000 
        _stageKey = stageKey;
        // wavecount 이거 stagemanager에서 모두 구해서 여기서 말고 다른곳에서 호출하기
        // monsterspawn스크립트에서 써야함
        _waveCount = StageManager.Instance.GetWaveCount(_stageKey);
    }

    public int CurDeckUnitCount()
    {
        return _deckUnitDatas.Count;
    }

    public bool CanStartGame()
    {
        // 덱 편성을 통해 6명의 캐릭터가 편성됬다면 true
        bool canGameStart = (_deckUnitDatas.Count == _deckCharacterCount);

        return true;
        // return canGameStart;
    }

    public bool IsDeckFull()
    {
        return _deckUnitDatas.Count >= _maxDeckUnitCount;
    }    

    public void AddUnit(PlayerData data, Vector3 spawnPos)
    {
        spawnPos.x += 3.33f;

        PlayerUnitData unitData = new PlayerUnitData
        {
            spawnPos = spawnPos,
            playerData = data
        };

        _deckUnitDatas.Add(data.Key, unitData);
    }

    public void RemoveUnit(PlayerData data)
    {
        _deckUnitDatas.Remove(data.Key);
    }

    public void SetActiveGameObjectInScenes(GameObject target, params SceneName[] sceneNames)
    {
        target.SetActive(IsAllowedScene(sceneNames));
    }

    public void EnableScriptInScenes(MonoBehaviour target, params SceneName[] sceneNames)
    {
        target.enabled = IsAllowedScene(sceneNames);
    }
}