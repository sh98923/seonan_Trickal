using System;
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
        LeftCard = 1,
        CenterCard = 9,
        RightCard = 17,
    }

    public static event Action OnCardAction;

    private BattleSetUpUI _battleSetUpUI;
    private Animator _cardSelectAnimator;

    private Transform[] _rerollChildren;
    private Button[] _rerollStateButtons;

    private List<PlayerData> _rerollCandidates = new List<PlayerData>();

    private readonly CardUI[] _cardPositions = { CardUI.LeftCard, CardUI.CenterCard, CardUI.RightCard };

    private const int _rerollCost = 5;
    private const int _unitCardCount = 3;

    private void Awake()
    {
        _cardSelectAnimator = GetComponent<Animator>();
        _rerollChildren = GetComponentsInChildren<Transform>();
        _battleSetUpUI = GetComponentInParent<BattleSetUpUI>();
        _rerollStateButtons = GetComponentsInChildren<Button>();
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

        if (!InGameManager.Instance.CanPayCoin)
            return;

        OnCardAction?.Invoke();

        _battleSetUpUI.OnSelectedCardAnimationStart();
        _cardSelectAnimator.SetTrigger("SelectedCard" + index);
    }

    private void OnClickCardReroll()
    {
        if (_battleSetUpUI.IsRerollAnimating)
            return;

        if (!InGameManager.Instance.TrySpendCoin(_rerollCost))
            return;

        _battleSetUpUI.UpdateCoinUI(_rerollCost);

        _battleSetUpUI.OnRerollAnimationStart();
        _cardSelectAnimator.Play("CardRoll", 0, 0.0f);
    }

    private void PlayCardRoll()
    {
        if (_battleSetUpUI.IsRerollAnimating)
            return;

        _battleSetUpUI.OnRerollAnimationStart();
        _cardSelectAnimator.Play("CardRoll", 0, 0.0f);
    }

    private void OnClickEnteringBattle()
    {
        _cardSelectAnimator.SetTrigger("EnteringBattle");
        InGameManager.Instance.IsGameStart = true;
        BattleStateManager.Instance.SetState(BattleState.EnteringBattle);
    }

    private void PlayCardHide()
    {
        _cardSelectAnimator.SetTrigger("EnteringBattle");
    }

    private void OnCardDataRamdomSet()
    {
        foreach (CardUI cardPos in _cardPositions)
        {
            int randomIndex = UnityEngine.Random.Range(0, _rerollCandidates.Count);
            _rerollChildren[(int)cardPos].GetComponent<InGameCardPanel>()
                .SetPlayerUnit(_rerollCandidates[randomIndex]);
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