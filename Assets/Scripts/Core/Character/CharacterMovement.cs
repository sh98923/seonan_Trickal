using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class CharacterMovement : MonoBehaviour
{
    private Character _self;
    private Character _target;
    public Character Target
    {
        get { return _target; }
    }
    public bool TargetColliderEnable
    {
        get { return _target.GetComponent<Collider2D>().enabled; }
    }

    private TargetSelector _targetSelector;

    private Vector3 _dir = Vector3.zero;

    protected const float _moveSpeed = 4.0f;

    private bool _isMoving = false;
    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _target != null; }
    }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "InGameScene")
        {
            this.enabled = false;
        }

        _self = GetComponent<Character>();
        _targetSelector = GetComponent<TargetSelector>();
    }

    protected void Update()
    {
        if (BattleStateManager.Instance.CurrentState == BattleState.Reroll)
            return;

        if (BattleStateManager.Instance.CurrentState == BattleState.EnteringReroll)
        {
            HandleEnteringRerollState();
            return;
        }

        HandleBattleState();
    }

    private void HandleBattleState()
    {
        switch (_self.CurState)
        {
            case CharacterState.Idle:
                _isMoving = false;
                break;

            case CharacterState.Move:
                _isMoving = true;
                break;

            case CharacterState.Attack:
                // 공격 중에는 이동 멈춤
                _isMoving = false;

                // 공격 중 타겟이 죽었으면 타겟 초기화 후 이동 가능하게 설정
                if (_target != null && _target.CurState == CharacterState.Dead)
                {
                    _target = null;
                    _isMoving = true; // 다시 이동 상태로 전환 가능
                }
                break;

            case CharacterState.Dead:
                _target = null;
                _isMoving = false;
                return; // Dead면 이후 처리 필요 없음
        }

        // 전투 상태가 아니거나 이동 불가면 처리 종료
        if (!_isMoving || BattleStateManager.Instance.CurrentState == BattleState.Reroll)
            return;

        // 타겟 갱신
        _target = _targetSelector.GetTarget(_self);

        // 타겟이 존재하면 타겟 방향으로 이동, 없으면 앞으로 이동
        if (_target != null)
        {
            _isMoving = false; // 타겟이 생기면 이동 멈춤
            MoveToward(_target.transform.position);
        }
        else
        {
            _isMoving = true;
            MoveForward();
        }
    }

    private void HandleEnteringRerollState()
    {
        _target = null;
        _isMoving = true;
    }

    private void MoveToward(Vector3 targetPos)
    {
        _dir = (targetPos - transform.position).normalized;
        transform.Translate(_dir * _moveSpeed * Time.deltaTime);
    }

    private void MoveForward()
    {
        if (_self.CompareTag("Player"))
        {
            _dir = Vector3.right;
        }
        else if (_self.CompareTag("Monster"))
        {
            _dir = Vector3.left;
        }

        transform.Translate(_dir * _moveSpeed * Time.deltaTime);
    }

    public void SetMovementActive(bool active)
    {
        _isMoving = active;
    }

    /*private void EnteringRerollMovement()
    {
        Vector3 dir = _nextWayPoint - transform.position;
        float distanceCurFrame = _moveSpeed * Time.deltaTime;

        if (dir.magnitude <= distanceCurFrame)
        {
            transform.position = _nextWayPoint;
            if (!_isArrived)
            {
                _isArrived = true;
                _isEnteringReroll = false;
                _isInBattle = false;
                InGamePlayerSpawn parent = transform.parent.GetComponent<InGamePlayerSpawn>();
                parent.CheckNextWaveReady();
            }
        }
        else
        {
            transform.Translate(dir.normalized * distanceCurFrame);
        }
    }*/
}