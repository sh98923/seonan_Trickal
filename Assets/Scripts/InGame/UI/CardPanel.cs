using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    protected enum CardUI
    {
        DeployButton = 1, 
        CharacterImage, CharacterName, 
        CostImage, CostText
    }

    protected Transform[] _cardChildren;

    protected PlayerData _playerData;
    protected Color _color = Color.white;

    protected string _curScene;
    private bool _wasRerollActive = false;

    protected void Awake()
    {
        _curScene = SceneManager.GetActiveScene().name;
        _cardChildren = GetComponentsInChildren<Transform>();
    }

    protected void Start()
    {
        InitUI();
        RegisterButtonEvents();
    }

    private void Update()
    {
        // 여기 부분 이벤트로 리팩토링 해보자
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
        HandleClick();
    }

    protected virtual void InitUI()
    {
        // 자식 스크립트에서 처리
    }

    protected virtual void HandleClick()
    {
        // 자식 스크립트에서 처리
    }

    public void SetPlayerUnit(PlayerData playerData)
    {
        _playerData = playerData;

        SetCardInfo();
    }

    protected virtual void SetCardInfo()
    {
        SetCharacterImage();
        SetCharacterName();
        SetCardCost();
    }

    private void SetCharacterImage()
    {
        Image characterImage = _cardChildren[(int)CardUI.CharacterImage].GetComponent<Image>();
        characterImage.sprite = Resources.Load<Sprite>(_playerData.SpritePath);
        characterImage.color = _color;
    }

    private void SetCharacterName()
    {
        CharacterData characterData = CharacterManager.Instance.GetCharacterData(_playerData.CharacterKey);
        TextMeshProUGUI characterName = _cardChildren[(int)CardUI.CharacterName].GetComponent<TextMeshProUGUI>();
        characterName.text = characterData.KrName;
        characterName.color = _color;
    }

    private void SetCardCost()
    {
        Sprite costImage = Resources.Load<Sprite>("Sprites/CardInfo/CostBG");

        Image cardCostBG = _cardChildren[(int)CardUI.CostImage].GetComponent<Image>();
        cardCostBG.sprite = costImage;

        TextMeshProUGUI cardUpgradeCost = _cardChildren[(int)CardUI.CostText].GetComponent<TextMeshProUGUI>();
        cardUpgradeCost.text = _playerData.CardUpgradeCost.ToString();
    }
}