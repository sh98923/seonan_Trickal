using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    private BattleSetUpUI _ingameUIRoot;

    private void Awake()
    {
        _ingameUIRoot = GetComponentInParent<BattleSetUpUI>();   
    }

    public void OnRerollAnimationEndEvent()
    {
        _ingameUIRoot.OnRerollAnimationEnd();
    }

    public void OnSelectedCardAnimationEndEvent()
    {
        _ingameUIRoot.OnSelectedCardAnimationEnd();
    }

    public void OnInGameEnter()
    {
        BattleStateManager.Instance.OnInGameEnter();
    }
}
