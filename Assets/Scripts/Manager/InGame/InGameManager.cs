using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
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
        _inGameCoin = 30;

        BattleStart();
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
}
