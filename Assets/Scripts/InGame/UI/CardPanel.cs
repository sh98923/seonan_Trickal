using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    private enum CardUI
    {
        cardBG = 1, DeployButton, 
        CharacterImage, CharacterName, 
        CostImage, CostText
    }

    private Transform[] _cardChildren;
    private BattleSetupPanel _parentPanel;
    private PlayerSpawn _spawnParent;

    private PlayerData _playerData;

    private string _sceneName;
    private bool _wasRerollActive = false;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _cardChildren = GetComponentsInChildren<Transform>();
        _spawnParent = GameObject.Find("SpawnPlayer").GetComponent<PlayerSpawn>();
    }

    private void Start()
    {
        InitUI();
        RegisterButtonEvents();
    }

    private void InitUI()
    {
        if (_sceneName == "StageSelectScene")
        {
            _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(false);
        }

        if(_sceneName == "InGameScene")
        {
            _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(true); 
            _parentPanel = GetComponentInParent<BattleSetupPanel>();
        }
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
        if (_sceneName == "StageSelectScene")
        {
            HandleStageSelectClick();
        }
        else if (_sceneName == "InGameScene")
        {
            HandleInGameClick();
        }
    }

    private void HandleStageSelectClick()
    {
        CharacterData characterData = CharacterManager.Instance.GetCharacterData(_playerData.CharacterKey);

        if (_spawnParent.IsDataDeployed(_playerData))
        {
            Debug.LogWarning(characterData.EngName + "이(가) 이미 배치되어 있습니다.");
            return;
        }

        Vector3 spawnPos = _spawnParent.SetPlayerPos(_playerData);
        spawnPos.z = 0.0f;

        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return;
        }

        SpawnPlayerAtPosition(characterData, spawnPos);

        CharacterFullData data = new CharacterFullData(characterData, _playerData);
        GameManager.Instance.SetDeckUnit(data, spawnPos);
    }

    private void SpawnPlayerAtPosition(CharacterData characterData, Vector3 spawnPos)
    {
        GameObject playerPrefab = Resources.Load<GameObject>(characterData.PrefabPath);
        GameObject player = Instantiate(playerPrefab, _spawnParent.transform);
        player.name = characterData.EngName;
        player.layer = LayerMask.NameToLayer(characterData.Layer);
        player.transform.position = spawnPos;
    }

    private void HandleInGameClick()
    {
        string characterName = CharacterManager.Instance.GetCharacterData(_playerData.CharacterKey).EngName;
        Transform existingChild = _spawnParent.transform.Find(characterName);

        if (existingChild != null)
        {
            if (!existingChild.gameObject.activeSelf)
            {
                existingChild.gameObject.SetActive(true);
            }
            else
            {
                UpgradePlayer();
            }
        }
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