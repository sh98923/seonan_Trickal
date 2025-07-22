using UnityEngine;

public class Monster : Character
{
    private void Awake()
    {
        base.Awake();
        _moveDir = Vector2.left;
    }

    private void Update()
    {
        base.Update();
        _target = FindTarget("Player", _findTargetRange);
    }

    protected override void IdleStateAction()
    {
        base.IdleStateAction();

        if (BattleStateManager.Instance.IsBattle)
        {
            _curState = State.Move;
        }
    }

    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        if(_target == null)
        {
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
        }
        else
        {
            if (FindTarget("Player", _attackRange) == null)
            {
                Vector2 dir = _target.transform.position - transform.position;
                transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);
            }
            else
            {
                _curState = State.Attack;
            }
        }
    }

    protected override void AttackStateAction()
    {
        base.AttackStateAction();

        if(_target == null)
        {
            _curState = State.Move;
        }
    }

    public void OnAttackHit()
    {
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange);

        foreach (Collider2D hitTarget in hitTargets)
        {
            if (hitTarget.CompareTag("Player"))
            {
                Character player = hitTarget.GetComponent<Character>();
                if (player != null)
                {
                    player.TakeDamage(_atk);
                }
            }
        }
    }


    public override void SetCharacterStat(MonsterData data, int wave)
    {
        base.SetCharacterStat(data, wave);

        float hpLinear = data.Hp + data.HpPerWave * wave;
        float hpExp = data.Hp * Mathf.Pow(data.HpGrowthRate, wave);

        // 두 가지의 공식을 평균 낸 값 (선형, 지수)
        _hp = (hpLinear + hpExp) * 0.5f;
        // Round가 정수로만 반올림 하기 떄문에
        // 소수점 첫째자리까지 나오게 하기위한 수식
        _hp = Mathf.Round(_hp * 10f) * 0.1f;

        float atkLinear = data.Atk + data.AtkPerWave * wave;
        float atkExp = data.Atk * Mathf.Pow(data.AtkGrowthRate, wave);
        _atk = (atkLinear + atkExp) * 0.5f;
        _atk = Mathf.Round(_atk * 10f) * 0.1f;

        print(data.Name + " : " + _hp.ToString("F1") + ", " + _atk.ToString("F1"));
    }
}