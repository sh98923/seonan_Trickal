using System;
using UnityEngine;

public class CharacterHp : MonoBehaviour, IDamageableHealth
{
    protected event Action<float, float> _onHpchanged;
    public event Action<float, float> OnHpChanged
    {
        add { _onHpchanged += value; }
        remove { _onHpchanged -= value; }
    }

    protected string _gameObjectName;

    protected float _curHp = 0.0f;
    public float CurHp
    {
        get { return _curHp; }
    }

    protected float _maxHp = 0.0f;
    public float MaxHp
    {
        get { return _maxHp; }
    }

    private bool _isInitialized = false;

    private void Awake()
    {
        GameManager.Instance.EnableScriptInScenes(this, SceneName.InGameScene);
    }

    private void Start()
    {
        Character character = GetComponentInParent<Character>();
        _gameObjectName = character.gameObject.name;
    }

    protected void OnEnable()
    {
        if (_isInitialized)
        { 
            UpdateHpState();
        }
    }

    protected void UpdateHpState()
    {
        _onHpchanged?.Invoke(_curHp, _maxHp);
    }

    // 최초 1회만 호출
    public void InitializeHp(float maxHp)
    {
        _isInitialized = true;
        _maxHp = maxHp;
        _curHp = maxHp;
        UpdateHpState();
    }

    // 레벨업 / 스탯 변경용
    public void UpdateMaxHp(float newMaxHp)
    {
        bool wasFullHp = _curHp >= _maxHp;

        _maxHp = newMaxHp;

        if (wasFullHp)
        {
            _curHp = _maxHp;
        }
        else if (_curHp > _maxHp)
        {
            _curHp = _maxHp;
        }

        UpdateHpState();
    }

    public void DecreaseHp(float amount)
    {
        _curHp -= amount;

        if (_curHp <= 0.0f)
        {
            _curHp = 0.0f;
        }

        UpdateHpState();
    }
}