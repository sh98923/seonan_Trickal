using UnityEngine;

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
        if (_curHp >= _maxHp) return;
        float amount = _maxHp * _waveEndHpRatio;
        IncreaseHp(amount);

        print($"{_gameObjectName} 회복 : {_curHp} / {_maxHp}");
    }

    public void UpgradeHp()
    {
        // 이것도 다시 해야함 풀피인 경우
        // 업그레이드 했을 때 풀피여야하는데
        // 지금은 그렇지 않음
        // 디아나로 해보셈 그럼 알거임

        float amount = _maxHp * _upgradeHpRatio;
        IncreaseHp(amount);

        print($"{_gameObjectName} 업그레이드 : {_curHp} / {_maxHp}");
    }

    public void ReviveHp()
    {
        float amount = _maxHp * _reviveHpRatio;
        IncreaseHp(amount);

        print($"{_gameObjectName} 부활 newHp(curHp) : {_curHp} / {_maxHp}");
    }

    private void IncreaseHp(float amount)
    {
        _curHp += amount;

        if (_curHp >= _maxHp)
        {
            _curHp = _maxHp;
        }

        UpdateHpState();
    }
}