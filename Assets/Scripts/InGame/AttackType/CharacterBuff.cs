using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffEffectType
{
    SoloBuff, PartialBuff, AllBuff
}

public enum BuffValueType
{
    AttackPower, Heal    
}

public class CharacterBuff : CharacterAction
{
    private List<Player> _activePlayers = new List<Player>();
    //private Player _caster;

    private BuffValueType _valueType;
    private BuffEffectType _effectType;

    private float _effectValue = 0.0f;
    private float _timer = 0.0f;
    private string _clipName = "";

    public override void SetBuffInfo(ActionSlot type, string clipName, float effectValue, float time)
    {
        _type = type;
        _timer = time;
        _clipName = clipName;
        _effectValue = effectValue;
    }

    private void ApplyBuffToAll()
    {
        foreach (Player player in _activePlayers)
        {
            ApplyBuff(player);
        }
    }

    private void ApplyBuff(Player player)
    {
        // 기존 값 저장
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
            case BuffValueType.Heal:
                //originalValue = player.Hp;
                //player.Hp = Mathf.Min(player.MaxHp, player.Hp + _effectValue);
                break;
        }

        // 이펙트 재생
        Transform characterTransform = player.transform;
        EffectManager.Instance.PlayEffect(characterTransform, _clipName, _timer);

        // 타이머 종료 후 원래 값 복구 (힐 제외)
        if (_valueType != BuffValueType.Heal)
        {
            StartCoroutine(RemoveBuffAfterTime(player, originalValue, _timer));
        }
    }

    private IEnumerator RemoveBuffAfterTime(Player player, float originalValue, float duration)
    {
        yield return new WaitForSeconds(duration);

        player.AtkBuff = originalValue;
    }

    private void Buff()
    {
        _activePlayers = InGameManager.Instance.GetActiveUnits();

        string effectName = _character.Data.ActionImpact[(int)_type];

        _effectType = GetEffectType<BuffEffectType>(effectName, out _isValid);

        if (!_isValid) return;

        switch(_effectType)
        {
            case BuffEffectType.SoloBuff:
                break;
            case BuffEffectType.PartialBuff:
                break;
            case BuffEffectType.AllBuff:
                ApplyBuffToAll();
                break;
        }
    }

    public override void Excute()
    {
        Buff();
    }
}