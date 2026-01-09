using UnityEngine;

public class InGameCardPanel : CardPanel
{
    private BattleSetUpUI _battleSetUpUI;

    private void Awake()
    {
        base.Awake();

        GameManager.Instance.EnableScriptInScenes(this, SceneName.InGameScene);
    }

    protected override void InitUI()
    {
        base.InitUI();
        _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(true);
        _battleSetUpUI = GetComponentInParent<BattleSetUpUI>();
    }

    protected override void SetCardInfo()
    {
        UpdateDeployButtonState();

        base.SetCardInfo();
    }

    // 코스트를 지불 할 수 있는지 또는 애니메이션이 실행 중인지 검사하고
    // 만약 가능하다면 아래의 두 함수를 실행함 ( UI갱신, 캐릭터 배치 등 )
    public bool TryCardClick()
    {
        if (IsBlocked())
            return false;

        if (!TryPayCost())
            return false;

        ExecuteCardAction();
        UpdateAfterAction();
        return true;
    }

    private bool IsBlocked()
    {
        return _battleSetUpUI.IsSelectedCardAnimating;
    }

    public bool TryPayCost()
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
    }

    private void UpdateCardCostUI()
    {
        int curLevel = BattleUnitManager.Instance.GetUnitLevel(_playerData.Key);
        _battleSetUpUI.CardCostUpdate(_playerData.Key, curLevel);
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
