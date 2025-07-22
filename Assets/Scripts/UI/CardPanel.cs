using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    private enum CardUIElement
    {
        cardBG, DeployButton, CostImage
    }

    private enum BtnUIElement
    {
        CharacterImage, CharacterName
    }

    private InGameUIPanel _inGameUIPanel;
    private Transform[] _cardPanelChildren;
    private PlayerSpawn _spawnParent;
    private PlayerData _playerData;

    private void Awake()
    {
        InitUIElements();
    }

    private void Start()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _spawnParent = _inGameUIPanel.SpawnParent.GetComponent<PlayerSpawn>();

        _cardPanelChildren[(int)CardUIElement.DeployButton].
            GetComponent<Button>().onClick.AddListener(OnClickMyDeckCard);

        _cardPanelChildren[(int)CardUIElement.CostImage].gameObject.SetActive(false);  
    }

    private void Update()
    {
        UpdateCostImageVisibility();
    }

    private void UpdateCostImageVisibility()
    {
        bool shouldShow = BattleStateManager.Instance.IsReroll;
        _cardPanelChildren[(int)CardUIElement.CostImage].gameObject.SetActive(shouldShow);
    }

    private void OnClickMyDeckCard()
    {
        if(BattleStateManager.Instance.IsReroll)
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
        if (_spawnParent.IsDataDeployed(_playerData))
        {
            Debug.LogWarning(_playerData.Name + "이(가) 이미 배치되어 있습니다.");
            return;
        }


        Vector3 spawnPos = _spawnParent.SetPlayerPos(_playerData);

        // 위치 못 찾으면 생성을 안 함
        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return;
        }

        GameObject playerPrefab = Resources.Load<GameObject>(_playerData.PrefabPath);
        GameObject player = Instantiate(playerPrefab, _spawnParent.transform);
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

    private void InitUIElements()
    {
        _cardPanelChildren = new Transform[transform.childCount];

        for (int i = 0; i < _cardPanelChildren.Length; i++)
        {
            _cardPanelChildren[i] = transform.GetChild(i);
        }
    }

    public void SetPlayerUnit(int key)
    {
        _playerData = CharacterManager.Instance.GetPlayerData(key);

        Transform deployBtn = _cardPanelChildren[(int)CardUIElement.DeployButton];

        Image characterImage = 
            deployBtn.GetChild((int)BtnUIElement.CharacterImage).GetComponent<Image>();
        characterImage.sprite = Resources.Load<Sprite>(_playerData.SpritePath);

        TextMeshProUGUI characterName = 
            deployBtn.GetChild((int)BtnUIElement.CharacterName).GetComponent<TextMeshProUGUI>();
        characterName.text = _playerData.Name;
    }
}