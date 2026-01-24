using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPadLock : MonoBehaviour
{
    private enum LockState 
    { 
        Unlocked,
        Locked
    }

    private Dictionary<LockState, Sprite> _lockSprites = new Dictionary<LockState, Sprite>();

    private Image _padLock;

    private void Awake()
    {
        _padLock = GetComponent<Image>();

        _lockSprites[LockState.Unlocked] = Resources.Load<Sprite>("Sprites/UI/Card/CardPadUnlocked");
        _lockSprites[LockState.Locked] = Resources.Load<Sprite>("Sprites/UI/Card/CardPadLocked");
        
        _padLock.sprite = _lockSprites[LockState.Unlocked];
    }

    public void SetLockState(bool isLocked)
    {
        LockState state = isLocked ? LockState.Locked : LockState.Unlocked;
        _padLock.sprite = _lockSprites[state];
    }
}
