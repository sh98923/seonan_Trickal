using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using JetBrains.Annotations;

public class CardRerollPanel : MonoBehaviour
{
    private enum CardRerollPanelButton
    {
        LeftCard, CenterCard, RightCard, Reroll,
        CardCount = 3
    }

    private List<PlayerData> _rerollCandidates = new List<PlayerData>();
    private InGameUIPanel _inGameUIPanel;
    private Animator _cardRerollAnimator;
    private Button[] _cardBtns;

    private void Awake()
    {
        _cardBtns = GetComponentsInChildren<Button>();
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _cardRerollAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnBattleStartChanged += HandleBattleStartChanged;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnBattleStartChanged -= HandleBattleStartChanged;
    }

    private void Start()
    {
        _cardBtns[(int)CardRerollPanelButton.Reroll].onClick.AddListener(OnClickReroll);
    }

    private void OnClickReroll()
    {
        SetRandomCard();
        _cardRerollAnimator.Play("CardRoll", 0, 0f);
    }

    private void HandleBattleStartChanged(bool isBattleStart)
    {
        if (isBattleStart)
        {
            _rerollCandidates = _inGameUIPanel.SetRerollCandidates();
            SetRandomCard();
        }
    }

    private void SetRandomCard()
    {
        int cardCount = (int)CardRerollPanelButton.CardCount;

        for (int i = 0; i < cardCount; i++)
        {
            int randomIndex = Random.Range(0, _rerollCandidates.Count);
            _cardBtns[i].GetComponent<CardDeployBtn>().SetPlayerUnit(_rerollCandidates[randomIndex].Key);
        }
    }
}