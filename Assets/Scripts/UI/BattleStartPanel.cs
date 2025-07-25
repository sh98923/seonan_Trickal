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
        _startBtn.onClick.AddListener(OnClickBattleStartAnim);
    }

    private void OnBattleStartBtnInActive()
    {
        gameObject.SetActive(false);
    }

    public void OnClickBattleStartAnim()
    {
        BattleStateManager.Instance.SetState(BattleState.Battle);
        _animator.SetTrigger("BattleStart");
    }
}
