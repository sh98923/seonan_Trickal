using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{
    private Dictionary<int, PlayerUnit> _activeUnits = new Dictionary<int, PlayerUnit>();

    public void RegisterUnit(PlayerData data, GameObject instance)
    {
        _activeUnits[data.Key] = new PlayerUnit(data, instance);
    }

    public void UpgradeUnit(int key)
    {
        if (_activeUnits.TryGetValue(key, out PlayerUnit unit))
        {
            unit.LevelUp();
        }
    }
}

public class PlayerUnit
{
    private Player _playerUnitObj;

    private PlayerStatData[] _playerStats;

    private int _level = 0;

    public PlayerUnit(PlayerData data, GameObject unit)
    {
        _playerStats = PlayerStatManager.Instance.GetPlayerStatData(data.StatKey);

        // 여기서 이제 캐릭터 스크립트에 스탯 설정하는 함수 있으면 스탯 세팅 해주면 댐
        _playerUnitObj = unit.GetComponent<Player>();
        _playerUnitObj.SetPlayerStat(_playerStats[_level]);
    }

    public void LevelUp()
    {
        _level++;
        // 이 조건문이 달성되면 그 카드는 제외시켜야함
        if (_level >= _playerStats.Length) return;

        Debug.Log(_playerUnitObj.name + " " + _playerStats[_level].Hp + " " + _playerStats[_level].Mp + " " + _playerStats[_level].Atk);
        _playerUnitObj.SetPlayerStat(_playerStats[_level]);
        //curUnit.(_playerStat[levelUp]);
    }
}