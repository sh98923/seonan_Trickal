using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardPanel : MonoBehaviour
{
    private enum DeckElement
    {
        StageName = 1, StartBtn = 2, DeckContent = 5
    }

    private Transform[] _deckChildren;

    private CardPanel _deployBtn;

    private int _startPlayerKey;

    private void Awake()
    {
        _deckChildren = GetComponentsInChildren<Transform>();
    }

    private void Start()
    {
        SetStageName();
        SetDeckCard();
        SetStartBtn();
    }

    private void SetStageName()
    {
        GameObject stageObj = _deckChildren[(int)DeckElement.StageName].gameObject;
        TextMeshProUGUI stageName = stageObj.GetComponent<TextMeshProUGUI>();
        int key = StageManager.Instance.StageStartKey;
        StageData data = StageManager.Instance.GetStageData(key);

        stageName.text = data.Stage.ToString() + "-" + data.Wave.ToString();
    }

    private void SetDeckCard()
    {
        Transform deckConent = _deckChildren[(int)DeckElement.DeckContent];

        _startPlayerKey = CharacterManager.Instance.PlayerStartKey;

        GameObject UIprefab = Resources.Load<GameObject>("Prefabs/UI/CardPanel");

        /*for (int i = _startPlayerKey; i < CharacterManager.Instance.PlayerCount; i++)
        {
            GameObject obj = Instantiate(UIprefab, deckConent);
            obj.name += ("_" + i);

            PlayerData playerData = CharacterManager.Instance.GetCharacterData(_startPlayerKey + i);

            _deployBtn = obj.GetComponent<CardPanel>();
            _deployBtn.SetPlayerUnit(playerData);
        }*/
    }

    private void SetStartBtn()
    {
        Button startBtn = _deckChildren[(int)DeckElement.StartBtn].GetComponent<Button>();
        startBtn.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        gameObject.SetActive(false);
        BattleStateManager.Instance.SetState(BattleState.Reroll);
        BattleStateManager.Instance.RerollEvent();
    }
}