using System.Collections;
using UnityEngine;

public class Player : Character
{
    private Vector3 _originPos;

    private PlayerStatData _data;

    private float _curMp = 0.0f;

    private void Awake()
    {
        base.Awake();
        _moveDir = Vector2.right;
    }

    protected override void IdleStateAction()
    {
        base.IdleStateAction();

        if (BattleStateManager.Instance.IsBattle)
        {
            StartCoroutine(RegenerateMp());
            _curState = State.Move;
        }
    }

    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        if (BattleStateManager.Instance.CurrentState == BattleState.MonstersDefeated)
        {
            Vector3 dir = _originPos - transform.position;
            transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _originPos) < 0.01f)
            {
                BattleStateManager.Instance.SetState(BattleState.Reroll);
                _curState = State.Idle;
            }
        }
        else
        {
            if (_targetCollider == null)
            {
                _targetCollider = FindTarget("Enemy", _findTargetRange);
                transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
            }
            else
            {
                if (FindTarget("Enemy", _attackRange) == null)
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
    }

    protected override void AttackStateAction()
    {
        if (!_targetCollider.enabled)
        {
            _targetCollider = null;
            _curState = State.Move;
        }

        // 몬스터 전멸
        if (BattleStateManager.Instance.CurrentState == BattleState.MonstersDefeated)
        {
            _curState = State.Move;
            return;
        }

        if (_isAttacking)
            return;

        _isAttacking = true;

        if (_curMp >= _data.Mp)
        {
            _animator.SetTrigger("Skill");
        }
        else
        {
            _animator.SetTrigger("Attack");
        }
    }

    public void OnAttackHit()
    {
        _targetCollider.GetComponent<Monster>().TakeDamage(_data.Atk * 119);
    }

    public void OnSkillHit()
    {
        float atk = _data.Atk * _data.BaseSkill;
        _curMp -= _data.Mp;
        _targetCollider.GetComponent<Monster>().TakeDamage(atk * 119);
    }

    private IEnumerator RegenerateMp()
    {
        while (BattleStateManager.Instance.IsBattle)
        {
            if(_curMp >= _data.Mp)
            {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            _curMp += 20.0f;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void SetPlayerStat(PlayerStatData data)
    {
        _data = data;
        _curHp = _data.Hp * 10;
        _attackRange = _data.Range;
        _atkCoolTime = data.AtkCoolTime;
        _originPos = transform.position;
    }
}