using UnityEngine;
using UnityEngine.UI;

public class UltUIPanel : MonoBehaviour
{
    private Animator _ultUIPanelAnim;

    private void Awake()
    {
        _ultUIPanelAnim = GetComponentInParent<Animator>();
    }

    private void Start()
    {
        BattleStateManager.Instance.OnBattle += PlayShowUltUI;
        BattleStateManager.Instance.OnEnteringReroll += PlayHideUltUI;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnBattle -= PlayShowUltUI;
        BattleStateManager.Instance.OnEnteringReroll -= PlayHideUltUI;
    }

    private void PlayShowUltUI()
    {
        _ultUIPanelAnim.SetTrigger("ShowUltPanel");
    }

    private void PlayHideUltUI()
    {
        _ultUIPanelAnim.SetTrigger("HideUltPanel");
    }
}
