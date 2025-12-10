using UnityEngine;

public class InGameCardPanel : CardPanel
{
    private BattleSetupPanel _parentPanel;

    private void Awake()
    {
        base.Awake();

        if(_curScene != "InGameScene")
        {
            this.enabled = false;
        }
    }

    protected override void InitUI()
    {
        base.InitUI();
        _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(true);
        _parentPanel = GetComponentInParent<BattleSetupPanel>();
    }

    protected override void SetCardInfo()
    {
        UpdateDeployButtonState();

        base.SetCardInfo();
    }

    protected override void HandleClick()
    {
        int cost = _playerData.UpgradeCost;
        string characterName = _playerData.EngName;

        if (!InGameManager.Instance.TrySpendCoin(cost))
        {
            Debug.LogWarning("코인이 부족합니다.");
            return;
        }

        if (InGamePlayerSpawn.Instance.IsPlayerInactive(characterName))
        {
            // 아직 비활성화 상태 → 유닛 배치
            InGamePlayerSpawn.Instance.SetActivePlayer(characterName);
            UpdateCardCostUI();
        }
        else
        {
            // 이미 배치됨 → 업그레이드
            UpgradePlayer();
        }

        _parentPanel.UpdateCoinUI();
    }

    private void UpgradePlayer()
    {
        BattleUnitManager.Instance.UpgradeUnit(_playerData.Key);

        UpdateCardCostUI();
    }

    private void UpdateCardCostUI()
    {
        int curLevel = BattleUnitManager.Instance.CurLevel;
        _parentPanel.CardCostUpdate(_playerData.Key, curLevel);
    }

    private void UpdateDeployButtonState()
    {
        //Button deployBtn = _cardChildren[(int)CardUI.CardButton].GetComponent<Button>();
        bool isCardLocked = InGameManager.Instance.InGameCoin < _playerData.UpgradeCost;

        if(isCardLocked)
        {
            _color = Color.gray;
        }
        else
        {
            _color = Color.white;
        }
    }
}
