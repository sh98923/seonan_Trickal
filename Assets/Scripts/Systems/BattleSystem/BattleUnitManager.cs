using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{
    private Dictionary<string, Vector3> _unitsPos = new Dictionary<string, Vector3>();
    private Dictionary<int, PlayerUnit> _activeUnits = new Dictionary<int, PlayerUnit>();

    private int _deadPlayerCount = 0;
    private int _curLevel;
    public int CurLevel
    {
        get { return _curLevel; }
    }

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

        _unitsPos[data.EngName] = instance.transform.position;
        _activeUnits[data.PlayerKey] = new PlayerUnit(data, player);

        CharacterData a = _activeUnits[data.PlayerKey].CharacterInfo;

        //Debug.Log(a.EngName + " " + a.Hp + " " + a.Mp + " " + a.Atk);
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

    private int _level = 0;
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
        if (_level >= maxLevel) return;

        _data.UpdatePlayerStats(_level);

        //Debug.Log(_unitObj.name + " " + _data.Hp + " " + _data.Mp + " " + _data.Atk);

        _unitObj.SetCharacterData(_data);
        _unitObj.GetComponent<PlayerHealth>().UpgradeHp();
        _unitObj.GetComponent<PlayerMp>().SetMpData(_data.Mp, _data.MpTickRate);
        //curUnit.(_playerStat[levelUp]);
    }
}