using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardPanel : MonoBehaviour
{
    private InGameUIPanel _inGameUIPanel;
    private CardDeployBtn _deployBtn;
    private GameObject _cardRerollPanel;
    private GameObject _deckPanel;

    private int _startPlayerKey;

    private void Awake()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
    }

    private void Start()
    {
        SetStageName();
        SetDeckCard();
        SetStartBtn();
    }

    private void SetStageName()
    {
        TextMeshProUGUI stageName = _inGameUIPanel.GetUIElement<TextMeshProUGUI>(InGameUIElement.StageName);
        int key = StageManager.Instance.StageStartKey;
        StageData data = StageManager.Instance.GetStageData(key);

        stageName.text = data.Stage.ToString() + "-" + data.Wave.ToString();
    }

    private void SetDeckCard()
    {
        Transform deckConent = _inGameUIPanel.GetUIElement<Transform>(InGameUIElement.DeckContent);

        _startPlayerKey = CharacterManager.Instance.PlayerStartKey;

        GameObject UIprefab = Resources.Load<GameObject>("Prefabs/UI/CardPanel");

        for (int i = _startPlayerKey; i < CharacterManager.Instance.GetPlayerDataCount(); i++)
        {
            GameObject obj = Instantiate(UIprefab, deckConent);
            obj.name += ("_" + i);

            Transform buttonTransform = obj.transform.Find("Button");

            _deployBtn = buttonTransform.GetComponent<CardDeployBtn>();
            _deployBtn.SetPlayerUnit(_startPlayerKey + i);
        }
    }

    private void SetStartBtn()
    {
        Button startBtn = _inGameUIPanel.GetUIElement<Button>(InGameUIElement.InGameStartBtn);
        startBtn.onClick.AddListener(OnClickStart);
    }

    public void OnClickStart()
    {
        /*GameObject deckPanel = _inGameUIPanel.GetUIElement(InGameUIElement.DeckPanel);
        GameObject cardRerollPanel = _inGameUIPanel.GetUIElement(InGameUIElement.CardRerollPanel);*/
        
        _deckPanel = _inGameUIPanel.GetUIElement(InGameUIElement.DeckPanel);
        _cardRerollPanel = _inGameUIPanel.GetUIElement(InGameUIElement.CardRerollPanel);

        _deckPanel.SetActive(false);
        _cardRerollPanel.SetActive(true);

        _inGameUIPanel.IsDeckMode = true;
        BattleStateManager.Instance.IsBattleStart = true;
    }
}