using System;

public class BattleStateManager : Singleton<BattleStateManager>
{
    private bool _isBattleStart = false;

    // 상태 변경 시 발생하는 이벤트 (구독 가능)
    public event Action<bool> OnBattleStartChanged;

    public bool IsBattleStart
    {
        get { return _isBattleStart; }
        set
        {
            if (_isBattleStart == value) return;

            _isBattleStart = value;
            OnBattleStartChanged?.Invoke(_isBattleStart);
        }
    }
}