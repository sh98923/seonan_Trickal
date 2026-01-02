using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UltPanel : MonoBehaviour
{
    private enum UltUI
    {
        UltBtn = 2,
        CoolDown = 3,
        CoolTimeText = 4,
        HpBar = 5,
        MpBar = 9
    }

    private Transform[] _ultUIChildren;

    private Player _player;
    
    private Slider _hpBar;
    private Slider _mpBar;
    private Button _ultButton;
    private Image _cooldownImage;
    private TextMeshProUGUI _cooltimeText;

    private const float _ultButtonOffsetYLocked = 25.0f;  
    private const float _ultButtonOffsetYUnlocked = -25.0f;   
        
    private const float _statusBarOffsetYLocked = 75.0f;  
    private const float _statusBarOffsetYUnlocked = -100.0f;

    private float _remainCooldown = 0.0f;
    private bool _isCooldown = false;
    private bool _isPaused = false; 
    private bool _isUltIconSet = false;
    private bool _isUltUnlock = false;

    private void Awake()
    {
        CacheUI();
    }

    private void Start()
    {
        _ultButton.onClick.AddListener(OnClickUltimate);
    }

    private void CacheUI()
    {
        _ultUIChildren = GetComponentsInChildren<Transform>(true);

        _hpBar = _ultUIChildren[(int)UltUI.HpBar].GetComponent<Slider>();
        _mpBar = _ultUIChildren[(int)UltUI.MpBar].GetComponent<Slider>();
        _ultButton = _ultUIChildren[(int)UltUI.UltBtn].GetComponent<Button>();
        _cooldownImage = _ultUIChildren[(int)UltUI.CoolDown].GetComponent<Image>();
        _cooltimeText = _ultUIChildren[(int)UltUI.CoolTimeText].GetComponent<TextMeshProUGUI>();
    }

    private void OnClickUltimate()
    {
        if (!BattleUnitManager.Instance.IsUltimateUnlocked(_player.Data.PlayerKey))
            return;

        if (_isCooldown)
            return;

        //궁극기 발동
        _remainCooldown = _player.Data.UltCoolTime;
        StartCoroutine(StartCooldown(_player.Data.UltCoolTime));
    }

    private IEnumerator StartCooldown(float cooldownTime)
    {
        _isCooldown = true;
        _ultButton.interactable = false;

        _cooldownImage.fillAmount = 1.0f;
        _cooltimeText.gameObject.SetActive(true);

        while (_remainCooldown > 0.0f)
        {
            if (!_isPaused)
            {
                _remainCooldown -= Time.deltaTime;
                _cooldownImage.fillAmount = _remainCooldown / _player.Data.UltCoolTime;
                _cooltimeText.text = Mathf.CeilToInt(_remainCooldown).ToString();
            }

            yield return null;
        }

        _cooldownImage.fillAmount = 0.0f;
        _cooltimeText.text = string.Empty;
        _cooltimeText.gameObject.SetActive(false);

        _ultButton.interactable = true;
        _isCooldown = false;
    }

    // 쿨타임 흐름 멈춤
    public void PauseCooldown()
    {
        _isPaused = true;
    }

    // 쿨타임 다시 흐름
    public void ResumeCooldown()
    {
        _isPaused = false;
    }

    public void UpdateHpBar(float curHp)
    {
        _hpBar.value = curHp;
    }

    public void UpdateMpBar(float curMp)
    {
        _mpBar.value = curMp;
    }

    public void BindPlayer(Player player)
    {
        if (_player != null)
        {
            _player.PlayerHealth.OnHpChanged -= UpdateHpBar;
            _player.PlayerMana.OnMpChanged -= UpdateMpBar;
        }

        _player = player;

        _player.PlayerHealth.OnHpChanged += UpdateHpBar;
        _player.PlayerMana.OnMpChanged += UpdateMpBar;

        // 초기값 갱신
        InitStatusBar();
        // 처음 한번 아이콘 로드
        SetUltIcon();

        if (BattleUnitManager.Instance.IsUltimateUnlocked(_player.Data.PlayerKey))
        {
            _cooldownImage.fillAmount = 0.0f;
            
            if(!_isUltUnlock)
            {
                SetUIForUltUnlocked();
                _isUltUnlock = true;
            }
        }
    }

    private void SetUltIcon()
    {
        if (_isUltIconSet || _player == null)
            return;

        Sprite ultSprite = Resources.Load<Sprite>(_player.Data.UltIcon);
        if (ultSprite != null)
            _ultButton.image.sprite = ultSprite;

        _isUltIconSet = true;

        SetUIForUltLocked();
    }

    private void InitStatusBar()
    {
        _hpBar.maxValue = _player.PlayerHealth.MaxHp;
        _mpBar.maxValue = _player.PlayerMana.MaxMp;

        UpdateHpBar(_player.PlayerHealth.CurHp);
        UpdateMpBar(_player.PlayerMana.CurMp);
    }

    private void SetUIForUltUnlocked()
    {
        SetUIOffsets(_ultButtonOffsetYUnlocked, _statusBarOffsetYUnlocked);
    }

    private void SetUIForUltLocked()
    {
        SetUIOffsets(_ultButtonOffsetYLocked, _statusBarOffsetYLocked);
    }

    private void SetUIOffsets(float buttonOffsetY, float statusOffsetY)
    {
        RectTransform btn = _ultButton.GetComponent<RectTransform>();
        RectTransform hp = _hpBar.GetComponent<RectTransform>();
        RectTransform mp = _mpBar.GetComponent<RectTransform>();

        btn.anchoredPosition += new Vector2(0, buttonOffsetY);
        hp.anchoredPosition += new Vector2(0, statusOffsetY);
        mp.anchoredPosition += new Vector2(0, statusOffsetY);
    }
}