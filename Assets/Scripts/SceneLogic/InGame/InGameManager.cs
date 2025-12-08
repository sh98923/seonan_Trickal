using System.Collections.Generic;
using UnityEngine;
public struct PlayerUnitData
{
    public Vector3 spawnPos;
    public PlayerData playerData;
}

public class InGameManager : Singleton<InGameManager>
{
    private InGamePlayerSpawn _playerSpawnPoint;

    private List<ITrackable> _trackables = new List<ITrackable>();
    public List<ITrackable> Trackables
    {
        get { return _trackables; }
    }

    private List<ITrackable> _monsters = new List<ITrackable>();
    public List<ITrackable> Monsters
    {
        get 
        { 
            foreach(ITrackable trackable in  Trackables)
            {
                if (trackable.Object.tag == "Monster")
                {
                    _monsters.Add(trackable);
                }
            }

            return _monsters; 
        }
    }

    private int _maxWave = 0;
    public int MaxWave
    {
        get { return _maxWave; }
    }
    private int _waveStep = 0;
    public int WaveStep
    {
        get { return _waveStep; }
    }

    private int _inGameCoin;
    public int InGameCoin
    {
        get { return _inGameCoin; }
    }

    private bool _canPayCoin = false;
    public bool CanPayCoin
    {
        get { return _canPayCoin; }
    }

    private bool _isGameStart = false;
    public bool IsGameStart
    {
        get { return _isGameStart; }
        set { _isGameStart = value; }
    }

    private bool _canBGMove = false;
    public bool CanBGMove
    {
        get { return _canBGMove; }
        set { _canBGMove = value; }
    }

    private void Awake()
    {
        _inGameCoin = 106;

        BattleStart();
    }

    private void Start()
    {
        WeaponManager.Instance.CreateWeapon();
        EffectManager.Instance.CreateEffect();
    }

    private void BattleStart()
    {
        BattleStateManager.Instance.SetState(BattleState.Reroll);
    }

    public bool TrySpendCoin(int amount)
    {
        _canPayCoin = _inGameCoin >= amount;

        if (_canPayCoin)
        {
            _inGameCoin -= amount;
            return true;
        }

        return false;
    }

    public void SetWaveText(int waveStep, int maxWave)
    {
        _waveStep = waveStep + 1;
        _maxWave = maxWave;
    }

    public void AddCoin(int amount)
    {
        _inGameCoin += amount;
    }

    public void RegisterDeckUnits(Transform inGameSpawnPoint)
    {
        _playerSpawnPoint = inGameSpawnPoint.GetComponent<InGamePlayerSpawn>();

        Dictionary<int, PlayerUnitData> deckUnitDatas = GameManager.Instance.DeckUnitDatas;

        foreach (PlayerUnitData data in deckUnitDatas.Values)
        {
            PlayerData playerData = data.playerData;

            GameObject prefab = Resources.Load<GameObject>(playerData.CharacterPrefabPath);

            GameObject player = Instantiate(prefab, inGameSpawnPoint);
            player.name = playerData.EngName;
            player.layer = LayerMask.NameToLayer(playerData.Layer);
            player.transform.position = data.spawnPos;
            player.SetActive(false);

            ITrackable trackable = player.GetComponent<ITrackable>();
            _trackables.Add(trackable);

            CharacterData characterData = new CharacterData(playerData);
            BattleUnitManager.Instance.RegisterUnit(characterData, player);
        }
    }

    public void RegisterMonsterUnits(List<ITrackable> trackables)
    {
        foreach(ITrackable trackable in trackables)
        {
            _trackables.Add(trackable);
        }
    }
}