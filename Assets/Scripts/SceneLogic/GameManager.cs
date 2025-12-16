using System.Collections.Generic;
using System.ComponentModel;
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
        foreach (KeyValuePair<int, PlayerUnitData> deckUnitData in _deckUnitDatas)
        {
            _spawnablePlayerDatas.Add(deckUnitData.Value.playerData);
        }

        return _spawnablePlayerDatas;
    }

    public void SetStageKey(int stageKey)
    {
        _stageKey = stageKey; 
        _waveCount = StageManager.Instance.GetWaveCount(_stageKey);
    }

    public bool CanStartGame()
    {
        // 덱 편성을 통해 최소 1명이상의 캐릭터가 편성됬다면 true
        return _deckUnitDatas.Count > 0;
    }

    public void AddUnit(PlayerData data, Vector3 spawnPos)
    {
        spawnPos.x += 3.33f;

        // 신규라면 추가
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

    public void EnableInScenes(MonoBehaviour target, params SceneName[] sceneNames)
    {
        string curScene = SceneManager.GetActiveScene().name;

        foreach (SceneName scene in sceneNames)
        {
            if (curScene == scene.ToString())
                return; // 허용된 씬
        }

        target.enabled = false;
    }
}