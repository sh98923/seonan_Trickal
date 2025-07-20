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
    private Animator _CardRerollPanelAnimator;
    private Button[] _cardBtns;

    private int _cardCount = (int)CardRerollPanelButton.CardCount;

    private void Awake()
    {
        _cardBtns = GetComponentsInChildren<Button>();
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _CardRerollPanelAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _rerollCandidates = _inGameUIPanel.SetRerollCandidates();

        for (int i = 0; i < _cardCount; i++)
        {
            int index = i;
            _cardBtns[i].onClick.AddListener(() => OnClickCard(index));
        }

        _cardBtns[(int)CardRerollPanelButton.Reroll].onClick.AddListener(OnClickReroll);
    }

    private void OnClickReroll()
    {
        _CardRerollPanelAnimator.Play("CardRoll", 0, 0f);
    }

    private void OnClickCard(int index)
    {
        _CardRerollPanelAnimator.SetTrigger("SelectedCard" + index);
    }

    private void OnCardDataRamdomSet()
    {
        for (int i = 0; i < _cardCount; i++)
        {
            int randomIndex = Random.Range(0, _rerollCandidates.Count);
            _cardBtns[i].GetComponent<CardDeployBtn>().SetPlayerUnit(_rerollCandidates[randomIndex].Key);
        }
    }
}