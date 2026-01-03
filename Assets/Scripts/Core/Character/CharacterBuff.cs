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
    private Dictionary<Player, Coroutine> _runningBuffs = new Dictionary<Player, Coroutine>();

    private BuffValueType _valueType;
    private BuffEffectType _effectType;

    private float _effectValue = 0.0f;

    private void OnEnable()
    {
        BattleStateManager.Instance.OnEnteringReroll += ResetAllBuffs;
    }

    private void OnDisable()
    {
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnEnteringReroll -= ResetAllBuffs;
        }
    }

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

        float originalValue = 0.0f;

        switch (_valueType)
        {
            case BuffValueType.AttackPower:
                originalValue = player.AtkBuff;
                player.AtkBuff = _effectValue;
                break;

            case BuffValueType.DamageReduction:
                originalValue = player.DamageReduction;
                player.ApplyDamageReduction(_effectValue);
                break;

            case BuffValueType.Heal:
                // player.PlayerHealth.CurHp = Mathf.Min(player.PlayerHealth.MaxHp, player.PlayerHealth.CurHp + _effectValue);
                return; // Heal은 즉시 적용 후 종료
        }

        // 이미 적용 중인 버프가 있다면 Coroutine 정리
        if (_runningBuffs.ContainsKey(player))
        {
            Coroutine routine = _runningBuffs[player];
            if (routine != null)
                StopCoroutine(routine);
        }

        Coroutine coroutine = StartCoroutine(RemoveBuffAfterTime(player, _valueType, originalValue, _timer));
        _runningBuffs[player] = coroutine;

        PlayEffect(player.transform);

        /*switch (_valueType)
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

        PlayEffect(player.transform);*/

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
        float timer = 0.0f;

        while (timer < duration)
        {
            if (BattleStateManager.Instance.CurrentState == BattleState.EnteringReroll)
                break;

            timer += Time.deltaTime;
            yield return null;
        }

        // 원래 값으로 복구
        switch (type)
        {
            case BuffValueType.AttackPower:
                player.AtkBuff = originalValue;
                break;
            case BuffValueType.DamageReduction:
                player.ApplyDamageReduction(originalValue);
                break;
        }

        if (_runningBuffs.ContainsKey(player))
        {
            _runningBuffs.Remove(player);
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
        _activePlayers = InGamePlayerSpawn.Instance.GetActivePlayers();

        foreach (Player player in _activePlayers)
        {
            ApplyBuff(player);
        }
    }

    private void ResetAllBuffs()
    {
        foreach (KeyValuePair<Player, Coroutine> runningBuff in _runningBuffs)
        {
            Player player = runningBuff.Key;
            Coroutine coroutine = runningBuff.Value;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            // 즉시 원래 값 복구
            switch (_valueType)
            {
                case BuffValueType.AttackPower:
                    player.AtkBuff = 1.0f;
                    break;
                case BuffValueType.DamageReduction:
                    player.ApplyDamageReduction(1.0f);
                    break;
            }
        }

        _runningBuffs.Clear();
    }

    public override void Excute()
    {
        Buff();
    }
}