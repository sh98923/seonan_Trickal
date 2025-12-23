using UnityEngine;

public class Monster : Character
{
    private CharacterHp _monsterHp;

    private float _atk = 0.0f;

    private void Awake()
    {
        base.Awake();

        _monsterHp = GetComponentInChildren<CharacterHp>();

        TargetSelector melee = GetComponent<MonsterTargetSelector>();

        if (melee != null)
        {
            _targets[(int)ActionSlot.Attack] = melee;
        }
        else
        {
            Debug.LogError($"{name} : 몬스터 타겟팅 스크립트 없음");
        }
    }

    // Idle 상태면 Battle모드일 때 move로 넘어옴
    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        if (_movement.HasTarget)
        {
            // 타겟이 사거리 안에 들어오면 공격 상태로
            float dist = Vector2.Distance(transform.position, _movement.Target.transform.position);
            if (dist <= _data.AtkRange)
            {
                _animator.SetIdle(true);
                _movement.SetMovementActive(false);
                _curState = CharacterState.Attack;
            }
        }
        else
        {
            _movement.SetMovementActive(true);
        }
    }

    public override void SetCharacterData(CharacterData data)
    {
        _data = data;
    }

    public override void OnAttack()
    {
        _action[(int)ActionCategory.Attack].SetAttackInfo(_movement.Target, _actionType, _atk);

        base.OnAttack();
    }

    public void WaveUpgrade(int wave)
    {
        float maxHp = 0.0f;
        float hpLinear = _data.Hp + _data.HpPerWave * wave;
        float hpExp = _data.Hp * Mathf.Pow(_data.HpGrowthRate, wave);

        // 두 가지의 공식을 평균 낸 값 (선형, 지수)
        maxHp = (hpLinear + hpExp) * 0.5f;
        // Round가 정수로만 반올림 하기 떄문에
        // 소수점 첫째자리까지 나오게 하기위한 수식
        maxHp = Mathf.Round(maxHp);

        _monsterHp.InitializeHp(maxHp);

        float atkLinear = _data.Atk + _data.AtkPerWave * wave;
        float atkExp = _data.Atk * Mathf.Pow(_data.AtkGrowthRate, wave);
        _atk = (atkLinear + atkExp) * 0.5f;
        _atk = Mathf.Round(_atk);

        //print("몬스터 : " + _data.EngName + " : " + _maxHp.ToString("F1") + ", " + _atk.ToString("F1"));
    }
}