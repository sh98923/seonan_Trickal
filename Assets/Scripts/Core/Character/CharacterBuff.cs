using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum BuffEffectType
{
    SoloBuff, 
    PartialBuff, 
    AllBuff
}

public enum BuffValueType
{
    AttackPower, 
    DamageReduction,
    Heal
}

public class CharacterBuff : CharacterAction
{
    private List<Player> _activePlayers = new List<Player>();
    //private Player _caster;

    private BuffValueType _valueType;
    private BuffEffectType _effectType;

    private float _effectValue = 0.0f;

    public override void SetBuffInfo(ActionSlot type, string clipName, float effectValue, float time)
    {
        _type = type;
        _timer = time;
        _clipName = clipName;
        _effectValue = effectValue;
    }

    private void ApplyBuff(Player player)
    {
        string effectName = _character.Data.BuffEffect[(int)_type];
        _valueType = GetEffectType<BuffValueType>(effectName, out _isValid);

        if (!_isValid) return;

        switch (_valueType)
        {
            case BuffValueType.AttackPower:
                float originalAtk = player.AtkBuff;
                player.AtkBuff = _effectValue;
                StartCoroutine(RemoveBuffAfterTime(player, _valueType, originalAtk, _timer));
                break;
            case BuffValueType.DamageReduction:
                float originalDR = player.DamageReduction;
                player.ApplyDamageReduction(_effectValue);
                StartCoroutine(RemoveBuffAfterTime(player, _valueType, originalDR, _timer));
                break;
            case BuffValueType.Heal:
                //player.Hp = Mathf.Min(player.MaxHp, player.Hp + _effectValue);
                break;
        }

        PlayEffect(player.transform);

        /*// 기존 값 저장
        float originalValue = 0.0f;

        string effectName = _character.Data.BuffEffect[(int)_type];
        _valueType = GetEffectType<BuffValueType>(effectName, out _isValid);

        if (!_isValid) return;

        switch (_valueType)
        {
            case BuffValueType.AttackPower:
                originalValue = player.AtkBuff;
                player.AtkBuff = _effectValue;
                break;
            case BuffValueType.DamageReduction:
                player.ApplyDamageReduction(_effectValue);
                break;
            case BuffValueType.Heal:
                //originalValue = player.Hp;
                //player.Hp = Mathf.Min(player.MaxHp, player.Hp + _effectValue);
                break;
        }

        // 이펙트 재생
        PlayEffect(player.transform);

        // 타이머 종료 후 원래 값 복구 (힐 제외)
        if (_valueType != BuffValueType.Heal)
        {
            StartCoroutine(RemoveBuffAfterTime(player, originalValue, _timer));
        }*/
    }

    private IEnumerator RemoveBuffAfterTime(Player player, BuffValueType type, float originalValue, float duration)
    {
        yield return new WaitForSeconds(duration);

        switch (type)
        {
            case BuffValueType.AttackPower:
                player.AtkBuff = originalValue;
                break;
            case BuffValueType.DamageReduction:
                player.ApplyDamageReduction(originalValue);
                break;
        }
    }

    private void Buff()
    {
        string effectName = _character.Data.ActionImpact[(int)_type];

        _effectType = GetEffectType<BuffEffectType>(effectName, out _isValid);

        if (!_isValid) return;

        switch(_effectType)
        {
            case BuffEffectType.SoloBuff:
                SoloBuff();
                break;
            case BuffEffectType.PartialBuff:
                break;
            case BuffEffectType.AllBuff:
                ApplyBuffToAll();
                break;
        }
    }

    private void SoloBuff()
    {
        Player player = GetComponent<Player>();

        ApplyBuff(player);
    }

    private void ApplyBuffToAll()
    {
        _activePlayers = transform.parent.GetComponent<InGamePlayerSpawn>().GetActivePlayers();

        foreach (Player player in _activePlayers)
        {
            ApplyBuff(player);
        }
    }

    public override void Excute()
    {
        Buff();
    }
}