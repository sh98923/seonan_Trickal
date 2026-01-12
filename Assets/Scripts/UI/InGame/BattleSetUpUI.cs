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

        // �ʴ� ������: ���̿� ���� �ӵ� �ڵ� ����
        float incrementPerSecond = Mathf.Abs(coinDiff) / _coinChangeDuration;

        while (_curCoin != targetCoin)
        {
            // 1�����ӿ��� ������ �� ���
            float frameDelta = incrementPerSecond * Time.deltaTime;
            accumulated += frameDelta;

            // ���� float >= 1�̸� ���� int�� ����/����
            if (accumulated >= 1.0f)
            {
                int change = Mathf.FloorToInt(accumulated);
                if (coinDiff < 0)
                {
                    change *= -1;
                }

                // startCoin, targetCoin �� min, max�� ���ϰ� clamp�� _curCoin���� ����
                _curCoin = Mathf.Clamp(_curCoin + change,
                                       Mathf.Min(startCoin, targetCoin),
                                       Mathf.Max(startCoin, targetCoin));
                _coinText.text = _curCoin.ToString();

                accumulated -= Mathf.Abs(change);
            }

            yield return null;
        }

        // ���������� ��Ȯ�ϰ� target ����
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