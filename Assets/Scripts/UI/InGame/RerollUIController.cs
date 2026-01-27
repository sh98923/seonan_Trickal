using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class RerollUIController : MonoBehaviour
{
    enum PadLockImage 
    {
        PadLock = 29
    }

    private enum RerollUI
    {
        CardLockBtn = 3,
        CardRerollBtn = 4, 
        StartBattleBtn = 5
    }

    private enum CardUI
    {
        LeftCard = 2,
        CenterCard = 11,
        RightCard = 20
    }

    private CardPadLock _padLock;
    private Animator _cardSelectAnimator;
    private BattleSetUpUI _battleSetUpUI;
    private CardPanelParent _cardPanelRoot;

    private Transform[] _rerollChildren;
    private Button[] _rerollStateButtons;
    private InGameCardPanel[] _cardPanels;

    private List<PlayerData> _rerollCandidates = new List<PlayerData>();

    private readonly CardUI[] _cardPositions = { CardUI.LeftCard, CardUI.CenterCard, CardUI.RightCard };

    private const float _unlockDelay = 0.6f;
    private const int _rerollCost = 5;
    private const int _unitCardCount = 3;

    private bool _isCardLocked = false;

    private void Awake()
    {
        _cardSelectAnimator = GetComponent<Animator>();
        _battleSetUpUI = GetComponentInParent<BattleSetUpUI>();
        _rerollChildren = GetComponentsInChildren<Transform>();
        _rerollStateButtons = GetComponentsInChildren<Button>();
        _cardPanelRoot = GetComponentInChildren<CardPanelParent>();

        _cardPanels = new InGameCardPanel[_cardPositions.Length];

        _padLock = _rerollChildren[(int)PadLockImage.PadLock].GetComponent<CardPadLock>();

        for (int i = 0; i < _cardPositions.Length; i++)
        {
            _cardPanels[i] = _rerollChildren[(int)_cardPositions[i]].GetComponent<InGameCardPanel>();
        }
    }

    private void Start()
    {
        _rerollCandidates = _battleSetUpUI.GetRerollCandidatesFromUI();

        for (int i = 0; i < _unitCardCount; i++)
        {
            int index = i;
            _rerollStateButtons[i].onClick.AddListener(() => OnClickCard(index));
        }

        _rerollStateButtons[(int)RerollUI.CardLockBtn].onClick.AddListener(OnClickCardLock);
        _rerollStateButtons[(int)RerollUI.CardRerollBtn].onClick.AddListener(OnClickCardReroll);
        _rerollStateButtons[(int)RerollUI.StartBattleBtn].onClick.AddListener(OnClickEnteringBattle);

        BattleStateManager.Instance.OnReroll += PlayCardRoll;
        BattleStateManager.Instance.OnEnteringBattle += PlayCardHide;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= PlayCardRoll;
        BattleStateManager.Instance.OnEnteringBattle -= PlayCardHide;
    }

    private void OnClickCard(int index)
    {
        // 카드 클릭이 안되는건 이 조건문이 true인 상태에서 애니메이션이 끝 프레임까지 못가서
        // 이벤트 호출을 못했고 그래서 _isSelectedCardAnimating이 false가 되지 못한 이유임
        if (_battleSetUpUI.IsSelectedCardAnimating)
            return;

        bool canPlayAnimation = _cardPanels[index].TryCardClick();
        if (!canPlayAnimation)
            return;

        _isCardLocked = false;
        SetLockSprite(_isCardLocked);
        _battleSetUpUI.OnSelectedCardAnimationStart();
        _cardSelectAnimator.SetTrigger("SelectedCard" + index);
    }

    private void OnClickCardReroll()
    {
        if (_battleSetUpUI.IsRerollAnimating)
            return;

        if (!InGameManager.Instance.TrySpendCoin(_rerollCost))
            return;

        _isCardLocked = false;
        SetLockSprite(_isCardLocked);
        _battleSetUpUI.OnRerollAnimationStart();
        _cardSelectAnimator.Play("CardRoll", 0, 0.0f);
    }

    private void OnClickCardLock()
    {
        _isCardLocked = !_isCardLocked;
        SetLockSprite(_isCardLocked);
    }

    private void SetLockSprite(bool locked)
    {
        _padLock.SetLockState(locked);
        _cardPanelRoot.SetCardLockState(locked);
    }

    private void PlayCardRoll()
    {
        if (_battleSetUpUI.IsRerollAnimating)
            return;

        _battleSetUpUI.OnRerollAnimationStart();
        _cardPanelRoot.ShowCardPanel();
        ResetCardLock();    
        _cardSelectAnimator.Play("CardRoll", 0, 0.0f);
    }

    private void OnClickEnteringBattle()
    {
        _cardPanelRoot.HideCardPanel();
        InGameManager.Instance.IsGameStart = true;
        BattleStateManager.Instance.SetState(BattleState.EnteringBattle);
    }

    private void PlayCardHide()
    {
        _cardPanelRoot.HideCardPanel();
    }

    private void OnCardDataRamdomSet()
    {
        if (_isCardLocked)
            return;

        for (int i = 0; i < _cardPanels.Length; i++)
        {
            int randomIndex = Random.Range(0, _rerollCandidates.Count);
            _cardPanels[i].SetPlayerUnit(_rerollCandidates[randomIndex]);
        }
    }

    private IEnumerator ResetCardLock()
    {
        yield return new WaitForSeconds(_unlockDelay);

        _isCardLocked = false;
        SetLockSprite(_isCardLocked);
    }

    public void OnRerollAnimationEndEvent()
    {
        _battleSetUpUI.OnRerollAnimationEnd();
    }

    public void OnSelectedCardAnimationEndEvent()
    {
        _battleSetUpUI.OnSelectedCardAnimationEnd();
    }
}