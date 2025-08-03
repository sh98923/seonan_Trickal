using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private struct PlayerUnitFullData
    {
        public Vector3 spawnPos;
        public CharacterFullData characterFullData;
    }

    private List<PlayerData> _spawnablePlayerDatas = new List<PlayerData>();
    public List<PlayerData> SpawnablePlayerDatas
    {
        get { return _spawnablePlayerDatas; }
    }

    private Dictionary<int, PlayerUnitFullData> _deckUnitDatas = new Dictionary<int, PlayerUnitFullData>();

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

    public void SetDeckUnit(CharacterFullData fullData, Vector3 spawnPos)
    {
        PlayerUnitFullData data =
            new PlayerUnitFullData
            {
                spawnPos = spawnPos,
                characterFullData = fullData
            };

        _spawnablePlayerDatas.Add(fullData.PlayerInfo);
        _deckUnitDatas.Add(fullData.PlayerKey, data);
    }

    public void RegisterDeckUnits(Transform parent)
    {
        foreach(PlayerUnitFullData data in _deckUnitDatas.Values)
        {
            CharacterFullData fullData = data.characterFullData;

            GameObject prefab = Resources.Load<GameObject>(fullData.PrefabPath);

            GameObject player = Instantiate(prefab, parent);
            player.name = fullData.EngName;
            player.layer = LayerMask.NameToLayer(fullData.Layer);
            player.transform.position = data.spawnPos;
            player.SetActive(false);

            BattleUnitManager.Instance.RegisterUnit(fullData, player);
        }
    }
}