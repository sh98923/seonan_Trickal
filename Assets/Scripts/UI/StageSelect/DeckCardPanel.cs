using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckCardPanel : MonoBehaviour
{
    private enum DeckUI
    {
        StageName = 1, 
        StartBtn = 2, 
        DeckContent = 5,
        DeckSlotCountView = 8
    }

    private Transform[] _deckChildren;
    private StageCardPanel _deployBtn;
    private TextMeshProUGUI _slotCountText;

    private int _startPlayerKey;

    private void Awake()
    {
        _deckChildren = GetComponentsInChildren<Transform>();
        _slotCountText = _deckChildren[(int)DeckUI.DeckSlotCountView].GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetStageName();
        SetDeckCard();
        SetStartBtn();
        UpdateSlotCount();
    }

    private void SetStageName()
    {
        GameObject stageObj = _deckChildren[(int)DeckUI.StageName].gameObject;
        TextMeshProUGUI stageName = stageObj.GetComponent<TextMeshProUGUI>();
        int key = StageManager.Instance.StageStartKey;
        StageData data = StageManager.Instance.GetStageData(key);
        int stage = StageBtnPosManager.Instance.GetStageKey(data.Stage);

        stageName.text = "Stage " + stage.ToString();
    }

    private void SetDeckCard()
    {
        Transform deckConent = _deckChildren[(int)DeckUI.DeckContent];

        _startPlayerKey = PlayerManager.Instance.PlayerStartKey;

        GameObject UIprefab = Resources.Load<GameObject>("Prefabs/UI/CardPanel");

        for (int i = 0; i < PlayerManager.Instance.PlayerCount; i++)
        {
            GameObject obj = Instantiate(UIprefab, deckConent);
            obj.name += ("_" + i);

            PlayerData playerData = PlayerManager.Instance.GetPlayerData(_startPlayerKey + i);

            _deployBtn = obj.GetComponent<StageCardPanel>();
            _deployBtn.SetPlayerUnit(playerData);
        }
    }

    private void SetStartBtn()
    {
        Button startBtn = _deckChildren[(int)DeckUI.StartBtn].GetComponent<Button>();
        startBtn.onClick.AddListener(OnClickStart);
    }

    private void UpdateSlotCount()
    {
        _slotCountText.text = $"{GameManager.Instance.CurDeckUnitCount()} / {GameManager.Instance.MaxDeckUnitCount}";
    }

    public void SetSlotCount()
    {
        UpdateSlotCount();
    }

    private void OnClickStart()
    {
        if (GameManager.Instance.CanStartGame())
        {
            SceneManager.LoadScene("InGameScene");
        }
        else
        {
            print("용사들이 모두 편성되지 않았습니다.");
        }
    }
}