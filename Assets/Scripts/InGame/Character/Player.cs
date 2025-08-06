using UnityEngine;
using System.Collections;

public class Player : Character
{
    private Vector3 _originPos;

    private float _curMp = 0.0f;

    [SerializeField]
    private GameObject _skillEffect;

    private void Awake()
    {
        base.Awake();
        _moveDir = Vector2.right;

        _skillEffect.SetActive(false);

        _atk = 5f;
        _curHp = 5500;
    }

    protected override void IdleStateAction()
    {
        base.IdleStateAction();

        if (BattleStateManager.Instance.IsBattle)
        {
            StartCoroutine(RegenerateMp());
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
        else if (BattleStateManager.Instance.CurrentState == BattleState.Battle)
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

        if (_curMp >= _characterData.Mp)
        {
            _animator.SetTrigger("Skill");
        }
        else
        {
            _animator.SetTrigger("Attack");
        }
    }

    /*public void OnAttackHit()
    {
        _targetCollider.GetComponent<Monster>().TakeDamage(_characterData.Atk);
    }*/

    public void OnSkillHit()
    { 
        float skillAtk = _characterData.Atk * _characterData.SkillRate;
        _curMp -= _characterData.Mp;
        _attack.Skill(_targetCollider, skillAtk);
        Debug.Log("skilldamage " + skillAtk);
    }

    public void PlaySkillEffect()
    {
        _skillEffect.SetActive(true);
        _skillEffect.GetComponent<Animator>()?.Play("SkillEffect");
        StartCoroutine(DisableEffectAfterTime(1.0f)); 
    }

    private IEnumerator DisableEffectAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        _skillEffect.SetActive(false);
    }

    private IEnumerator RegenerateMp()
    {
        while (BattleStateManager.Instance.IsBattle)
        {
            if (_curMp >= _characterData.Mp)
            {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            _curMp += 20.0f;
            yield return new WaitForSeconds(1.0f);
        }
    }
}