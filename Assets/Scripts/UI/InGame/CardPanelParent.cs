using UnityEngine;

public class CardPanelParent : MonoBehaviour
{
    private Animator _cardPanelRootAnim;

    private void Awake()
    {
        _cardPanelRootAnim = GetComponent<Animator>();
    }

    public void HideCardPanel()
    {
        _cardPanelRootAnim.SetTrigger("EnteringBattle");
    }

    public void ShowCardPanel()
    {
        _cardPanelRootAnim.SetTrigger("Reroll");
    }
}