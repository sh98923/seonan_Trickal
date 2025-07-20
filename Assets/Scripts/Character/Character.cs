using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Character : MonoBehaviour
{
    protected enum State
    {
        Idle, Move, Attack, Dead
    }

    private SortingGroup _sortingGroup;
    private Animator _animator;
    protected Transform _target;

    protected Vector2 _moveDir;
    protected State _curState = State.Idle;

    protected string _type = "";
    protected readonly float _colliderOffset = 0.5f;
    protected float _hp = 0.0f;
    protected float _atk = 0.0f;
    protected float _criRate = 0.0f;
    protected float _range = 0.0f;
    protected float _atkCoolTime = 0.0f;
    protected float _moveSpeed = 2.0f;
    private float _attackCoolTimer = 0.0f;
    private float _animLength = 0.0f;

    private bool _isAttacking = false;

    protected void Awake() 
    { 
        _animator = GetComponent<Animator>();
        _sortingGroup = GetComponent<SortingGroup>();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _curState = State.Idle;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _curState = State.Move;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _curState = State.Attack;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _curState = State.Dead;
        }

        switch (_curState)
        {
            case State.Idle:
                IdleStateAction();
                break;
            case State.Move:
                MoveStateAction();
                break;
            case State.Attack:
                AttackStateAction();
                AttackCoolTime();
                break;
            case State.Dead:
                DeadStateAction();
                break;
        }

        _sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    protected virtual void IdleStateAction()
    {
        _animator.SetBool("IsInRanged", true);
    }

    protected virtual void MoveStateAction()
    {
        _animator.SetBool("IsInRanged", false);
    }

    protected virtual void AttackStateAction()
    {
        if (_isAttacking)
            return;

        _isAttacking = true;
        _animator.SetTrigger("Attack");

        _animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private void AttackCoolTime()
    {
        if (_isAttacking)
        {
            _attackCoolTimer += Time.deltaTime;

            if (_attackCoolTimer >= _animLength)
            {
                _attackCoolTimer -= _animLength;
                _isAttacking = false;
            }
        }
    }

    protected virtual void DeadStateAction()
    {
        _animator.SetTrigger("Dead");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        
        Vector3 pos = transform.position;
        pos.y += _colliderOffset;

        Gizmos.DrawWireSphere(pos, _range);
    }
}