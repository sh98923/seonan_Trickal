using UnityEngine;

public class Monster : Character
{
    private ProjectilePool _projectilePool;

    private void Awake()
    {
        base.Awake();
        _moveDir = Vector2.left;
    }

    private void Start()
    {
        _projectilePool = FindObjectOfType<ProjectilePool>();
    }

    private void Update()
    {
        base.Update();
        _targetCollider = FindTarget("Player", _findTargetRange);
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

        if(_targetCollider == null)
        {
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
        }
        else
        {
            if (FindTarget("Player", _attackRange) == null)
            {
                Vector2 dir = _targetCollider.transform.position - transform.position;
                transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);
            }
            else
            {
                _animator.SetBool("IdleState", true);
                _curState = State.Attack;
            }
        }
    }

    protected override void AttackStateAction()
    {
        base.AttackStateAction();

        if(_targetCollider == null)
        {
            _curState = State.Move;
        }
    }

    public void OnAttackHit()
    {
        if(_type == "Range")
        {
            if (_projectilePool == null || _targetCollider == null)
                return;

            Vector3 pos = _targetCollider.transform.position;
            pos.y += _colliderOffset;

            Vector2 direction = (pos - _attackPoint.position).normalized;

            Projectile proj = _projectilePool.Get(_attackPoint.position, direction, _atk);
            proj.SetPool(_projectilePool);
        }

        else
        {
            _targetCollider.GetComponent<Player>().TakeDamage(_atk);
        }

        /*Collider2D[] hitTargets = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange);

        foreach (Collider2D hitTarget in hitTargets)
        {
            if (hitTarget.CompareTag("Player"))
            {
                Player player = hitTarget.GetComponent<Player>();
                if (player != null)
                {
                    player.TakeDamage(_atk);
                }
            }
        }*/
    }

    /*public void SetMonsterStat(MonsterData data, int wave)
    {
        _type = data.Type;
        _criRate = data.CriRate;
        _attackRange = data.Range;
        _atkCoolTime = data.AtkCoolTime;

        float hpLinear = data.Hp + data.HpPerWave * wave;
        float hpExp = data.Hp * Mathf.Pow(data.HpGrowthRate, wave);

        // 두 가지의 공식을 평균 낸 값 (선형, 지수)
        _maxHp = (hpLinear + hpExp) * 0.5f;
        // Round가 정수로만 반올림 하기 떄문에
        // 소수점 첫째자리까지 나오게 하기위한 수식
        _maxHp = Mathf.Round(_maxHp * 10f) * 0.1f;
        _curHp = _maxHp;

        float atkLinear = data.Atk + data.AtkPerWave * wave;
        float atkExp = data.Atk * Mathf.Pow(data.AtkGrowthRate, wave);
        _atk = (atkLinear + atkExp) * 0.5f;
        _atk = Mathf.Round(_atk * 10f) * 0.1f;

        print(data.Name + " : " + _curHp.ToString("F1") + ", " + _atk.ToString("F1"));
    }*/
}