using UnityEngine;
using UnityEngine.UI;

public class InGameCardPanel : CardPanel
{
    private BattleSetupPanel _parentPanel; 
    private InGamePlayerSpawn _spawnParent;

    private void Awake()
    {
        base.Awake();

        if(_curScene != "InGameScene")
        {
            this.enabled = false;
        }

        _spawnParent = GameObject.Find("SpawnPlayer").GetComponent<InGamePlayerSpawn>();
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
        Transform existingChild = _spawnParent.transform.Find(characterName);
        
        if (!InGameManager.Instance.TrySpendCoin(cost))
        {
            Debug.LogWarning("코인이 부족합니다.");
            return;
        }

        if (!existingChild.gameObject.activeSelf)
        {
            // 아직 비활성화 상태 → 유닛 배치
            existingChild.gameObject.SetActive(true);
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
        Button deployBtn = _cardChildren[(int)CardUI.CardButton].GetComponent<Button>();
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
