using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private class BuffRuntimeData
    {
        public Coroutine Coroutine;
        public BuffValueType ValueType;
        public float OriginalValue;
    }
    
    private Player _player;
    private List<Player> _activePlayers = new List<Player>();
    private Dictionary<Player, List<BuffRuntimeData>> _runningBuffs = new Dictionary<Player, List<BuffRuntimeData>>();

    private BuffEffectType _effectType;

    private float _effectValue = 0.0f;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

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
        BuffValueType valueType = GetEffectType<BuffValueType>(effectName, out _isValid);

        if (!_isValid) return;

        if (valueType == BuffValueType.Heal)
        {
            player.PlayerHealth.IncreaseHp(_effectValue);
            PlayEffect(player.transform);
            return;
        }

        float originalValue = 0.0f;

        switch (valueType)
        {
            case BuffValueType.AttackPower:
                originalValue = player.AtkBuff;
                player.SetAttackBuff(_effectValue);
                break;

            case BuffValueType.DamageReduction:
                originalValue = player.DamageReduction;
                player.ApplyDamageReduction(_effectValue);
                break;
        }

        if (!_runningBuffs.TryGetValue(player, out List<BuffRuntimeData> list))
        {
            list = new List<BuffRuntimeData>();
            _runningBuffs[player] = list;
        }

        BuffRuntimeData exist = list.Find(buff => buff.ValueType == valueType);
        if (exist != null)
        {
            // 갱신시키기
            StopCoroutine(exist.Coroutine);
            RestoreBuff(player, exist);
            list.Remove(exist);
        }

        BuffRuntimeData runtime = new BuffRuntimeData
        {
            ValueType = valueType,
            OriginalValue = originalValue
        };

        runtime.Coroutine = StartCoroutine(RemoveBuffAfterTime(player, runtime, _timer));

        list.Add(runtime);

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

    private IEnumerator RemoveBuffAfterTime(Player player, BuffRuntimeData runtime, float duration)
    {
        float timer = 0.0f;

        while (timer < duration)
        {
            if (BattleStateManager.Instance.CurrentState == BattleState.EnteringReroll)
                break;

            timer += Time.deltaTime;
            yield return null;
        }

        RestoreBuff(player, runtime);

        if (_runningBuffs.TryGetValue(player, out List<BuffRuntimeData> list))
        {
            list.Remove(runtime);

            // 버프가 다 사라지면 키도 삭제
            if (list.Count == 0)
            {
                _runningBuffs.Remove(player);
            }
        }
    }

    /*private IEnumerator RemoveBuffAfterTime(Player player, BuffValueType type, float originalValue, float duration)
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
    }*/

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
                PartialBuff();
                break;
            case BuffEffectType.AllBuff:
                ApplyBuffToAll();
                break;
        }
    }

    private void SoloBuff()
    {
        ApplyBuff(_player);
    }

    private void PartialBuff()
    {
        // 자기 자신은 무조건 적용
        ApplyBuff(_player);

        // 자기 자신 제외한 후보 리스트 생성
        List<Player> candidates = new List<Player>();
        _activePlayers = InGamePlayerSpawn.Instance.GetActivePlayers();

        foreach (Player player in _activePlayers)
        {
            if (player != _player)
            {
                candidates.Add(player);
            }
        }

        // 랜덤으로 최대 2명 선택
        int count = Mathf.Min(2, candidates.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);

            ApplyBuff(candidates[randomIndex]);

            // 중복 방지 위해 제거
            candidates.RemoveAt(randomIndex);
        }
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
        foreach (KeyValuePair<Player, List<BuffRuntimeData>> runningBuff in _runningBuffs)
        {
            Player player = runningBuff.Key;
            List<BuffRuntimeData> buffs = runningBuff.Value;

            foreach (BuffRuntimeData buff in buffs)
            {
                if (buff.Coroutine != null)
                {
                    StopCoroutine(buff.Coroutine);
                }

                RestoreBuff(player, buff);
                // 즉시 원래 값 복구
                /*switch (_valueType)
                {
                    case BuffValueType.AttackPower:
                        player.AtkBuff = 1.0f;
                        break;
                    case BuffValueType.DamageReduction:
                        player.ApplyDamageReduction(1.0f);
                        break;
                }*/

            }
        }

        _runningBuffs.Clear();
    }

    private void RestoreBuff(Player player, BuffRuntimeData runtime)
    {
        switch (runtime.ValueType)
        {
            case BuffValueType.AttackPower:
                player.SetAttackBuff(runtime.OriginalValue);
                print(player.name + "의 오리지널 데미지 버프 수치 : " + runtime.OriginalValue);
                break;

            case BuffValueType.DamageReduction:
                player.ApplyDamageReduction(runtime.OriginalValue);
                print(player.name + "의 오리지널 데미지 감소 버프 수치 : " + runtime.OriginalValue);
                break;
        }
    }

    public override void Excute()
    {
        Buff();
    }
}