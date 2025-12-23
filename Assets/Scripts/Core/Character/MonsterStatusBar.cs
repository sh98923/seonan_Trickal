using UnityEngine;

public class MonsterStatusBar : CharacterStatusBar
{
    protected enum MonsterStatus
    {
        Hp = 1
    }

    private CharacterHp _monsterHp;

    private void Awake()
    {
        _monsterHp = GetComponent<CharacterHp>();

        Transform[] transforms = GetComponentsInChildren<Transform>();

        _hpBar = transforms[(int)MonsterStatus.Hp];

        base.Awake();
    }

    private void OnEnable()
    {
        _monsterHp.OnHpChanged += UpdateHpBar;
    }

    private void OnDisable()
    {
        _monsterHp.OnHpChanged -= UpdateHpBar;
    }
}