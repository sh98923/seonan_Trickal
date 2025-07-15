using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDeployBtn : MonoBehaviour
{
    private enum CardUIElement
    {
        Image, NameText
    }

    private Transform[] _buttonChildren;
    private Transform _spawnParent;
    private PlayerData _playerData;

    private void Awake()
    {
        SetButtonElement();
    }

    private void Start()
    {
        _spawnParent = InGameUIPanelManager.Instance.SpawnParent;
        GetComponent<Button>().onClick.AddListener(OnClickMyDeckCard);
    }

    private void OnClickMyDeckCard()
    {
        if(BattleStateManager.Instance.IsBattleStart)
        {
            UpgradePlayer();
        }
        else
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPos = _spawnParent.GetComponent<PlayerSpawn>().SetPlayerPos(_playerData);

        // 위치 못 찾으면 생성을 안 함
        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return;
        }

        GameObject playerPrefab = Resources.Load<GameObject>(_playerData.PrefabPath);
        GameObject player = Instantiate(playerPrefab, _spawnParent);
        player.name = _playerData.EngName;
        player.layer = LayerMask.NameToLayer(_playerData.Layer);
        spawnPos.z = 0.0f;
        player.transform.position = spawnPos;

        BattleUnitManager.Instance.RegisterUnit(_playerData, player);
    }

    private void UpgradePlayer()
    {
        BattleUnitManager.Instance.UpgradeUnit(_playerData.Key);  
    }

    private void SetButtonElement()
    {
        _buttonChildren = new Transform[transform.childCount];

        for (int i = 0; i < _buttonChildren.Length; i++)
        {
            _buttonChildren[i] = transform.GetChild(i);
        }
    }

    public void SetPlayerUnit(int key)
    {
        _playerData = CharacterManager.Instance.GetPlayerData(key);

        Image image = _buttonChildren[(int)CardUIElement.Image].GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>(_playerData.SpritePath);

        TextMeshProUGUI tmpText = _buttonChildren[(int)CardUIElement.NameText].GetComponent<TextMeshProUGUI>();
        tmpText.text = _playerData.Name;
    }
}