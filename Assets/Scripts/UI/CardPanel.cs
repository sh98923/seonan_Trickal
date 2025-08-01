using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    private enum CardUI
    {
        cardBG = 1, DeployButton, 
        CharacterImage, CharacterName, 
        CostImage, CostText
    }

    private InGameUIPanel _inGameUIPanel;
    private Transform[] _cardChildren;
    private BattleSetupPanel _parentPanel;
    private PlayerSpawn _spawnParent;

    private PlayerData _playerData;

    private bool _wasRerollActive = false;

    private void Awake()
    {
        _cardChildren = GetComponentsInChildren<Transform>();
    }

    private void Start()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _spawnParent = _inGameUIPanel.SpawnParent.GetComponent<PlayerSpawn>();
        _parentPanel = _inGameUIPanel.GetUIElement<BattleSetupPanel>(InGameUI.BattleSetUpPanel);

        InitUI();
        RegisterButtonEvents();
    }

    private void InitUI()
    {
        _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCostImageVisibility();
    }

    private void UpdateCostImageVisibility()
    {
        bool isRerollActive = BattleStateManager.Instance.IsReroll;

        if (_wasRerollActive != isRerollActive)
        {
            _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(isRerollActive);
            _wasRerollActive = isRerollActive;
        }
    }

    private void RegisterButtonEvents()
    {
        _cardChildren[(int)CardUI.DeployButton]
            .GetComponent<Button>().onClick.AddListener(OnClickMyDeckCard);
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
        CharacterData characterData = CharacterManager.Instance.GetCharacterData(_playerData.CharacterKey);

        if (_spawnParent.IsDataDeployed(_playerData))
        {
            Debug.LogWarning(characterData.EngName + "이(가) 이미 배치되어 있습니다.");
            return;
        }

        Vector3 spawnPos = _spawnParent.SetPlayerPos(_playerData);

        // 위치 못 찾으면 생성을 안 함
        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return;
        }

        GameObject playerPrefab = Resources.Load<GameObject>(characterData.PrefabPath);
        GameObject player = Instantiate(playerPrefab, _spawnParent.transform);
        player.name = characterData.EngName;
        player.layer = LayerMask.NameToLayer(characterData.Layer);
        spawnPos.z = 0.0f;
        player.transform.position = spawnPos;

        BattleUnitManager.Instance.RegisterUnit(_playerData, player);
    }

    private void UpgradePlayer()
    {
        BattleUnitManager.Instance.UpgradeUnit(_playerData.Key);

        int curLevel = BattleUnitManager.Instance.CurLevel;
        _parentPanel.CardCostUpdate(_playerData.Key, curLevel);
    }

    public void SetPlayerUnit(PlayerData playerData)
    {
        _playerData = playerData;

        SetCardInfo();
    }

    private void SetCardInfo()
    {
        CharacterData characterData = CharacterManager.Instance.GetCharacterData(_playerData.CharacterKey);

        // 캐릭터 텍스쳐
        Image characterImage = _cardChildren[(int)CardUI.CharacterImage].GetComponent<Image>();
        characterImage.sprite = Resources.Load<Sprite>(_playerData.SpritePath);

        // 캐릭터 이름
        TextMeshProUGUI characterName = _cardChildren[(int)CardUI.CharacterName].GetComponent<TextMeshProUGUI>();
        characterName.text = characterData.KrName;

        // 캐릭터 Cost BG
        Sprite costImage = Resources.Load<Sprite>("Sprites/CardInfo/CostBG");

        Image cardCostBG = _cardChildren[(int)CardUI.CostImage].GetComponent<Image>();
        cardCostBG.sprite = costImage;

        // 캐릭터 Cost Text
        TextMeshProUGUI cardUpgradeCost = _cardChildren[(int)CardUI.CostText].GetComponent<TextMeshProUGUI>();
        string curUpgradeCost = _playerData.CardUpgradeCost.ToString();
        cardUpgradeCost.text = curUpgradeCost;
    }
}