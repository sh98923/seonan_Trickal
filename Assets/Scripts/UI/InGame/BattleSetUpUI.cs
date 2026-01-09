using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class BattleSetUpUI : MonoBehaviour
{
    private TextMeshProUGUI _coinText;
    private Coroutine _coinCoroutine;
    private InGameUIPanel _inGameUIPanel;

    private List<PlayerData> _rerollCandidates = new List<PlayerData>();

    private const int _unitCost = 5;
    private int _curCoin = 0;

    private bool _isRerollAnimating = false;
    public bool IsRerollAnimating
    {
        get { return _isRerollAnimating; }
    }
    private bool _isSelectedCardAnimating = false;
    public bool IsSelectedCardAnimating
    {
        get { return _isSelectedCardAnimating; }
    }

    private void Awake()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
    }

    private void Start()
    {
        InitRerollUI();
        InGameManager.OnCoinChanged += OnCoinChanged;
    }

    private void OnDisable()
    {
        InGameManager.OnCoinChanged -= OnCoinChanged;
    }

    private void InitRerollUI()
    {
        _rerollCandidates = _inGameUIPanel.GetRerollCandidates();

        GameObject inGameCoin = _inGameUIPanel.GetUIElement(InGameUI.CoinText);
        TextMeshProUGUI coinText = inGameCoin.GetComponent<TextMeshProUGUI>();

        _curCoin = InGameManager.Instance.InGameCoin;
        _coinText = coinText;
        _coinText.text = _curCoin.ToString();
    }

    private void OnCoinChanged(int targetCoin)
    {
        if (_coinCoroutine != null)
            StopCoroutine(_coinCoroutine);

        _coinCoroutine = StartCoroutine(PlayCoinChange(targetCoin));
    }

    private IEnumerator PlayCoinChange(int targetCoin)
    {
        while (_curCoin != targetCoin)
        {
            _curCoin += (_curCoin < targetCoin) ? 1 : -1;
            _coinText.text = _curCoin.ToString();
            yield return new WaitForSeconds(0.05f);
        }

        _coinCoroutine = null;
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
                data.UpgradeCost += _unitCost;
                _rerollCandidates[i] = data;
            }
        }
    }

    public List<PlayerData> GetRerollCandidatesFromUI()
    {
        return _rerollCandidates;
    }

    public void OnRerollAnimationStart()
    {
        _isRerollAnimating = true;
    }

    public void OnRerollAnimationEnd()
    {
        _isRerollAnimating = false;
    }

    public void OnSelectedCardAnimationStart()
    {
        _isSelectedCardAnimating = true;
    }

    public void OnSelectedCardAnimationEnd()
    {
        _isSelectedCardAnimating = false;
    }
}