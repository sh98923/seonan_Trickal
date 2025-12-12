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

    private Button[] _cardBtns;
    private Animator _setupAnimator;
    private TextMeshProUGUI _coinText;
    private Transform[] _rerollChildren;
    private InGameUIPanel _inGameUIPanel;
    private List<PlayerData> _rerollCandidates = new List<PlayerData>();

    private readonly int _rerollCost = 5;
    private int CurCoin => InGameManager.Instance.InGameCoin; 
    private int _cardCount = (int)CardRerollUI.CardCount;

    private void Awake() 
    { 
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

        _cardBtns[(int)CardRerollUI.RerollBtn].onClick.AddListener(OnClickCardReroll);
        _cardBtns[(int)CardRerollUI.BattleBtn].onClick.AddListener(OnClickEnteringBattle);
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
                _coinText.text = CurCoin.ToString();
            }
        }

        _rerollChildren = childList.ToArray();
        _cardBtns = buttonList.ToArray();
    }

    private void OnClickCardReroll()
    {
        if (!InGameManager.Instance.TrySpendCoin(_rerollCost))
        {
            return; 
        }

        UpdateCoinUI();
        _setupAnimator.Play("CardRoll", 0, 0f);
    }

    private void OnClickEnteringBattle()
    {
        InGameManager.Instance.IsGameStart = true;
        BattleStateManager.Instance.SetState(BattleState.EnteringBattle);
    }

    private void OnClickCard(int index)
    {
        if (!InGameManager.Instance.CanPayCoin) return;

        _setupAnimator.SetTrigger("SelectedCard" + index);
    }

    private void OnCardDataRamdomSet()
    {
        for (int i = 0; i < _cardCount; i++)
        {
            int randomIndex = Random.Range(0, _rerollCandidates.Count);
            _rerollChildren[i].GetComponent<InGameCardPanel>()
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
                data.UpgradeCost += 5;
                _rerollCandidates[i] = data;
            }
        }
    }

    public void UpdateCoinUI()
    {
        _coinText.text = CurCoin.ToString();
    }
}