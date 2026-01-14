using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{
    private event Action<int> _onUltUnlocked;
    public event Action<int> OnUltUnlocked
    {
        add { _onUltUnlocked += value;}
        remove { _onUltUnlocked -= value;}
    }

    private Dictionary<string, Vector3> _unitsPos = new Dictionary<string, Vector3>();
    private Dictionary<int, PlayerUnit> _activeUnits = new Dictionary<int, PlayerUnit>();
    private Dictionary<int, int> _unitLevels = new Dictionary<int, int>();

    private int _deadPlayerCount = 0;

    private void OnPlayerDie()
    {
        _deadPlayerCount = 0;

        foreach (KeyValuePair<int, PlayerUnit> player in _activeUnits)
        {
            Player character = player.Value.Unit;

            if (character.CurState() == CharacterState.Dead)
            {
                _deadPlayerCount++;
            }
        }

        if (_deadPlayerCount == _activeUnits.Count)
        {
            Debug.Log("All players defeated. Game Over.");
            BattleStateManager.Instance.SetState(BattleState.GameOver);
        }
    }

    public Vector3 GetOriginalPos(string unitName)
    {
        foreach (KeyValuePair<string, Vector3> player in _unitsPos)
        {
            if(player.Key == unitName)
            {
                return player.Value;
            }
        }

        return Vector3.zero;
    }

    public void RegisterUnit(CharacterData data, GameObject instance)
    {
        Player player = instance.GetComponent<Player>();
        player.OnDie += OnPlayerDie;

        _unitLevels[data.PlayerKey] = 1;
        _activeUnits[data.PlayerKey] = new PlayerUnit(data, player);
        _unitsPos[data.EngName] = instance.transform.position;

        //CharacterData a = _activeUnits[data.PlayerKey].CharacterInfo;
        //Debug.Log(a.EngName + " " + a.Hp + " " + a.Mp + " " + a.Atk);
    }

    public void UpgradeUnit(int key)
    {
        if (_activeUnits.TryGetValue(key, out PlayerUnit unit))
        {
            unit.LevelUp();
            _unitLevels[key] = unit.Level;
        }
    }

    public int GetUnitLevel(int key)
    {
        return _unitLevels.TryGetValue(key, out int level) ? level : 0;
    }

    public bool IsUltimateUnlocked(int key)
    {
        return _activeUnits[key].CharacterInfo.CanUseUlt;
    }

    public void NotifyUltimateUnlocked(int key)
    {
        _onUltUnlocked?.Invoke(key);
    }
}

public class PlayerUnit
{
    private Player _unitObj;
    public Player Unit
    {
        get { return _unitObj; }
    }

    private CharacterData _data;
    public CharacterData CharacterInfo
    {
        get { return _data; }
    }

    private PlayerStatData _upgradeData;

    private int _level = 1;
    public int Level
    {
        get { return _level; }
    }

    public PlayerUnit(CharacterData data, Player unit)
    {
        _data = data;
        //PlayerManager.Instance.GetPlayerFullData(data.Key);

        // 여기서 이제 캐릭터 스크립트에 스탯 설정하는 함수 있으면 스탯 세팅 해주면 댐
        _unitObj = unit;
        _unitObj.SetCharacterData(_data);
        _unitObj.SetCharacterActionInit();
    }

    public void LevelUp()
    {
        _level++;
        
        int maxLevel = _data.MaxLevel;

        // 이 조건문이 달성되면 그 카드는 제외시켜야함
        if (_level > maxLevel) return;

        // 레벨업 전 상태를 저장
        bool wasUnlocked = _data.CanUseUlt;
        // 레벨업 후 스탯과 CanUseUlt가 갱신됨
        _data.UpdatePlayerStats(_level);
        // 전 후 비교
        if (!wasUnlocked && _data.CanUseUlt)
        {
            BattleUnitManager.Instance.NotifyUltimateUnlocked(_data.PlayerKey);
        }

        //Debug.Log(_unitObj.name + " " + _data.Hp + " " + _data.Mp + " " + _data.Atk);

        _unitObj.SetCharacterData(_data);
        _unitObj.PlayerMana.SetMpData(_data.Mp, _data.MpTickRate);
        //curUnit.(_playerStat[levelUp]);
    }
}