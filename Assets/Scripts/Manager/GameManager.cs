using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public struct DeckUnitData
    {
        public PlayerData playerData;
        public GameObject playerObj;
    }

    private Dictionary<int, DeckUnitData> _deckUnitDatas = new Dictionary<int, DeckUnitData>();

    private readonly int _inGameStartCoin = 30;
    public int InGameStartCoin
    {
        get { return _inGameStartCoin; }
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

    public void SetDeckUnit(PlayerData data, GameObject unit)
    {
        DeckUnitData unitData = 
            new DeckUnitData 
            { 
                playerData = data, 
                playerObj = unit
            };

        _deckUnitDatas.Add(data.Key, unitData);
    }

    public void RegisterDeckUnits()
    {
        foreach(DeckUnitData data in _deckUnitDatas.Values)
        {
            BattleUnitManager.Instance.RegisterUnit(data.playerData, data.playerObj);
        }
    }
}