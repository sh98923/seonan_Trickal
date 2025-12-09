using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSelector : MonoBehaviour
{
    protected List<ITrackable> _characters;
    protected Character _currentTarget = null;

    protected const float _findTargetRange = 8.0f;

    private void Awake()
    {
        _characters = InGameManager.Instance.Trackables;
    }

    // 현재 타겟이 유효한지 확인하고 반환
    protected Character GetCurrentTarget()
    {
        if (_currentTarget != null)
        {
            if (_currentTarget.GetComponent<Collider2D>().enabled &&
                _currentTarget.gameObject.activeSelf)
            {
                return _currentTarget;
            }
            else
            {
                _currentTarget = null; // 죽었거나 비활성화면 null로 초기화
            }
        }
        return null;
    }

    public abstract Character GetTarget(Character self);
}