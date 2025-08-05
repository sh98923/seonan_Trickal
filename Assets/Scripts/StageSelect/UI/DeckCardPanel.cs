using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckCardPanel : MonoBehaviour
{
    private enum DeckUI
    {
        StageName = 1, StartBtn = 2, DeckContent = 5
    }

    private Transform[] _deckChildren;

    private CardPanel _deployBtn;

    private int _startPlayerKey;

    private void Awake()
    {
        _deckChildren = GetComponentsInChildren<Transform>();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        SetStageName();
        SetDeckCard();
        SetStartBtn();
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

            _deployBtn = obj.GetComponent<CardPanel>();
            _deployBtn.SetPlayerUnit(playerData);
        }
    }

    private void SetStartBtn()
    {
        Button startBtn = _deckChildren[(int)DeckUI.StartBtn].GetComponent<Button>();
        startBtn.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        SceneManager.LoadScene("InGameScene");

        /*gameObject.SetActive(false);
        BattleStateManager.Instance.SetState(BattleState.Reroll);
        BattleStateManager.Instance.RerollEvent();*/
    }
}