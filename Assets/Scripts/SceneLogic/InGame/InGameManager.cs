using System;
using UnityEngine;
using System.Collections.Generic;

public struct PlayerUnitData
{
    public Vector3 spawnPos;
    public PlayerData playerData;
}

public class InGameManager : MonoBehaviour
{
    private static InGameManager _instance;
    public static InGameManager Instance
    {
        get { return _instance; }
    }

    public static event Action<int> OnCoinChanged;

    private List<ITrackable> _trackables = new List<ITrackable>();
    public List<ITrackable> Trackables
    {
        get { return _trackables; }
    }

    private List<ITrackable> _players = new List<ITrackable>();
    public List<ITrackable> Players
    {
        get { return _players; }
    }

    private List<ITrackable> _monsters = new List<ITrackable>();
    public List<ITrackable> Monsters
    {
        get { return _monsters; }
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

    private int _baseWaveReward;
    private int _nextWaveReward = 12;

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
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _inGameCoin = 999;
        _baseWaveReward = 23;
        _nextWaveReward = 12;
        //_inGameCoin = 30;
    }

    private void Start()
    {
        SlotMachineController.Instance.OnSlotMachineFinished += OnSlotFinished;
    }

    private void OnDisable()
    {
        SlotMachineController.Instance.OnSlotMachineFinished -= OnSlotFinished;
        BattleStateManager.Instance.OnReroll -= OnWaveEnd;
    }

    private void OnSlotFinished()
    {
        WeaponManager.Instance.CreateWeapon();
        EffectManager.Instance.CreateEffect();

        BattleStateManager.Instance.OnReroll += OnWaveEnd;

        ApplySlotResult();
    }

    private void ApplySlotResult()
    {
        int[] resultKeys = SlotMachineController.Instance.SlotMachineResultPlayerKeys;

        foreach (int key in resultKeys)
        {
            PlayerData data = PlayerManager.Instance.GetPlayerData(key);
            InGamePlayerSpawn.Instance.SetActivePlayer(data.EngName);
        }
    }

    private void RegisterPlayer(ITrackable trackable)
    {
        _players.Add(trackable); 
        _trackables.Add(trackable);
    }

    private void RegisterMonster(ITrackable trackable)
    {
        _monsters.Add(trackable);
        _trackables.Add(trackable);
    }

    private void OnWaveEnd()
    {
        if (!BattleStateManager.Instance.IsWaveCleared)
            return;

        AddCoin(_baseWaveReward);
        CalculateNextWaveReward();
    }

    private void CalculateNextWaveReward()
    {
        _baseWaveReward += _nextWaveReward;
        _nextWaveReward--;
    }

    public bool TrySpendCoin(int amount)
    {
        if (_inGameCoin < amount)
        {
            return false;
        }

        _inGameCoin -= amount;
        OnCoinChanged?.Invoke(_inGameCoin);
        return true;
    }

    public void SetWaveText(int waveStep, int maxWave)
    {
        _waveStep = waveStep + 1;
        _maxWave = maxWave;
    }

    public void AddCoin(int amount)
    {
        _inGameCoin += amount;
        OnCoinChanged?.Invoke(_inGameCoin);
    }

    public void RegisterDeckUnits(Transform inGameSpawnPoint)
    {
        Dictionary<int, PlayerUnitData> deckUnitDatas = GameManager.Instance.DeckUnitDatas;

        foreach (PlayerUnitData data in deckUnitDatas.Values)
        {
            PlayerData playerData = data.playerData;

            GameObject prefab = Resources.Load<GameObject>(playerData.CharacterPrefabPath);

            GameObject player = Instantiate(prefab, inGameSpawnPoint);
            player.name = playerData.EngName;
            player.layer = LayerMask.NameToLayer(playerData.Layer);
            player.transform.position = data.spawnPos;

            ITrackable trackable = player.GetComponent<ITrackable>();
            RegisterPlayer(trackable);

            CharacterData characterData = new CharacterData(playerData);
            BattleUnitManager.Instance.RegisterUnit(characterData, player); 

            player.SetActive(false);
        }
    }

    public void RegisterMonsterUnits(List<ITrackable> trackables)
    {
        foreach (ITrackable trackable in trackables)
        {
            RegisterMonster(trackable);
        }
    }
}