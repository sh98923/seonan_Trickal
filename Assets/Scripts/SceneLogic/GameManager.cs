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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private List<PlayerData> SetSpawnablePlayerData()
    {
        if(_spawnablePlayerDatas.Count > 0)
        {
            return _spawnablePlayerDatas;
        }

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

    public void SetStageKey(int stageKey)
    {
        _stageKey = stageKey; 
        _waveCount = StageManager.Instance.GetWaveCount(_stageKey);
    }

    public int CurDeckUnitCount()
    {
        return _deckUnitDatas.Count;
    }

    public bool CanStartGame()
    {
        // 덱 편성을 통해 최소 1명이상의 캐릭터가 편성됬다면 true
        return _deckUnitDatas.Count > 0;
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