using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHp : MonoBehaviour
{
    private event Action<bool> _onHpZero;
    public event Action<bool> OnHpZero
    {
        add { _onHpZero += value; }
        remove { _onHpZero -= value; }
    }

    private event Action<float> _onHpChanged;
    public event Action<float> OnHpChanged
    {
        add { _onHpChanged += value; }
        remove { _onHpChanged -= value; }
    }

    private Slider _hpSlider;

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
        _hpSlider = GetComponent<Slider>();
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
        if (_hpSlider != null)
        {
            _hpSlider.value = _curHp;
            _onHpChanged?.Invoke(_curHp);
        }
    }

    public void InitializeHp(float maxHp)
    {
        _isInitialized = true;
        _maxHp = maxHp;
        _curHp = maxHp;

        _hpSlider.maxValue = _maxHp;
        UpdateHpState();
    }

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
        if (_curHp <= 0.0f) return;

        _curHp -= amount;

        if (_curHp <= 0.0f)
        {
            _curHp = 0.0f;
            UpdateHpState();
            _onHpZero?.Invoke(false);
            return;
        }

        UpdateHpState();
    }
}
