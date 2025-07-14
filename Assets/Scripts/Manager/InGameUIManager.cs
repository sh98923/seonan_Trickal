using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    private enum InGameUIElement
    { 
        DeckContent = 6, SelectCardPanel = 7, 
        CardPanelLeft = 8, CardPanelCenter = 13, CardPanelRight = 18,
        RerollImage = 23
    }

    private Transform[] _inGameUIs;
    private Transform _spawnParent;
    public Transform SpawnParent
    {
        get { return _spawnParent; }
    }

    private void Awake()
    {
        _spawnParent = GameObject.Find("SpawnPlayer").transform;
        _inGameUIs = GetComponentsInChildren<Transform>();

        SetInit();
    }

    private void SetInit()
    {
        DeckCard deckCard = _inGameUIs[(int)InGameUIElement.DeckContent].GetComponent<DeckCard>();
        deckCard.SpawnParent = _spawnParent;

        _inGameUIs[(int)InGameUIElement.SelectCardPanel].gameObject.SetActive(false);
        _inGameUIs[(int)InGameUIElement.RerollImage].gameObject.SetActive(false);
    }
}