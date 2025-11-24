using System.Collections.Generic;
using UnityEngine;

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

    public void SetDeckUnit(PlayerData data, Vector3 spawnPos)
    {
        spawnPos.x += 3.33f;

        PlayerUnitData unitData =
            new PlayerUnitData
            {
                spawnPos = spawnPos,
                playerData = data
            };

        // 이미 존재하면 갱신
        if (_deckUnitDatas.ContainsKey(data.Key))
        {
            _deckUnitDatas[data.Key] = unitData;
        }
        else
        {
            _deckUnitDatas.Add(data.Key, unitData);
        }

        // _spawnablePlayerDatas는 중복 체크 필요하면 처리
        if (!_spawnablePlayerDatas.Contains(data))
        { 
            _spawnablePlayerDatas.Add(data);
        }
    }
}