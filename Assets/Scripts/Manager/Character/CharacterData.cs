using UnityEngine;

public class CharacterData
{
    /* ───────────────────────────────────────────────
     *  공통으로 사용되는 데이터 분류
     *  ConstantInfo = 생성 후 절대 변하지 않는 값
     *  Stats        = 레벨업/업데이트로 변할 수 있는 값
     * ─────────────────────────────────────────────── */
    private struct ConstantInfo
    {
        public string Type;
        public string Layer;
        public string EngName;
        public string PrefabPath;
        public string AtkType;
        public string Target;
        public string[] ProjectileKey;
        public string[] ActionImpact;
        public string[] FlashHit;
        public string[] ProjectileSpritePath;
    }

    private struct Stats
    {
        public float Hp;
        public float Atk;
        public float CriRate;
        public float AtkRange;

        // 변할 수도 있는 공격 데이터
        public float[] AtkSpeed;
        public bool[] IsRange;
    }

    private ConstantInfo _constant;
    private Stats _stats;

    private PlayerData _playerInfo;
    private PlayerStatData _playerStatData;
    private PlayerActionData _playerActionData;
    private PlayerSkillUpgradeData _playerSkillData;

    private MonsterData _monsterInfo;

    /* ─────────────────────────────
     *  외부로 제공되는 public Getter
     * ───────────────────────────── */
    public float Hp => _stats.Hp;
    public float Atk => _stats.Atk;
    public float CriRate => _stats.CriRate;
    public float AtkRange => _stats.AtkRange;
    public float[] AtkSpeed => _stats.AtkSpeed;
    public bool[] IsRangeAtk => _stats.IsRange;

    public string Layer => _constant.Layer;
    public string EngName => _constant.EngName;
    public string PrefabPath => _constant.PrefabPath;
    public string Type => _constant.Type;
    public string AtkType => _constant.AtkType;
    public string Target => _constant.Target;
    public string[] ProjectileKey => _constant.ProjectileKey;
    public string[] ProjectileSpritePath => _constant.ProjectileSpritePath;
    public string[] ActionImpact => _constant.ActionImpact;
    public string[] FlashHit => _constant.FlashHit;

    // 플레이어 전용
    public PlayerData PlayerInfo => _playerInfo;
    public int PlayerKey => _playerInfo.Key;
    public int MaxLevel => _playerInfo.MaxLevel;
    public int UpgradeKey => _playerInfo.StatUpgradeKey;

    public int UpgradeCost
    {
        get => _playerInfo.UpgradeCost;
        set => _playerInfo.UpgradeCost = value;
    }

    public float Mp => _playerStatData.Mp;
    public float MpTickRate => _playerStatData.MpTickRate;
    public float SkillRate => _playerStatData.SkillRate;
    public float Ultimate => _playerStatData.Ultimate;
    public float UltCoolTime => _playerStatData.UltCoolTime;

    public bool IsDeployed
    {
        get => _playerInfo.IsDeployed;
        set => _playerInfo.IsDeployed = value;
    }

    public bool CanUseUlt
    {
        get => _playerStatData.CanUseUlt;
        set => _playerStatData.CanUseUlt = value;
    }

    public string[] BuffEffect => _playerActionData.BuffEffect;
    public string[] ClipName => _playerActionData.ClipName;

    public float DotDamageRate => _playerSkillData.DotDamageRate;
    public float[] EffectValue => _playerSkillData.EffectValue;
    public float[] Duration => _playerSkillData.Duration;

    public bool IsRotationProjectile => _playerActionData.IsRotationProjectile;
    public bool[] IsEffectInFront => _playerActionData.IsEffectInFront;

    // 몬스터 전용
    public MonsterData MonsterInfo => _monsterInfo;
    public float HpPerWave => _monsterInfo.HpPerWave;
    public float HpGrowthRate => _monsterInfo.HpGrowthRate;
    public float AtkPerWave => _monsterInfo.AtkPerWave;
    public float AtkGrowthRate => _monsterInfo.AtkGrowthRate;


    /* ─────────────────────────────
     *        생성자 - 플레이어
     * ───────────────────────────── */
    public CharacterData(PlayerData playerData)
    {
        _playerInfo = playerData;

        _playerActionData = PlayerActionManager.Instance.GetPlayerActionData(playerData.AtkKey);

        UpdatePlayerStats(); // 스탯 먼저 갱신

        _constant = new ConstantInfo
        {
            Type = "Player",
            Layer = playerData.Layer,
            EngName = playerData.EngName,
            PrefabPath = playerData.CharacterPrefabPath,
            AtkType = playerData.AtkType,
            Target = playerData.Target,
            ProjectileKey = _playerActionData.ProjectileKey,

            ActionImpact = _playerActionData.ActionImpact,
            FlashHit = _playerActionData.Hittype,
            ProjectileSpritePath = _playerActionData.ProjectileSpritePath
        };
    }


    /* ──────────────────────────
     *       생성자 - 몬스터
     * ────────────────────────── */
    public CharacterData(MonsterData monsterData)
    {
        _monsterInfo = monsterData;

        _stats = new Stats
        {
            Hp = monsterData.Hp,
            Atk = monsterData.Atk,
            CriRate = monsterData.CriRate,
            AtkRange = monsterData.AtkRange,
            AtkSpeed = monsterData.AtkSpeed,
            IsRange = monsterData.IsRange
        };

        _constant = new ConstantInfo
        {
            Type = "Monster",
            Layer = monsterData.Layer,
            EngName = monsterData.EngName,
            PrefabPath = monsterData.PrefabPath,
            AtkType = monsterData.AtkType,
            Target = monsterData.Target,
            ProjectileKey = monsterData.ProjectileKey,

            ActionImpact = monsterData.ActionImpact,
            FlashHit = monsterData.FlashHit,
            ProjectileSpritePath = monsterData.ProjectileSpritePath
        };
    }


    /* ───────────────────────────
     *    Stats 적용 (플레이어용)
     * ─────────────────────────── */
    private void ApplyPlayerStats()
    {
        _stats.Hp = _playerStatData.Hp;
        _stats.Atk = _playerStatData.Atk;
        _stats.CriRate = _playerStatData.CriRate;
        _stats.AtkRange = _playerStatData.AtkRange;

        // 변할 수도 있는 데이터
        _stats.AtkSpeed = _playerActionData.ProjectileSpeed;
        _stats.IsRange = _playerActionData.IsRange;
    }


    /* ───────────────────────────
     *     레벨업 / 스탯 재계산
     * ─────────────────────────── */
    public void UpdatePlayerStats(int level = 0)
    {
        int statKey = _playerInfo.StatUpgradeKey + level;
        int skillKey = _playerInfo.SkillUpgradeKey + level;

        _playerStatData = PlayerStatManager.Instance.GetPlayerStatData(statKey);
        _playerSkillData = PlayerSkillUpgradeManager.Instance.GetPlayerSkillUpgradeData(skillKey);

        ApplyPlayerStats();
    }
}
