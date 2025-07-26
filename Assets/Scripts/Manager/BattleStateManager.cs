public enum BattleState
{
    None,
    Reroll,
    Battle
}

public class BattleStateManager : Singleton<BattleStateManager>
{
    private BattleState _currentState = BattleState.None;

    public BattleState CurrentState
    {
        get { return _currentState; }
    }

    public bool IsBattle
    {
        get { return _currentState == BattleState.Battle; }
    }

    public bool IsReroll
    {
        get { return _currentState == BattleState.Reroll; }
    }

    public void SetState(BattleState newState)
    {
        _currentState = newState;
    }
}