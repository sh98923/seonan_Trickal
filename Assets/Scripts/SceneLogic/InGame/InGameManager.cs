using System.Collections.Generic;
using UnityEngine;
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

    private int _inGameCoin;
    public int InGameCoin
    {
        get { return _inGameCoin; }
        set { _inGameCoin = value; }
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

        _inGameCoin = 106;
    }

    private void Start()
    {
        BattleStart();
        WeaponManager.Instance.CreateWeapon();
        EffectManager.Instance.CreateEffect();
    }

    private void BattleStart()
    {
        BattleStateManager.Instance.SetState(BattleState.Reroll);
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

    public bool TrySpendCoin(int amount)
    {
        return _canPayCoin = (_inGameCoin >= amount);
    }

    public int SpendCoin(int amount)
    {
        if (!_canPayCoin)
        {
            return _inGameCoin;
        }

        _inGameCoin -= amount;
        return _inGameCoin;
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
            RegisterPlayer(trackable);

            CharacterData characterData = new CharacterData(playerData);
            BattleUnitManager.Instance.RegisterUnit(characterData, player);
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