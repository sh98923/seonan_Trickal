using UnityEngine;

public class PlayerMovement : CharacterMovement , IWaveMoveable
{
    private Vector3 _playerPos = new Vector3();
    private Vector3 _nextWayPoint = new Vector3();
    private Vector2 _updatePos = new Vector2();

    private float _camPosX = 0.0f;
    private float _nextWaveOffsetPosX = 0.0f;

    private float _distanceCurFrame;
    private float _nextDestX = 0.0f; // 배틀 → 리롤 이동 시 플레이어 이동 거리 변수

    private bool _isArrived = false;

    private void OnEnable()
    {
        base.OnEnable();
        BattleStateManager.Instance.OnEnteringReroll += MoveToNextWaypoint;
    }

    private void OnDisable()
    {
        base.OnDisable();
        BattleStateManager.Instance.OnEnteringReroll -= MoveToNextWaypoint;
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnEnteringReroll -= MoveToNextWaypoint;
        }
    }

    public void InitWavePosition()
    {
        _camPosX = InGameCam.Instance.CamOriginalPosX;
        _playerPos = BattleUnitManager.Instance.GetOriginalPos(gameObject.name);

        _nextWayPoint.y = _playerPos.y;
        _nextWaveOffsetPosX = _playerPos.x - _camPosX;
    }

    private void MoveToNextWaypoint()
    {
        _nextWayPoint.x = _nextWaveOffsetPosX + _nextDestX;
    }

    public void UpdateNextDestX(float destX)
    {
        _nextDestX = destX;
    }

    public void UpdateSpawnX(float destX)
    {
        if (!_self.Data.IsDeployed)
        { 
            _nextDestX = destX;
        }

        _updatePos = transform.position;
        _updatePos.x = _nextWaveOffsetPosX + destX;
        _updatePos.y = _playerPos.y;
    }

    public void SetRevivePos()
    {
        transform.position = _updatePos;
    }

    public void EnteringRerollMovement()
    {
        _dir = _nextWayPoint - transform.position;
        _distanceCurFrame = _moveSpeed * Time.deltaTime;

        if (_dir.magnitude <= _distanceCurFrame)
        {
            // 목표 위치 도착
            transform.position = _nextWayPoint;
            _isArrived = true;
        }
        else
        {
            // 목표를 향해 이동
            transform.Translate(_dir.normalized * _distanceCurFrame);
            _isArrived = false;
        }
    }

    public bool IsArrived()
    {
        return _isArrived;
    }
}