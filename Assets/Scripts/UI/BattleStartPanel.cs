using UnityEngine;
using UnityEngine.UI;

public class BattleStartPanel : MonoBehaviour
{
    private Button _startBtn;
    private Animator _animator;

    private void Awake()
    {
        _startBtn = GetComponentInChildren<Button>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetTrigger("RerollStart");
        _startBtn.onClick.AddListener(OnClickBattleStartAnim);
    }

    public void OnClickBattleStartAnim()
    {
        _animator.SetTrigger("BattleStart");
        BattleStateManager.Instance.SetState(BattleState.Battle);
    }
}
