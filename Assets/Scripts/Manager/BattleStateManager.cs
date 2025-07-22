public enum BattleState
{
    None, Reroll, Battle
}

public class BattleStateManager : Singleton<BattleStateManager>
{
    private BattleState _currentState = BattleState.None;

    public BattleState CurrentState => _currentState;

    public void SetState(BattleState newState)
    {
        _currentState = newState;
    }

    public bool IsBattle => _currentState == BattleState.Battle;
    public bool IsReroll => _currentState == BattleState.Reroll;
}
