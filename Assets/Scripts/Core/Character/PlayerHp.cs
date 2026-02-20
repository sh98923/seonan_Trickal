public class PlayerHp : CharacterHp
{
    private readonly float _waveEndHpRatio = 0.2f;
    private readonly float _upgradeHpRatio = 0.3f;
    private readonly float _reviveHpRatio = 0.4f;

    private void OnEnable()
    {
        base.OnEnable();
        BattleStateManager.Instance.OnReroll += RecoverWaveEndHp;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= RecoverWaveEndHp;
    }

    private void OnDestroy()
    {
        if (BattleStateManager.Instance != null)
            BattleStateManager.Instance.OnReroll -= RecoverWaveEndHp;
    }

    private void RecoverWaveEndHp()
    {
        if (_curHp >= _maxHp) 
            return;

        float amount = _maxHp * _waveEndHpRatio;
        IncreaseHp(amount);

        print($"{_gameObjectName} 회복 : {_curHp} / {_maxHp}");
    }

    public void ApplyLevelUpHp(float newMaxHp)
    {
        bool wasFullHp = IsFullHp();

        _maxHp = newMaxHp;
        _hpSlider.maxValue = _maxHp;

        if (wasFullHp)
        {
            _curHp = _maxHp; // 풀피 유지
            UpdateHpState();
        }
        else
        {
            float amount = _maxHp * _upgradeHpRatio; // ✅ 새 maxHp 기준
            IncreaseHp(amount);
        }

        print($"{_gameObjectName} 업그레이드 : {_curHp} / {_maxHp}");
    }

    public void ReviveHp()
    {
        float amount = _maxHp * _reviveHpRatio;
        IncreaseHp(amount);

        print($"{_gameObjectName} 부활 newHp(curHp) : {_curHp} / {_maxHp}");
    }

    public void IncreaseHp(float amount)
    {
        _curHp += amount;

        if (_curHp >= _maxHp)
        {
            _curHp = _maxHp;
        }

        UpdateHpState();
    }

    private bool IsFullHp()
    {
        return _curHp == _maxHp;
    }
}