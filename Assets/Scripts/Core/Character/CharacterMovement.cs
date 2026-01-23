using UnityEngine;

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
        get { return _target.IsColliderEnable; }
    }

    private TargetSelector _targetSelector;

    protected Vector3 _dir = Vector3.zero;

    protected const float _moveSpeed = 4.0f;
    private const float _minDistanceThreshold = 0.015f;
    private const float _diagonalToleranceRatio = 0.4f; // 오차 허용 비율

    private bool _canMoveForward = false;

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
                _canMoveForward = false;
                break;

            case CharacterState.Move:
                _canMoveForward = true;
                break;

            case CharacterState.Attack:
                _canMoveForward = false;

                // 공격 중 타겟이 죽었으면 타겟 초기화 후 이동 가능하게 설정
                if (_target != null && _target.CurState == CharacterState.Dead)
                {
                    _target = null;
                    _canMoveForward = true; // 다시 이동 상태로 전환 가능
                }
                break;
        }

        // 전투 상태가 아니거나 이동 불가면 처리 종료
        if (!_canMoveForward || BattleStateManager.Instance.CurrentState == BattleState.Reroll)
            return;

        // 타겟 갱신
        _target = _targetSelector.GetTarget(_self);

        // 타겟이 존재하면 타겟 방향으로 이동, 없으면 앞으로 이동
        if (_target != null)
        {
            MoveToTarget(_target.transform.position);
        }
    }

    private void ResetForReroll()
    {
        ClearTarget();
        _canMoveForward = true;
    }

    private void MoveToTarget(Vector3 targetPos)
    {
        if (_self.CurState == CharacterState.Attack)
            return;

        Vector2 dir = GetFakeDiagonalDir(transform.position, targetPos);
        _dir = dir;

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

        // 긴 축 판단
        Vector2 finalDirection = Vector2.zero;

        bool isXLonger = distanceX > distanceY;

        float longAxis = isXLonger ? distanceX : distanceY;
        float shortAxis = isXLonger ? distanceY : distanceX;
        float ratio = shortAxis / longAxis;

        // 오차가 크면 → 긴 축 단독 이동
        if (ratio < _diagonalToleranceRatio)
        {
            finalDirection = isXLonger ? new Vector2(directionX, 0.0f) : new Vector2(0.0f, directionY);
            return finalDirection;
        }

        // 오차 범위 안일 때만 대각
        finalDirection = isXLonger ? new Vector2(directionX, directionY * ratio) : new Vector2(directionX * ratio, directionY);
        // 정규화로 속도 보정
        return finalDirection.normalized;
    }

    public void SetMovementActive(bool active)
    {
        _canMoveForward = active;
    }

    public void ClearTarget()
    {
        _target = null;
    }
}