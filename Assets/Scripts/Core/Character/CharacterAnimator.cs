using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetIdle(bool isIdle)
    {
        _animator.SetBool("IdleState", isIdle);
    }

    public void SetTrigger(string animName)
    {
        _animator.SetTrigger(animName);
    }

    public void SetSpeed(float speed)
    {
        _animator.speed = speed;
    }

    public bool HasEnteredThenExited(string stateName, ref bool entered, int layer = 0)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(layer);

        // 현재 상태가 stateName인 경우 진입 플래그 ON 아직 종료되지 않음
        if (stateInfo.IsName(stateName))
        {
            entered = true;
            return false;
        }

        // 현재 stateName이 아니면서 이전에 진입한 적이 있었다면 "진입 후 종료" 완료
        if (entered)
        { 
            return true;
        }

        return false;
    }

    public bool IsAnimationFinished(int layer = 0)
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(layer);
        return info.normalizedTime >= 1.0f;
    }
}