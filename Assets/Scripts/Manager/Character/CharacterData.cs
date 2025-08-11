public class CharacterData
{
    private struct SharedStats
    {
        public string Type;
        public string Layer;
        public string EngName;
        public string PrefabPath;
        public string ProjectilePath;
        public string AttackType;
        public string Target;
        public string[] ProjectileSpritePath;
        public float Hp;
        public float Atk;
        public float CriRate;
        public float AtkRange;
        public float AtkCoolTime;
        public int[] ProjectilePool;
        public bool[] IsRange;
    }

    private SharedStats _shared;
    private PlayerData _playerInfo;
    private PlayerStatData _playerStat;
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

    public string ProjectilePath
    {
        get => _shared.ProjectilePath;
        set => _shared.ProjectilePath = value;
    }

    public string[] ProjectileSpritePath
    {
        get { return _shared.ProjectileSpritePath; }
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

        _shared = new SharedStats
        {
            Hp = _playerStat.Hp,
            Atk = _playerStat.Atk,
            AtkRange = _playerStat.AtkRange,
            AtkCoolTime = _playerStat.AtkCoolTime,
            CriRate = _playerStat.CriRate,

            AttackType = playerData.AttackType,
            Layer = playerData.Layer,
            EngName = playerData.EngName,
            PrefabPath = playerData.CharacterPrefabPath,
            Type = "Player",
            Target = playerData.Target,
            ProjectilePath = playerData.ProjectilePath,
            IsRange = playerData.IsRange,
            ProjectileSpritePath = playerData.ProjectileSpritePath,
            ProjectilePool = playerData.ProjectilePool
};

        UpdatePlayerStat(PlayerStatManager.Instance.GetPlayerStatData(playerData.UpgradeKey));
    }

    // 생성자 - 몬스터용
    public CharacterData(MonsterData monsterData)
    {
        _monsterInfo = monsterData;

        _shared = new SharedStats
        {
            Hp = monsterData.Hp,
            Atk = monsterData.Atk,
            AtkRange = monsterData.AtkRange,
            AtkCoolTime = monsterData.AtkCoolTime,
            CriRate = monsterData.CriRate,

            AttackType = monsterData.AttackType,
            Layer = monsterData.Layer,
            EngName = monsterData.EngName,
            PrefabPath = monsterData.PrefabPath,
            Type = "Monster",
            Target = monsterData.Target,
            ProjectilePath = monsterData.ProjectilePath,
            IsRange = monsterData.IsRange,
            ProjectileSpritePath = monsterData.ProjectileSpritePath,
            ProjectilePool = monsterData.ProjectilePool
        };
    }

    public void UpdatePlayerStat(PlayerStatData data)
    {
        _playerStat = data;

        Hp = data.Hp;
        Atk = data.Atk;
        AtkRange = data.AtkRange;
        AtkCoolTime = data.AtkCoolTime;
        CriRate = data.CriRate;

        Mp = data.Mp;
        SkillRate = data.SkillRate;
        Ultimate = data.Ultimate;
        UltCoolTime = data.UltCoolTime;
        CanUseUlt = data.CanUseUlt;
    }
}
