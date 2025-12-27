using UnityEngine;

public class MonsterStatusBar : CharacterStatusBar
{
    protected enum MonsterStatus
    {
        Hp = 1
    }

    private void Awake()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        _hpBar = transforms[(int)MonsterStatus.Hp];

       // base.Awake();
    }
}