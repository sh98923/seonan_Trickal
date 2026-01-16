using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RerollUIController : MonoBehaviour
{
    private enum RerollUI
    {
        cardRerollBtn = 3, 
        StartBattleBtn
    }

    private enum CardUI
    {
        LeftCard = 2,
        CenterCard = 10,
        RightCard = 18,
    }

    private CardPanelParent _cardPanelRoot;
    private BattleSetUpUI _battleSetUpUI;
    private Animator _cardSelectAnimator;

    private Transform[] _rerollChildren;
    private Button[] _rerollStateButtons;
    private InGameCardPanel[] _cardPanels;

    private List<PlayerData> _rerollCandidates = new List<PlayerData>();

    private readonly CardUI[] _cardPositions = { CardUI.LeftCard, CardUI.CenterCard, CardUI.RightCard };

    private const int _rerollCost = 5;
    private const int _unitCardCount = 3;

    private void Awake()
    {
        _cardSelectAnimator = GetComponent<Animator>();
        _battleSetUpUI = GetComponentInParent<BattleSetUpUI>();
        _rerollChildren = GetComponentsInChildren<Transform>();
        _rerollStateButtons = GetComponentsInChildren<Button>();
        _cardPanelRoot = GetComponentInChildren<CardPanelParent>();

        _cardPanels = new InGameCardPanel[_cardPositions.Length];

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

        _rerollStateButtons[(int)RerollUI.cardRerollBtn].onClick.AddListener(OnClickCardReroll);
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
        if (_battleSetUpUI.IsSelectedCardAnimating)
            return;

        bool canPlayAnimation = _cardPanels[index].TryCardClick();
        if (!canPlayAnimation)
            return;

        _battleSetUpUI.OnSelectedCardAnimationStart();
        _cardSelectAnimator.SetTrigger("SelectedCard" + index);
    }

    private void OnClickCardReroll()
    {
        if (_battleSetUpUI.IsRerollAnimating)
            return;

        if (!InGameManager.Instance.TrySpendCoin(_rerollCost))
            return;

        _battleSetUpUI.OnRerollAnimationStart();
        _cardSelectAnimator.Play("CardRoll", 0, 0.0f);
    }

    private void PlayCardRoll()
    {
        if (_battleSetUpUI.IsRerollAnimating)
            return;

        _battleSetUpUI.OnRerollAnimationStart();
        _cardPanelRoot.ShowCardPanel();
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
        for (int i = 0; i < _cardPanels.Length; i++)
        {
            int randomIndex = Random.Range(0, _rerollCandidates.Count);
            _cardPanels[i].SetPlayerUnit(_rerollCandidates[randomIndex]);
        }
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