using UnityEngine;

public class CharacterStatusBar : MonoBehaviour
{
    protected CharacterHp _hp;
    protected Transform _hpBar;
    
    private Transform _parent;
    private float _prevParentSign;

    private void Awake()
    {
        GameManager.Instance.SetActiveGameObjectInScenes(gameObject, SceneName.InGameScene);

        _hp = GetComponentInChildren<CharacterHp>();
        _parent = transform.parent;

        if (_parent != null)
        {
            _prevParentSign = Mathf.Sign(_parent.lossyScale.x);
        }
    }

    private void LateUpdate()
    {
        if (_parent == null) return;

        float currentSign = Mathf.Sign(_parent.lossyScale.x);

        if (!Mathf.Approximately(currentSign, _prevParentSign))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            _prevParentSign = currentSign;
        }
    }

    private void OnEnable()
    {
        if (_hp != null)
            _hp.OnHpZero += SetActiveBar;
    }

    private void OnDisable()
    {
        if (_hp != null)
            _hp.OnHpZero -= SetActiveBar;
    }

    public void SetActiveBar(bool active)
    {   
        gameObject.SetActive(active);
    }
}