using UnityEngine;

public class Monster : Character
{
    private float _finalHp;
    private float _finalAtk;

    private void Awake()
    {
        base.Awake();
        _moveDir = Vector2.left;
    }

    private void Update()
    {
        base.Update();
        _target = FindPlayer();
    }

    protected override void IdleStateAction()
    {
        base.IdleStateAction();

        if (BattleStateManager.Instance.IsBattleStart)
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
            if (_type == "Melee" &&
                Mathf.Abs(_target.transform.position.y - transform.position.y) > 0.75f)
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

    private Transform FindPlayer()
    {
        Vector3 pos = transform.position;
        pos.y += _colliderOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, _range);

        float closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPlayer = hit.transform;
                }
            }
        }

        return closestPlayer;
    }

    public void SetMonsterStat(MonsterData data, int wave)
    {
        _type = data.Type;

        _criRate = data.CriRate;
        _range = data.Range;
        _atkCoolTime = data.AtkCoolTime;

        float hpLinear = data.Hp + data.HpPerWave * wave;
        float hpExp = data.Hp * Mathf.Pow(data.HpGrowthRate, wave);

        // 두 가지의 공식을 평균 낸 값 (선형, 지수)
        _finalHp = (hpLinear + hpExp) * 0.5f;
        // Round가 정수로만 반올림 하기 떄문에
        // 소수점 첫째자리까지 나오게 하기위한 수식
        _finalHp = Mathf.Round(_finalHp * 10f) * 0.1f;

        float atkLinear = data.Atk + data.AtkPerWave * wave;
        float atkExp = data.Atk * Mathf.Pow(data.AtkGrowthRate, wave);
        _finalAtk = (atkLinear + atkExp) * 0.5f;
        _finalAtk = Mathf.Round(_finalAtk * 10f) * 0.1f;

        print(data.Name + " : " + _finalHp.ToString("F1") + ", " + _finalAtk.ToString("F1"));
    }
}