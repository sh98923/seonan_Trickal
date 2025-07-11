using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCard : MonoBehaviour
{
    private int _startPlayerKey;

    private void Start()
    {
        _startPlayerKey = CharacterManager.Instance.PlayerStartKey;
        SetDeckCard(_startPlayerKey);
    }

    private void SetDeckCard(int startPlayerKey)
    {
        for(int i = startPlayerKey; i < CharacterManager.Instance.GetPlayerDataCount(); i++)
        {
            GameObject UIprefab = Resources.Load<GameObject>("Prefabs/UI/CardPanel");

            GameObject obj = Instantiate(UIprefab, gameObject.transform);
            obj.name += ("_" + i);

            Transform buttonTransform = obj.transform.Find("Button");
            Transform[] buttonChildren = new Transform[buttonTransform.childCount];

            for (int j = 0; j < buttonTransform.childCount; j++)
            {
                buttonChildren[j] = buttonTransform.GetChild(j);
            }

            PlayerData data = CharacterManager.Instance.GetPlayerData(startPlayerKey + i);

            Image image = buttonChildren[0].GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(data.SpritePath);
            image.SetNativeSize();

            TextMeshProUGUI tmpText = buttonChildren[1].GetComponent<TextMeshProUGUI>();
            tmpText.text = data.Name;
        }
    }
}