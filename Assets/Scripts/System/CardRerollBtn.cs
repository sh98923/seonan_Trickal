using UnityEngine;
using System.Collections;

public class CardReroll : MonoBehaviour
{
    [SerializeField] private Animator _selectCardAnimator;

    public void OnClickReroll()
    {
        InGameUIPanelManager.Instance.IsClickedReroll = true;
        _selectCardAnimator.Play("CardRoll", 0, 0f); // ��� cardroll �ִϸ��̼� ���
        StartCoroutine(ResetReroll());
    }

    private IEnumerator ResetReroll()
    {
        yield return null;
        InGameUIPanelManager.Instance.IsClickedReroll = false;
    }

}