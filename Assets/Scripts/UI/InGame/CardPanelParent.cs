using UnityEngine;

public class CardPanelParent : MonoBehaviour
{
    private Animator _cardPanelRootAnim;
    private InGameCardPanel[] _cardPanels; 

    private void Awake()
    {
        _cardPanelRootAnim = GetComponent<Animator>();
        _cardPanels = GetComponentsInChildren<InGameCardPanel>();
    }

    public void SetCardLockState(bool isLock)
    {
        for(int i = 0; i < _cardPanels.Length; i++)
        {
            _cardPanels[i].CardPadLock(isLock);
        }
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