using UnityEngine;

public class CardReroll : MonoBehaviour
{
    [SerializeField] private Animator _selectCardAnimator;

    public void OnClickReroll()
    {
        _selectCardAnimator.Play("CardRoll", 0, 0f); // ��� cardroll �ִϸ��̼� ���
    }
}
