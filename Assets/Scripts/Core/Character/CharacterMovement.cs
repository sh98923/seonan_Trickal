using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    protected Character _self;
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

    protected Vector3 _dir = Vector3.zero;

    private const float _minDistanceThreshold = 0.001f;
    protected const float _moveSpeed = 4.0f;

    private bool _isMoving = false;
    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _target != null; }
    }

    private void Awake()
    {
        GameManager.Instance.EnableScriptInScenes(this, SceneName.InGameScene);

        _self = GetComponent<Character>();
    }

    private void Start()
    {
        _targetSelector = _self.GetSelector(ActionSlot.Attack);
    }

    protected void OnEnable()
    {
        _target = null;
        BattleStateManager.Instance.OnEnteringReroll += ResetForReroll;
    }

    protected void OnDisable()
    {
        BattleStateManager.Instance.OnEnteringReroll -= ResetForReroll;
    }

    protected void OnDestroy()
    {
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnEnteringReroll -= ResetForReroll;
        }
    }

    protected void Update()
    {
        switch(BattleStateManager.Instance.CurrentState)
        {
            case BattleState.Reroll:
            case BattleState.EnteringReroll:
                return;
        }

        UpdateCombatMovement();
    }

    private void UpdateCombatMovement()
    {
        switch (_self.CurState)
        {
            case CharacterState.Idle:
            case CharacterState.Dead:
                _isMoving = false;
                break;

            case CharacterState.Move:
                _isMoving = true;
                break;

            case CharacterState.Attack:
                _isMoving = false;

                // 공격 중 타겟이 죽었으면 타겟 초기화 후 이동 가능하게 설정
                if (_target != null && _target.CurState == CharacterState.Dead)
                {
                    _target = null;
                    _isMoving = true; // 다시 이동 상태로 전환 가능
                }
                break;
        }

        // 전투 상태가 아니거나 이동 불가면 처리 종료
        if (!_isMoving || BattleStateManager.Instance.CurrentState == BattleState.Reroll)
            return;

        // 타겟 갱신
        _target = _targetSelector.GetTarget(_self);

        // 타겟이 존재하면 타겟 방향으로 이동, 없으면 앞으로 이동
        if (_target != null && _isMoving)
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

    private void ResetForReroll()
    {
        _target = null;
        _isMoving = true;
    }

    private void MoveToward(Vector3 targetPos)
    {
        Vector2 dir = GetFakeDiagonalDir(transform.position, targetPos);
        _dir = dir;

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

    protected Vector2 GetFakeDiagonalDir(Vector2 curPos, Vector2 targetPos)
    {
        Vector2 delta = targetPos - curPos;

        float distanceX = Mathf.Abs(delta.x);
        float distanceY = Mathf.Abs(delta.y);

        if (distanceX < _minDistanceThreshold && distanceY < _minDistanceThreshold)
            return Vector2.zero;

        float directionX = Mathf.Sign(delta.x);
        float directionY = Mathf.Sign(delta.y);

        if (distanceX > distanceY)
        {
            float ratio = distanceY / distanceX;
            return new Vector2(directionX, directionY * ratio);
        }
        else
        {
            float ratio = distanceX / distanceY;
            return new Vector2(directionX * ratio, directionY);
        }
    }

    public void SetMovementActive(bool active)
    {
        _isMoving = active;
    }
}