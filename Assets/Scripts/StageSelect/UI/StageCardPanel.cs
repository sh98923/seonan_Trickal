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
        if (IsAlreadyDeployed(_playerData, out CharacterData characterData))
        {
            Debug.LogWarning($"{characterData.EngName}이(가) 이미 배치되어 있습니다.");
            return;
        }

        if (!TryGetSpawnPosition(_playerData, out Vector3 spawnPos))
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return;
        }

        SpawnPlayerAtPosition(characterData, spawnPos);

        CharacterFullData data = new CharacterFullData(characterData, _playerData);
        GameManager.Instance.SetDeckUnit(data, spawnPos);
    }

    private bool IsAlreadyDeployed(PlayerData playerData, out CharacterData characterData)
    {
        characterData = CharacterManager.Instance.GetCharacterData(playerData.CharacterKey);
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

    private void SpawnPlayerAtPosition(CharacterData characterData, Vector3 spawnPos)
    {
        GameObject playerPrefab = Resources.Load<GameObject>(characterData.PrefabPath);
        GameObject player = Instantiate(playerPrefab, _spawnParent.transform);
        player.name = characterData.EngName;
        player.layer = LayerMask.NameToLayer(characterData.Layer);
        player.transform.position = spawnPos;
    }
}