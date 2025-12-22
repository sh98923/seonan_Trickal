using UnityEngine;

public class PlayerHpManager : MonoBehaviour
{
    private DamageReceiver _player;

    private readonly float _waveEndHpRatio = 0.2f;
    private readonly float _upgradeHpRatio = 0.3f;
    private readonly float _reviveHpRatio = 0.4f;

    private void Awake()
    {
        _player = GetComponent<DamageReceiver>();
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
        if (_player.MaxHp == _player.CurHp)
            return;

        float newHp = _player.CurHp + _player.MaxHp * _waveEndHpRatio;
        newHp = Mathf.Clamp(newHp, 0, _player.MaxHp);
        _player.SetHp(newHp);
        print($"{gameObject.name} 회복 : {newHp} / {_player.MaxHp}");
    }

    public void UpgradeHp()
    {
        // 이것도 다시 해야함 풀피인 경우
        // 업그레이드 했을 때 풀피여야하는데
        // 지금은 그렇지 않음
        // 디아나로 해보셈 그럼 알거임

        float newHp = _player.CurHp + _player.MaxHp * _upgradeHpRatio;
        newHp = Mathf.Clamp(newHp, 0, _player.MaxHp);
        _player.SetHp(newHp);
        print($"{gameObject.name} 업그레이드 : {newHp} / {_player.MaxHp}");
    }

    public void ReviveHp()
    {
        float newHp = _player.MaxHp * _reviveHpRatio;
        _player.SetHp(newHp);
        print(gameObject.name + " 부활 newHp(curHp) : " + newHp);
    }
}
