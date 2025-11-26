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

            CharacterData characterData = new CharacterData(playerData);
            BattleUnitManager.Instance.RegisterUnit(characterData, player);
        }
    }
}