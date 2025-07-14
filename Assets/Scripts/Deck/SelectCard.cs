using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectCard : MonoBehaviour
{
    private Button[] _cardBtns;

    private List<PlayerData> _rerollCandidates = new List<PlayerData>();
    
    private void Awake()
    {
        _cardBtns = GetComponentsInChildren<Button>();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnBattleStartChanged += HandleBattleStartChanged;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnBattleStartChanged -= HandleBattleStartChanged;
    }

    private void HandleBattleStartChanged(bool isBattleStart)
    {
        if (isBattleStart)
        {
            _rerollCandidates = InGameUIPanelManager.Instance.SetRerollCandidates();
            SetRandomCard();
        }
    }

    private void Update()
    {
        bool isCardReroll = InGameUIPanelManager.Instance.IsClickedReroll;

        if (isCardReroll)
        {
            SetRandomCard();
        }
    }

    private void SetRandomCard()
    {
        for (int i = 0; i < _cardBtns.Length; i++)
        {
            int randomIndex = Random.Range(0, _rerollCandidates.Count);
            _cardBtns[i].GetComponent<CardDeployBtn>().SetPlayerUnit(_rerollCandidates[randomIndex].Key);
        }
    }
}