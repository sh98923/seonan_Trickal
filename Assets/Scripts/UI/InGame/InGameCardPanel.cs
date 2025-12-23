using UnityEngine;

public class InGameCardPanel : CardPanel
{
    private BattleSetupPanel _parentPanel;

    private void Awake()
    {
        base.Awake();

        GameManager.Instance.EnableScriptInScenes(this, SceneName.InGameScene);
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
        if (IsBlocked())
            return;

        if (!CanPayCost())
            return;

        ExecuteCardAction();
        UpdateAfterAction();
    }

    private bool IsBlocked()
    {
        return _parentPanel.IsSelectedCardAnimating;
    }

    private bool CanPayCost()
    {
        int cost = _playerData.UpgradeCost;

        if (!InGameManager.Instance.TrySpendCoin(cost))
        {
            Debug.LogWarning("코인이 부족합니다.");
            return false;
        }

        return true;
    }

    private void ExecuteCardAction()
    {
        string characterName = _playerData.EngName;

        if (InGamePlayerSpawn.Instance.IsPlayerInactive(characterName))
        {
            DeployPlayer(characterName);
        }
        else
        {
            UpgradePlayer();
        }
    }

    private void DeployPlayer(string characterName)
    {
        InGamePlayerSpawn.Instance.SetActivePlayer(characterName);
    }

    private void UpgradePlayer()
    {
        BattleUnitManager.Instance.UpgradeUnit(_playerData.Key);
    }

    private void UpdateAfterAction()
    {
        UpdateCardCostUI();
        _parentPanel.UpdateCoinUI(_playerData.UpgradeCost);
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

        if (isCardLocked)
        {
            _color = Color.gray;
        }
        else
        {
            _color = Color.white;
        }
    }
}
