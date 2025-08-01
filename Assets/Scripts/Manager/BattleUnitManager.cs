using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{
    private Dictionary<int, PlayerUnit> _activeUnits = new Dictionary<int, PlayerUnit>();

    private int _alivePlayerCount = 0;
    private int _curLevel;
    public int CurLevel
    {
        get { return _curLevel; }
    }

    public void RegisterUnit(PlayerData data, GameObject instance)
    {
        Player player = instance.GetComponent<Player>();
        player.OnDie += OnPlayerDie;
        _alivePlayerCount++;

        _activeUnits[data.Key] = new PlayerUnit(data, instance);
    }

    private void OnPlayerDie(Character ch)
    {
        _alivePlayerCount--;
        if (_alivePlayerCount <= 0)
        {
            Debug.Log("All players defeated. Game Over.");
            BattleStateManager.Instance.SetState(BattleState.GameOver);
        }
    }

    public void UpgradeUnit(int key)
    {
        if (_activeUnits.TryGetValue(key, out PlayerUnit unit))
        {
            unit.LevelUp();
            _curLevel = unit.Level;
        }
    }
}

public class PlayerUnit
{
    private Player _unitObj;

    private CharacterFullData _fullData;
    private PlayerUpgradeData _upgradeData;
    private int _level = 0;
    public int Level
    {
        get { return _level; }
    }

    public PlayerUnit(PlayerData data, GameObject unit)
    {
        _fullData.playerData = data;
        _fullData.characterData = CharacterManager.Instance.GetCharacterData(data.CharacterKey);

        // 여기서 이제 캐릭터 스크립트에 스탯 설정하는 함수 있으면 스탯 세팅 해주면 댐
        _unitObj = unit.GetComponent<Player>();
        _unitObj.SetCharacterData(_fullData);
    }

    public void LevelUp()
    {
        int maxLevel = _fullData.playerData.Value.MaxLevel;

        // 이 조건문이 달성되면 그 카드는 제외시켜야함
        if (_level >= maxLevel) return;

        SetCharacterFullData();

        Debug.Log(_unitObj.name + " " + _fullData.characterData.Hp + " " + _fullData.playerData.Value.Mp + " " + _fullData.characterData.Atk);

        _unitObj.SetCharacterData(_fullData);
        //curUnit.(_playerStat[levelUp]);
    }

    private void SetCharacterFullData()
    {
        int startKey = _fullData.playerData.Value.UpgradeKey;
        int curKey = startKey + _level;

        _upgradeData = PlayerUpgradeManager.Instance.GetPlayerStatData(curKey);

        UpdateFullDataStats(_upgradeData);

         _level++;
    }

    private void UpdateFullDataStats(PlayerUpgradeData upgradeData)
    {
        // CharacterData 업데이트
        CharacterData characterData = _fullData.characterData;
        characterData.Hp = upgradeData.Hp;
        characterData.Atk = upgradeData.Atk;
        characterData.AtkRange = upgradeData.AtkRange;
        characterData.AtkCoolTime = upgradeData.AtkCoolTime;
        characterData.CriRate = upgradeData.CriRate;
        _fullData.characterData = characterData;

        // PlayerData 업데이트 (nullable struct 처리)
        if (_fullData.playerData is PlayerData pData)
        {
            pData.Mp = upgradeData.Mp;
            pData.SkillRate = upgradeData.SkillRate;
            pData.Ultimate = upgradeData.Ultimate;
            pData.UltCoolTime = upgradeData.UltCoolTime;
            pData.CanUseUlt = upgradeData.CanUseUlt;
            _fullData.playerData = pData;
        }
    }
}