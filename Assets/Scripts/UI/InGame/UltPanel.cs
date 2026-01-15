using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UltPanel : MonoBehaviour
{
    private struct UIBasePos
    {
        public Vector2 button;
        public Vector2 hpBar;
        public Vector2 mpBar;

        public UIBasePos(Vector2 btn, Vector2 hp, Vector2 mp)
        {
            button = btn;
            hpBar = hp;
            mpBar = mp;
        }
    }

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
    private Coroutine _cooldownRoutine = null;
    private TextMeshProUGUI _cooltimeText;

    private UIBasePos _basePos;

    private const float _ultButtonOffsetYLocked = 25.0f;  
    private const float _ultButtonOffsetYUnlocked = 0.0f;   
        
    private const float _statusBarOffsetYLocked = 75.0f;  
    private const float _statusBarOffsetYUnlocked = -25.0f;
    public int BoundPlayerKey
    {
        get
        {
            if (_player != null)
                return _player.Data.PlayerKey;

            return -1;
        }
    }
    private float _remainCooldown = 0.0f;
    private bool _isPaused = false; 
    private bool _isCooldown = false;
    private bool _isUltIconSet = false;

    private void Awake()
    {
        CacheUI();

        _basePos = new UIBasePos()
        {
            button = _ultButton.GetComponent<RectTransform>().anchoredPosition,
            hpBar = _hpBar.GetComponent<RectTransform>().anchoredPosition,
            mpBar = _mpBar.GetComponent<RectTransform>().anchoredPosition
        };
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
        if (_player.CurState() == CharacterState.Dead)
            return;

        if (!BattleUnitManager.Instance.IsUltimateUnlocked(_player.Data.PlayerKey))
            return;

        if (_isCooldown)
            return;

        // Player에게 궁극기 사용 요청
        _player.RequestUseUltimate(_isCooldown);
    }

    private void OnUltimateUsed(float cooldown)
    {
        //궁극기 발동
        _remainCooldown = cooldown;
        _cooldownRoutine = StartCoroutine(StartCooldown(cooldown));
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
                _cooldownImage.fillAmount = _remainCooldown / cooldownTime;
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
            UnbindPlayer(_player);
        }
        
        _player = player;

        BindPlayerInternal(_player);
    }

    private void BindPlayerInternal(Player player)
    {
        BattleUnitManager.Instance.OnUltUnlocked += RefreshUltUI;
        BattleUnitManager.Instance.OnUltUnlocked += OnUltimateUnlocked;
        BattleUnitManager.Instance.OnUpdateStatusBar += RefreshStatusBar;

        player.OnUltUsed += OnUltimateUsed;
        player.OnDie += ApplyUltUIOnPlayerDead;
        player.OnRevive += ApplyUltUIOnPlayerRevive;
        player.PlayerHealth.OnHpChanged += UpdateHpBar;
        player.PlayerMana.OnMpChanged += UpdateMpBar;

        // 초기값 갱신
        RefreshStatusBar(player.Data.PlayerKey);
        // 처음 한번 아이콘 로드
        SetUltIcon();
    }

    private void UnbindPlayer(Player player)
    {
        BattleUnitManager.Instance.OnUltUnlocked -= RefreshUltUI;
        BattleUnitManager.Instance.OnUltUnlocked -= OnUltimateUnlocked;
        BattleUnitManager.Instance.OnUpdateStatusBar -= RefreshStatusBar;

        player.OnUltUsed -= OnUltimateUsed;
        player.OnDie -= ApplyUltUIOnPlayerDead;
        player.OnRevive -= ApplyUltUIOnPlayerRevive;
        player.PlayerHealth.OnHpChanged -= UpdateHpBar;
        player.PlayerMana.OnMpChanged -= UpdateMpBar;
    }

    private void OnUltimateUnlocked(int key)
    {
        if (_player.Data.PlayerKey != key)
            return;

        SetUIForUltUnlocked();
    }

    private void ApplyUltUIOnPlayerDead()
    {
        if(_cooldownRoutine != null) 
        {
            StopCoroutine(_cooldownRoutine);
            _cooldownRoutine = null;
        }

        _ultButton.interactable = false;
        _cooldownImage.fillAmount = 1.0f;

        _cooltimeText.gameObject.SetActive(false);

        _hpBar.gameObject.SetActive(false);
        _mpBar.gameObject.SetActive(false);
    }

    private void ApplyUltUIOnPlayerRevive()
    {
        _isPaused = false;
        _isCooldown = false;
        _remainCooldown = 0.0f;

        _cooldownImage.fillAmount = 0.0f;
        _ultButton.interactable = true;

        _hpBar.gameObject.SetActive(true);
        _mpBar.gameObject.SetActive(true);
    }

    private void SetUltIcon()
    {
        if (_isUltIconSet || _player == null)
            return;

        Sprite ultSprite = Resources.Load<Sprite>(_player.Data.UltIcon);
        if (ultSprite != null)
        {
            _ultButton.image.sprite = ultSprite;
        }
        else
        {
            Debug.LogWarning("궁극기 UI 아이콘이 없음");
        }

        _isUltIconSet = true;

        SetUIForUltLocked();
    }

    private void RefreshStatusBar(int key)
    {
        if (_player.Data.PlayerKey != key) return;

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

        btn.anchoredPosition = _basePos.button + new Vector2(0, buttonOffsetY);
        hp.anchoredPosition = _basePos.hpBar + new Vector2(0, statusOffsetY);
        mp.anchoredPosition = _basePos.mpBar + new Vector2(0, statusOffsetY);
    }

    private void RefreshUltUI(int key)
    {
        if (_player == null) return;
        if (_player.Data.PlayerKey != key) return;

        if (_player.Data.CanUseUlt)
        {
            _cooldownImage.fillAmount = 0.0f;
        }
    }
}