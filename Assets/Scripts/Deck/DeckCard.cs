using UnityEngine;

public class DeckCard : MonoBehaviour
{
    private CardDeployBtn _deployBtn;

    private int _startPlayerKey;

    private void Start()
    {
        SetDeckCard();
    }

    private void SetDeckCard()
    {
        _startPlayerKey = CharacterManager.Instance.PlayerStartKey;

        GameObject UIprefab = Resources.Load<GameObject>("Prefabs/UI/CardPanel");

        for (int i = _startPlayerKey; i < CharacterManager.Instance.GetPlayerDataCount(); i++)
        {
            GameObject obj = Instantiate(UIprefab, transform);
            obj.name += ("_" + i);

            Transform buttonTransform = obj.transform.Find("Button");

            _deployBtn = buttonTransform.GetComponent<CardDeployBtn>();
            _deployBtn.SetPlayerUnit(_startPlayerKey + i);
        }
    }
}