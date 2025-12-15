using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

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
    private Coroutine _coinSpendCoroutine;
    private List<PlayerData> _rerollCandidates = new List<PlayerData>();

    private readonly int _rerollCost = 5;
    private int _curCoin = InGameManager.Instance.InGameCoin;
    private int _cardCount = (int)CardRerollUI.CardCount;

    private bool _isRerollAnimating = false;
    private bool _isSelectedCardAnimating = false;
    public bool IsSelectedCardAnimating
    {
        get { return _isSelectedCardAnimating; }
    }

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
                _coinText.text = _curCoin.ToString();
            }
        }

        _rerollChildren = childList.ToArray();
        _cardBtns = buttonList.ToArray();
    }

    private void OnClickCardReroll()
    {
        if (_isRerollAnimating)
            return;

        if (!InGameManager.Instance.TrySpendCoin(_rerollCost))
            return;

        UpdateCoinUI(_rerollCost);

        _isRerollAnimating = true;
        _setupAnimator.Play("CardRoll", 0, 0.0f);
    }

    private void OnClickEnteringBattle()
    {
        InGameManager.Instance.IsGameStart = true;
        BattleStateManager.Instance.SetState(BattleState.EnteringBattle);
    }

    private void OnClickCard(int index)
    {
        if (_isSelectedCardAnimating)
            return;

        if (!InGameManager.Instance.CanPayCoin) 
            return;

        _isSelectedCardAnimating = true;
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

    private IEnumerator PlayCoinSpend(int targetCoin)
    {
        while (_curCoin > targetCoin)
        {
            _curCoin--;
            yield return new WaitForSeconds(0.05f);
            _coinText.text = _curCoin.ToString();
        }

        _coinSpendCoroutine = null;
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

    public void UpdateCoinUI(int cost)
    {
        int targetCoin = InGameManager.Instance.SpendCoin(cost);

        if (_curCoin == targetCoin)
            return;

        if (_coinSpendCoroutine != null)
        {
            StopCoroutine(_coinSpendCoroutine);
        }

        _coinSpendCoroutine = StartCoroutine(PlayCoinSpend(targetCoin));
    }

    public void OnRerollAnimationEnd()
    {
        _isRerollAnimating = false;
    }

    public void OnSelectedCardAnimationEnd()
    {
        _isSelectedCardAnimating = false;
    }
}