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

    private Dictionary<FormationLayer, FormationSlot[]> _formationPositions = new Dictionary<FormationLayer, FormationSlot[]>
    {
        { FormationLayer.Front, new FormationSlot[3] },
        { FormationLayer.Middle, new FormationSlot[3] },
        { FormationLayer.Back, new FormationSlot[3] }
    };

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

        foreach (FormationSlot[] positions in _formationPositions.Values)
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
        FormationLayer spawnLine = (FormationLayer)data.SpawnLine;
        FormationSlot[] slots = _formationPositions[spawnLine];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsOccupied) continue;

            slots[i].IsOccupied = true;
            return slots[i].Position;
        }

        return Vector3.zero;
    }
}