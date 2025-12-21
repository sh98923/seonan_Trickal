using UnityEngine;

public class StageCardPanel : CardPanel
{
    private DeckCardPanel _deckCardPanel;
    private StagePlayerSpawn _spawnParent;
    private OutLineLight _outline;

    private void Awake()
    {
        base.Awake();

        GameManager.Instance.EnableInScenes(this, SceneName.StageSelectScene);
    }

    private void Start()
    {
        base.Start();

        _deckCardPanel = GetComponentInParent<DeckCardPanel>();
        _spawnParent = GameObject.Find("SpawnPlayer").GetComponent<StagePlayerSpawn>();
        _outline = GetComponent<OutLineLight>();
        _outline.InitImage(_cardChildren[(int)CardUI.OutLineColorImage]);
    }

    protected override void InitUI()
    {
        base.InitUI();
        _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(false);
    }

    protected override void HandleClick()
    {
        if (CanDeploy(out Vector3 spawnPos))
        {
            _outline.OutLineActive(true);

            _spawnParent.SpawnPlayerAtPosition(_playerData, spawnPos);

            GameManager.Instance.AddUnit(_playerData, spawnPos);
        }
        else
        {
            _outline.OutLineActive(false);
        }

        _deckCardPanel.SetSlotCount();
    }

    private bool CanDeploy(out Vector3 spawnPos)
    {
        spawnPos = Vector3.zero;

        // 다시 누르면 제거하기
        if (RemoveDeployed(_playerData))
        {
            GameManager.Instance.RemoveUnit(_playerData);
            Debug.LogWarning($"{_playerData.EngName}를 제거함.");
            return false;
        }

        if (GameManager.Instance.IsDeckFull())
        {
            Debug.LogWarning("덱이 가득 찼습니다.");
            return false;
        }

        if (!TryGetSpawnPosition(_playerData, out spawnPos))
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return false;
        }

        return true;
    }

    private bool RemoveDeployed(PlayerData playerData)
    {
        return _spawnParent.CheckAndRemoveDeployed(playerData);
    }

    private bool TryGetSpawnPosition(PlayerData playerData, out Vector3 spawnPos)
    {
        spawnPos = _spawnParent.SetPlayerPos(playerData);
        if (spawnPos != Vector3.zero)
        {
            spawnPos.z = 0.0f;
            return true;
        }
        return false;
    }
}