public class CharacterFullData
{
    private CharacterData _characterInfo;
    public CharacterData CharacterInfo
    {
        get { return _characterInfo; }
        set { _characterInfo = value; }
    }

    private PlayerData _playerInfo;
    public PlayerData PlayerInfo
    {
        get { return _playerInfo; }
        set { _playerInfo = value; }
    }

    private MonsterData _monsterInfo;
    public MonsterData MonsterInfo
    {
        get { return _monsterInfo; }
        set { _monsterInfo = value; }
    }

    // 공통 정보
    public float Hp
    {
        get { return _characterInfo.Hp; }
        set { _characterInfo.Hp = value; }
    }

    public float Atk
    {
        get { return _characterInfo.Atk; }
        set { _characterInfo.Atk = value; }
    }

    public float AtkRange
    {
        get { return _characterInfo.AtkRange; }
        set { _characterInfo.AtkRange = value; }
    }

    public float AtkCoolTime
    {
        get { return _characterInfo.AtkCoolTime; }
        set { _characterInfo.AtkCoolTime = value; }
    }

    public float CriRate
    {
        get { return _characterInfo.CriRate; }
        set { _characterInfo.CriRate = value; }
    }

    public string EngName
    {
        get { return _characterInfo.EngName; }
    }

    public string PrefabPath
    {
        get { return _characterInfo.PrefabPath; }
    }

    // 플레이어 정보
    public int MaxLevel
    {
        get { return _playerInfo.MaxLevel; }
    }

    public int CharacterKey
    {
        get { return _playerInfo.CharacterKey; }
    }

    public int UpgradeKey
    {
        get { return _playerInfo.UpgradeKey; }
    }

    public int CardUpgradeCost
    {
        get { return _playerInfo.CardUpgradeCost; }
        set { _playerInfo.CardUpgradeCost = value; }
    }

    public float Mp
    {
        get { return _playerInfo.Mp; }
        set { _playerInfo.Mp = value; }
    }

    public float SkillRate
    {
        get { return _playerInfo.SkillRate; }
        set { _playerInfo.SkillRate = value; }
    }

    public float Ultimate
    {
        get { return _playerInfo.Ultimate; }
        set { _playerInfo.Ultimate = value; }
    }

    public float UltCoolTime
    {
        get { return _playerInfo.UltCoolTime; }
        set { _playerInfo.UltCoolTime = value; }
    }

    public bool CanUseUlt
    {
        get { return _playerInfo.CanUseUlt; }
        set { _playerInfo.CanUseUlt = value; }
    }

    // 몬스터 정보
    public float HpPerWave
    {
        get { return _monsterInfo.HpPerWave; }
        set { _monsterInfo.HpPerWave = value; }
    }

    public float HpGrowthRate
    {
        get { return _monsterInfo.HpGrowthRate; }
        set { _monsterInfo.HpGrowthRate = value; }
    }

    public float AtkPerWave
    {
        get { return _monsterInfo.AtkPerWave; }
        set { _monsterInfo.AtkPerWave = value; }
    }

    public float AtkGrowthRate
    {
        get { return _monsterInfo.AtkGrowthRate; }
        set { _monsterInfo.AtkGrowthRate = value; }
    }

    public string Type
    {
        get { return _monsterInfo.Type; }
        set { _monsterInfo.Type = value; }
    }

    public string WeaponPath
    {
        get { return _monsterInfo.WeaponPath; }
        set { _monsterInfo.WeaponPath = value; }
    }

    public CharacterFullData(CharacterData characterInfo, PlayerData playerInfo)
    {
        _characterInfo = characterInfo;
        _playerInfo = playerInfo;
    }

    public CharacterFullData(CharacterData characterInfo, MonsterData monsterInfo)
    {
        _characterInfo = characterInfo;
        _monsterInfo = monsterInfo;
    }

    public void UpdatePlayerStat(PlayerUpgradeData data)
    {
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
