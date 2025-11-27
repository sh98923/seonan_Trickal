using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private DamageReceiver _damageReceiver;

    private readonly float _waveEndHpRatio = 0.2f;
    private readonly float _upgradeHpRatio = 0.3f;
    private readonly float _reviveHpRatio = 0.4f;

    private void Awake()
    {
        _damageReceiver = GetComponent<DamageReceiver>();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnReroll += RecoverWaveEndHp;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= RecoverWaveEndHp;
    }

    private void OnDestroy()
    {
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnReroll -= RecoverWaveEndHp;
        }
    }

    private void RecoverWaveEndHp()
    {
        // 풀피라면 리턴
        if (_damageReceiver.MaxHp == _damageReceiver.CurHp)
            return;

        float newHp = _damageReceiver.CurHp + _damageReceiver.MaxHp * _waveEndHpRatio;
        newHp = Mathf.Clamp(newHp, 0, _damageReceiver.MaxHp);
        _damageReceiver.SetHp(newHp);
        print($"{gameObject.name} 회복 : {newHp} / {_damageReceiver.MaxHp}");
    }

    public void UpgradeHp()
    {
        float newHp = _damageReceiver.CurHp + _damageReceiver.MaxHp * _upgradeHpRatio;
        newHp = Mathf.Clamp(newHp, 0, _damageReceiver.MaxHp);
        _damageReceiver.SetHp(newHp);
        print($"{gameObject.name} 업그레이드 : {newHp} / {_damageReceiver.MaxHp}");
    }

    public void ReviveHp()
    {
        float newHp = _damageReceiver.MaxHp * _reviveHpRatio;
        _damageReceiver.SetHp(newHp);
        print(gameObject.name + " 부활 newHp(curHp) : " + newHp);
    }
}
