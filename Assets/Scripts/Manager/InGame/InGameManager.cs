using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    private int _inGameCoin;
    public int InGameCoin
    {
        get { return _inGameCoin; }
        set { _inGameCoin = value; }
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
}
