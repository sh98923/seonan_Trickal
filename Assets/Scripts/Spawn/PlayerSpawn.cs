using System.Collections.Generic;
using UnityEngine;

public struct FormationSlot
{
    public Vector3 Position;
    public bool IsOccupied;
}

public class PlayerSpawn : MonoBehaviour
{
    private enum FormationLayer
    {
        Front, Middle, Back
    }

    private Dictionary<int, FormationSlot[]> _deployedCharacters = new Dictionary<int, FormationSlot[]>
    {
        { (int)FormationLayer.Front, new FormationSlot[3] },
        { (int)FormationLayer.Middle, new FormationSlot[3] },
        { (int)FormationLayer.Back, new FormationSlot[3] }
    };

    private List<PlayerData> _deployedDatas = new List<PlayerData>();
    public List<PlayerData> DeployedDatas
    {
        get { return _deployedDatas; }
    }

    private int _startPlayerIndex;
    
    private void Start()
    {
        _startPlayerIndex = SpawnManager.Instance.StartPlayerSpawnKey;
        LoadPlayerPos();
    }

    private void LoadPlayerPos()
    {
        int index = 0;
        Vector3 origin = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

        foreach (FormationSlot[] positions in _deployedCharacters.Values)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                SpawnPosData spawnData = SpawnManager.Instance.GetPlayerData(_startPlayerIndex + index);
                Vector3 newPos = new Vector3(origin.x * spawnData.Ratio.x, origin.y * spawnData.Ratio.y, 0.0f);

                positions[i].Position = Camera.main.ScreenToWorldPoint(newPos);
                positions[i].IsOccupied = false;
                index++;
            }
        }
    }

    public Vector3 SetPlayerPos(PlayerData data)
    {
        FormationSlot[] slots = _deployedCharacters[data.SpawnLine];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsOccupied) continue;

            if(!BattleStateManager.Instance.IsBattleStart)
                _deployedDatas.Add(data);

            slots[i].IsOccupied = true;
            return slots[i].Position;
        }

        return Vector3.zero;
    }
}