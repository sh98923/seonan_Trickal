using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    private Vector3 _dir;
    private float _distanceCurFrame;
    private bool _isArrived = false;

    public void EnteringRerollMovement(Vector3 nextWayPoint)
    {
        _dir = nextWayPoint - transform.position;
        _distanceCurFrame = _moveSpeed * Time.deltaTime;

        if (_dir.magnitude <= _distanceCurFrame)
        {
            // 목표 위치 도착
            transform.position = nextWayPoint;
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