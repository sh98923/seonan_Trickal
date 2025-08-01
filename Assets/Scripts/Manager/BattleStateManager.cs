using System;

public enum BattleState
{
    None,
    Reroll,
    Battle,
    MonstersDefeated,
    PlayersDefeated,
    PlayerWin,
    PlayerLose,
    GameOver
}

public class BattleStateManager : Singleton<BattleStateManager>
{
    private event Action _onReroll;
    public event Action OnReroll
    {
        add { _onReroll += value; }
        remove { _onReroll -= value; }
    }

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
        get
        {
            return _currentState == BattleState.Reroll;
        }
    }

    public bool IsGameOver
    {
        get { return _currentState == BattleState.GameOver; }
    }

    public void SetState(BattleState newState)
    {
        _currentState = newState;
    }

    public void RerollEvent()
    {
        _onReroll?.Invoke();
    }
}