using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private struct PlayerUnitData
    {
        public Vector3 spawnPos;
        public PlayerData playerData;
    }

    private List<PlayerData> _spawnablePlayerDatas = new List<PlayerData>();
    public List<PlayerData> SpawnablePlayerDatas
    {
        get { return _spawnablePlayerDatas; }
    }

    private Dictionary<int, PlayerUnitData> _deckUnitDatas = new Dictionary<int, PlayerUnitData>();

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
        PlayerUnitData unitData =
            new PlayerUnitData
            {
                spawnPos = spawnPos,
                playerData = data
            };

        _spawnablePlayerDatas.Add(data);
        _deckUnitDatas.Add(data.Key, unitData);
    }

    public void RegisterDeckUnits(Transform parent)
    {
        foreach (PlayerUnitData data in _deckUnitDatas.Values)
        {
            PlayerData playerData = data.playerData;

            GameObject prefab = Resources.Load<GameObject>(playerData.PrefabPath);

            GameObject player = Instantiate(prefab, parent);
            player.name = playerData.EngName;
            player.layer = LayerMask.NameToLayer(playerData.Layer);
            player.transform.position = data.spawnPos;
            player.SetActive(false);

            CharacterData characterData = new CharacterData(playerData);
            BattleUnitManager.Instance.RegisterUnit(characterData, player);
        }
    }
}