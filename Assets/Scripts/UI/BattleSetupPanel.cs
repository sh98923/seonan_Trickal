using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleSetupPanel : MonoBehaviour
{
    private enum CardRerollUIElement
    {
        LeftCardPanel, CenterCardPanel, RightCardPanel, RerollBtn, BattleBtn,
        CardCount = 3
    }

   // private List<PlayerData> _rerollCandidates = new List<PlayerData>();
    private InGameUIPanel _inGameUIPanel;
    private Animator _setupAnimator;
    private Transform[] _rerollChildren;
    private Button[] _cardBtns;

    private readonly int _maxLevel = 5;
    private int _cardCount = (int)CardRerollUIElement.CardCount;

    private void Awake()
    {
        _setupAnimator = GetComponent<Animator>();
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        
        InitRerollUI();
    }

    /*private void Start()
    {
        _rerollCandidates = _inGameUIPanel.SetRerollCandidates();

        for (int i = 0; i < _cardCount; i++)
        {
            int index = i;
            _cardBtns[i].onClick.AddListener(() => OnClickCard(index));
        }

        _cardBtns[(int)CardRerollUIElement.RerollBtn].onClick.AddListener(OnClickReroll);
        _cardBtns[(int)CardRerollUIElement.BattleBtn].onClick.AddListener(OnClickBattle);
    }*/

    private void InitRerollUI()
    {
        int childCount = transform.childCount;
        _rerollChildren = new Transform[childCount];
        _cardBtns = new Button[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            _rerollChildren[i] = child;
            _cardBtns[i] = child.GetComponentInChildren<Button>();
        }
    }

    private void OnClickReroll()
    {
        _setupAnimator.Play("CardRoll", 0, 0f);
    }

    private void OnClickBattle()
    {
        BattleStateManager.Instance.SetState(BattleState.Battle);
    }

    private void OnClickCard(int index)
    {
        _setupAnimator.SetTrigger("SelectedCard" + index);
    }

    /*private void OnCardDataRamdomSet()
    {
        for (int i = 0; i < _cardCount; i++)
        {
            int randomIndex = Random.Range(0, _rerollCandidates.Count);
            _rerollChildren[i].GetComponent<CardPanel>()
                .SetPlayerUnit(_rerollCandidates[randomIndex]);
        }
    }

    public void CardCostUpdate(int key, int curLevel)
    {
        for(int i = 0; i < _rerollCandidates.Count; i++)
        {
            if (_rerollCandidates[i].Key == key && curLevel < _maxLevel)
            {
                PlayerData data = _rerollCandidates[i];
                data.CardCost += 5;
                _rerollCandidates[i] = data;
            }
        }
    }*/
}