using UnityEngine;

public class StageCardPanel : CardPanel
{
    private StagePlayerSpawn _spawnParent;

    private void Awake()
    {
        base.Awake();

        if(_curScene != "StageSelectScene")
        {
            this.enabled = false;
        }

        _spawnParent = GameObject.Find("SpawnPlayer").GetComponent<StagePlayerSpawn>();
    }

    protected override void InitUI()
    {
        _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(false);
    }

    protected override void HandleClick()
    {
        if (IsAlreadyDeployed(_playerData))
        {
            Debug.LogWarning($"{_playerData.EngName}이(가) 이미 배치되어 있습니다.");
            return;
        }

        if (!TryGetSpawnPosition(_playerData, out Vector3 spawnPos))
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return;
        }

        SpawnPlayerAtPosition(_playerData, spawnPos);

        GameManager.Instance.SetDeckUnit(_playerData, spawnPos);
    }

    private bool IsAlreadyDeployed(PlayerData playerData)
    {
        return _spawnParent.IsDataDeployed(playerData);
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

    private void SpawnPlayerAtPosition(PlayerData playerData, Vector3 spawnPos)
    {
        GameObject playerPrefab = Resources.Load<GameObject>(playerData.PrefabPath);
        GameObject player = Instantiate(playerPrefab, _spawnParent.transform);
        player.name = playerData.EngName;
        player.layer = LayerMask.NameToLayer(playerData.Layer);
        player.transform.position = spawnPos;
    }
}