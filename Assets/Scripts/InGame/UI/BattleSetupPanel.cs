using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BattleSetupPanel : MonoBehaviour
{
    private enum CardRerollUI
    {
        CardCount = 3, RerollBtn = 3, BattleBtn = 4
    }

    private List<PlayerData> _rerollCandidates = new List<PlayerData>();
    private InGameUIPanel _inGameUIPanel;
    private Animator _setupAnimator;
    private Transform[] _rerollChildren;
    private Button[] _cardBtns;
    private TextMeshProUGUI _coinText;

    private int _cardCount = (int)CardRerollUI.CardCount;
    private int _curCoin = 30;

    private void Awake()
    {
        _curCoin = InGameManager.Instance.InGameCoin;
        _setupAnimator = GetComponent<Animator>();
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();

        InitRerollUI();
    }

    private void Start()
    {
        _rerollCandidates = _inGameUIPanel.SetRerollCandidates();

        for (int i = 0; i < _cardCount; i++)
        {
            int index = i;
            _cardBtns[i].onClick.AddListener(() => OnClickCard(index));
        }

        _cardBtns[(int)CardRerollUI.RerollBtn].onClick.AddListener(OnClickReroll);
        _cardBtns[(int)CardRerollUI.BattleBtn].onClick.AddListener(OnClickBattle);
    }

    private void InitRerollUI()
    {
        List<Transform> childList = new List<Transform>();
        List<Button> buttonList = new List<Button>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Button btn = child.GetComponentInChildren<Button>();

            childList.Add(child);

            if (btn != null)
            {
                buttonList.Add(btn);
            }
            else
            {
                // 이건 CoinText임 (필요하면 저장)
                _coinText = child.GetComponentInChildren<TextMeshProUGUI>();
                _coinText.text = _curCoin.ToString();
            }
        }

        _rerollChildren = childList.ToArray();
        _cardBtns = buttonList.ToArray();
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

    /*private void UpdateCoinText(int cost)
    {
        int remainingCoin = _curCoin - cost;

        _coinText.text = remainingCoin.ToString();
    }*/

    private void OnCardDataRamdomSet()
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
        for (int i = 0; i < _rerollCandidates.Count; i++)
        {
            int curKey = _rerollCandidates[i].Key;
            int maxLevel = _rerollCandidates[i].MaxLevel;

            if (curKey == key && curLevel < maxLevel)
            {
                PlayerData data = _rerollCandidates[i];
                data.CardUpgradeCost += 5;
                _rerollCandidates[i] = data;
            }
        }
    }
}