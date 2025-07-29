using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    private enum CardUIElement
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
        _parentPanel = _inGameUIPanel.GetUIElement<BattleSetupPanel>(InGameUIElement.BattleSetUpPanel);

        InitUI();
        RegisterButtonEvents();
    }

    private void InitUI()
    {
        _cardChildren[(int)CardUIElement.CostImage].gameObject.SetActive(false);
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
            _cardChildren[(int)CardUIElement.CostImage].gameObject.SetActive(isRerollActive);
            _wasRerollActive = isRerollActive;
        }
    }

    private void RegisterButtonEvents()
    {
        _cardChildren[(int)CardUIElement.DeployButton]
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
        // 캐릭터 텍스쳐
        Image characterImage = _cardChildren[(int)CardUIElement.CharacterImage].GetComponent<Image>();
        characterImage.sprite = Resources.Load<Sprite>(_playerData.SpritePath);

        // 캐릭터 이름
        TextMeshProUGUI characterName = _cardChildren[(int)CardUIElement.CharacterName].GetComponent<TextMeshProUGUI>();
        characterName.text = _playerData.Name;

        // 캐릭터 Cost BG
        Sprite costImage = Resources.Load<Sprite>("Sprites/CardInfo/CostBG");

        Image cardCostBG = _cardChildren[(int)CardUIElement.CostImage].GetComponent<Image>();
        cardCostBG.sprite = costImage;

        // 캐릭터 Cost Text
        TextMeshProUGUI cardCost = _cardChildren[(int)CardUIElement.CostText].GetComponent<TextMeshProUGUI>();
        string curCost = _playerData.CardCost.ToString();
        cardCost.text = curCost;
    }
}