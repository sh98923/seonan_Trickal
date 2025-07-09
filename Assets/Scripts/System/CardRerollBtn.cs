using UnityEngine;

public class CardReroll : MonoBehaviour
{
    [SerializeField] private Animator _selectCardAnimator;

    public void OnClickReroll()
    {
        _selectCardAnimator.Play("CardRoll", 0, 0f); // 즉시 cardroll 애니메이션 재생
    }
}
