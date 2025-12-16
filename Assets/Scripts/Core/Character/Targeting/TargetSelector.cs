using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSelector : MonoBehaviour
{
    protected List<ITrackable> _characters;
    protected Character _currentTarget = null;
    public Character Target
    { 
        get { return _currentTarget; } 
    }

    protected const float _findTargetRange = 8.0f;

    private void Awake()
    {
        GameManager.Instance.EnableInScenes(this, SceneName.InGameScene);
    }

    private void Start()
    {
        _characters = InGameManager.Instance.Trackables;
    }

    private void OnEnable()
    {
        if(_currentTarget != null)
        {
            if (_currentTarget.tag == "Monster")
            { 
                _currentTarget = null;
            }
        }
    }

    protected bool IsTargetable(ITrackable trackable, string tag)
    {
        if (!trackable.IsColliderEnable)
        {
            return false;
        }

        if (!trackable.Object.activeSelf)
        {
            return false;
        }

        if (trackable.Object.tag == tag)
        {
            return false;
        }

        return true;
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