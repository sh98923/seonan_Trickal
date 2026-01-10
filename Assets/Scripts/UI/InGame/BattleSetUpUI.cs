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

    private const float _coinChangeDuration = 0.5f;

    private const int _increaseCost = 5;
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
        float accumulated = 0.0f;
        int startCoin = _curCoin;
        int coinDiff = targetCoin - startCoin;

        if (coinDiff == 0)
            yield break;

        // 초당 증가량: 차이에 따라 속도 자동 조절
        float incrementPerSecond = Mathf.Abs(coinDiff) / _coinChangeDuration;

        while (_curCoin != targetCoin)
        {
            // 1프레임에서 증가할 값 계산
            float frameDelta = incrementPerSecond * Time.deltaTime;
            accumulated += frameDelta;

            // 누적 float >= 1이면 실제 int로 증가/감소
            if (accumulated >= 1.0f)
            {
                int change = Mathf.FloorToInt(accumulated);
                if (coinDiff < 0)
                {
                    change *= -1;
                }

                // startCoin, targetCoin 중 min, max를 구하고 clamp로 _curCoin값을 구함
                _curCoin = Mathf.Clamp(_curCoin + change,
                                       Mathf.Min(startCoin, targetCoin),
                                       Mathf.Max(startCoin, targetCoin));
                _coinText.text = _curCoin.ToString();

                accumulated -= Mathf.Abs(change);
            }

            yield return null;
        }

        // 마지막으로 정확하게 target 맞춤
        _curCoin = targetCoin;
        _coinText.text = _curCoin.ToString();

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
                data.UpgradeCost += _increaseCost;
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