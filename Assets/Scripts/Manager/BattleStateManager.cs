using System;

public class BattleStateManager : Singleton<BattleStateManager>
{
    private bool _isBattleStart = false;

    public bool IsBattleStart
    {
        get { return _isBattleStart; }
        set { _isBattleStart = value; }
    }
}