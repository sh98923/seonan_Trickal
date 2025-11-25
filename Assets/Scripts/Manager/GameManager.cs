using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    private List<PlayerData> _spawnablePlayerDatas = new List<PlayerData>();
    public List<PlayerData> SpawnablePlayerDatas
    {
        get { return _spawnablePlayerDatas; }
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

    public void SetStageKey(int stageKey)
    {
        _stageKey = stageKey; 
        _waveCount = StageManager.Instance.GetWaveCount(_stageKey);
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

        if (!_spawnablePlayerDatas.Contains(data))
            _spawnablePlayerDatas.Add(data);
    }

    public void RemoveUnit(PlayerData data)
    {
        _deckUnitDatas.Remove(data.Key);

        if (_spawnablePlayerDatas.Contains(data))
            _spawnablePlayerDatas.Remove(data);
    }
}