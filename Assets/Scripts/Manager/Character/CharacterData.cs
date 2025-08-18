public class CharacterData
{
    private struct SharedStats
    {
        public string Type;
        public string Layer;
        public string EngName;
        public string PrefabPath;
        public string AttackType;
        public string Target;
        public string ProjectileKey;
        public string[] AttackEffect;
        public string[] ProjectileSpritePath;
        public float Hp;
        public float Atk;
        public float CriRate;
        public float AtkRange;
        public float AtkCoolTime;
        public float[] AtkSpeed;
        public int[] ProjectilePool;
        public bool[] IsRange;
    }

    private SharedStats _shared;
    private PlayerData _playerInfo;
    private PlayerStatData _playerStat;
    private PlayerAttackData _playerAttack;
    private MonsterData _monsterInfo;

    // 공통 정보
    public float Hp
    {
        get => _shared.Hp;
        set => _shared.Hp = value;
    }

    public float Atk
    {
        get => _shared.Atk;
        set => _shared.Atk = value;
    }

    public float AtkRange
    {
        get => _shared.AtkRange;
        set => _shared.AtkRange = value;
    }

    public float AtkCoolTime
    {
        get => _shared.AtkCoolTime;
        set => _shared.AtkCoolTime = value;
    }

    public float CriRate
    {
        get => _shared.CriRate;
        set => _shared.CriRate = value;
    }

    public float[] AtkSpeed
    {
        get => _shared.AtkSpeed;
    }

    public string AtkType
    {
        get => _shared.AttackType;
        set => _shared.AttackType = value;
    }
    public string Target
    {
        get { return _shared.Target; }
    }

    public string Layer => _shared.Layer;
    public string EngName => _shared.EngName;
    public string PrefabPath => _shared.PrefabPath;

    public string Type
    {
        get => _shared.Type;
        set => _shared.Type = value;
    }

    public string ProjectileKey
    {
        get => _shared.ProjectileKey;
        set => _shared.ProjectileKey = value;
    }

    public string[] ProjectileSpritePath
    {
        get { return _shared.ProjectileSpritePath; }
    }

    public string[] AttackEffect
    {
        get { return _shared.AttackEffect; }
    }

    public int[] PoolSize
    {
        get { return _shared.ProjectilePool; }
    }

    public bool[] IsRangeAttack
    {
        get { return _shared.IsRange; }
    }

    // 플레이어 전용
    public PlayerData PlayerInfo => _playerInfo;
    public int PlayerKey => _playerInfo.Key;
    public int MaxLevel => _playerInfo.MaxLevel;
    public int UpgradeKey => _playerInfo.UpgradeKey;

    public int UpgradeCost
    {
        get => _playerInfo.UpgradeCost;
        set => _playerInfo.UpgradeCost = value;
    }

    public float Mp
    {
        get => _playerStat.Mp;
        set => _playerStat.Mp = value;
    }

    public float SkillRate
    {
        get => _playerStat.SkillRate;
        set => _playerStat.SkillRate = value;
    }

    public float Ultimate
    {
        get => _playerStat.Ultimate;
        set => _playerStat.Ultimate = value;
    }

    public float UltCoolTime
    {
        get => _playerStat.UltCoolTime;
        set => _playerStat.UltCoolTime = value;
    }

    public bool CanUseUlt
    {
        get => _playerStat.CanUseUlt;
        set => _playerStat.CanUseUlt = value;
    }

    public bool[] IsEffectInFront
    {
        get => _playerAttack.IsEffectInFront;
    }

    // 몬스터 전용
    public MonsterData MonsterInfo => _monsterInfo;

    public float HpPerWave
    {
        get => _monsterInfo.HpPerWave;
        set => _monsterInfo.HpPerWave = value;
    }

    public float HpGrowthRate
    {
        get => _monsterInfo.HpGrowthRate;
        set => _monsterInfo.HpGrowthRate = value;
    }

    public float AtkPerWave
    {
        get => _monsterInfo.AtkPerWave;
        set => _monsterInfo.AtkPerWave = value;
    }

    public float AtkGrowthRate
    {
        get => _monsterInfo.AtkGrowthRate;
        set => _monsterInfo.AtkGrowthRate = value;
    }

    // 생성자 - 플레이어용
    public CharacterData(PlayerData playerData)
    {
        _playerInfo = playerData;

        // 공격 데이터 가져오기
        _playerAttack = PlayerAttackManager.Instance.GetplayerAttackData(playerData.AttackKey);

        // 공통 Stats 초기화
        _shared = new SharedStats
        {
            Hp = _playerStat.Hp,
            Atk = _playerStat.Atk,
            CriRate = _playerStat.CriRate,
            AtkRange = _playerStat.AtkRange,

            Type = "Player",
            Layer = playerData.Layer,
            Target = playerData.Target,
            EngName = playerData.EngName,
            AttackType = playerData.AttackType,
            PrefabPath = playerData.CharacterPrefabPath,

            // 공격 관련 데이터
            IsRange = _playerAttack.IsRange,
            AtkSpeed = _playerAttack.AtkSpeed,
            AttackEffect = _playerAttack.AtkImpact,
            ProjectileKey = _playerAttack.ProjectileKey,
            ProjectileSpritePath = _playerAttack.ProjectileSpritePath,
        };

        // 플레이어 스탯 업그레이드 적용
        UpdatePlayerStat(PlayerStatManager.Instance.GetPlayerStatData(playerData.UpgradeKey));
    }

    // 생성자 - 몬스터용
    public CharacterData(MonsterData monsterData)
    {
        _monsterInfo = monsterData;

        // 공통 Stats 초기화
        _shared = new SharedStats
        {
            Hp = monsterData.Hp,
            Atk = monsterData.Atk,
            CriRate = monsterData.CriRate,
            AtkRange = monsterData.AtkRange,

            Type = "Monster",
            Layer = monsterData.Layer,
            EngName = monsterData.EngName,
            AttackType = monsterData.AttackType,
            PrefabPath = monsterData.PrefabPath,

            // 공격 관련 데이터
            Target = monsterData.Target,
            IsRange = monsterData.IsRange,
            AtkSpeed = monsterData.AtkSpeed,
            AttackEffect = monsterData.AtkEffect,
            ProjectileKey = monsterData.ProjectileKey,
            ProjectileSpritePath = monsterData.ProjectileSpritePath,
        };
    }

    public void UpdatePlayerStat(PlayerStatData data)
    {
        _playerStat = data;

        Hp = data.Hp;
        Atk = data.Atk;
        AtkRange = data.AtkRange;
        CriRate = data.CriRate;

        Mp = data.Mp;
        SkillRate = data.SkillRate;
        Ultimate = data.Ultimate;
        UltCoolTime = data.UltCoolTime;
        CanUseUlt = data.CanUseUlt;
    }
}
