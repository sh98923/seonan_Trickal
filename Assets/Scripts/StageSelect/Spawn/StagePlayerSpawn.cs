using System.Collections.Generic;
using UnityEngine;

public class StagePlayerSpawn : MonoBehaviour
{
    private struct FormationSlot
    {
        public Vector3 Position;
        public bool IsOccupied;
        public PlayerData PlayerData;
        public GameObject PlayerCharacter;
    }

    private enum FormationLayer
    {
        Front, Middle, Back
    }

    // 각 슬롯을 키:SlotIndex 형태로 저장, 각 슬롯 안에 PlayerData와 GameObject 포함
    private Dictionary<int, FormationSlot[]> _deployedCharacters = new Dictionary<int, FormationSlot[]>
    {
        { (int)FormationLayer.Front, new FormationSlot[3] },
        { (int)FormationLayer.Middle, new FormationSlot[3] },
        { (int)FormationLayer.Back, new FormationSlot[3] }
    };

    private int _startPlayerIndex;

    private void Start()
    {
        _startPlayerIndex = SpawnManager.Instance.StartPlayerSpawnKey;
        InitDeployPos();
    }

    private void InitDeployPos()
    {
        int index = 0;
        Vector3 origin = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

        foreach (FormationSlot[] slots in _deployedCharacters.Values)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                SpawnPosData spawnData = SpawnManager.Instance.GetPlayerSpawnData(_startPlayerIndex + index);
                Vector3 newPos = new Vector3(origin.x * spawnData.Ratio.x, origin.y * spawnData.Ratio.y, Camera.main.nearClipPlane);

                slots[i].Position = Camera.main.ScreenToWorldPoint(newPos);
                slots[i].IsOccupied = false;
                slots[i].PlayerData = default;
                slots[i].PlayerCharacter = null;

                index++;
            }
        }
    }

    // 슬롯에서 배치된 캐릭터 제거
    public bool CheckAndRemoveDeployed(PlayerData data)
    {
        foreach (var kvp in _deployedCharacters)
        {
            FormationSlot[] slots = kvp.Value;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsOccupied && slots[i].PlayerData.Key == data.Key)
                {
                    // GameObject 제거
                    if (slots[i].PlayerCharacter != null)
                    {
                        Destroy(slots[i].PlayerCharacter);
                        slots[i].PlayerCharacter = null;
                    }

                    slots[i].IsOccupied = false;
                    slots[i].PlayerData = default;
                    return true;
                }
            }
        }

        return false; // 배치되어 있지 않음
    }

    // 빈 슬롯 찾아서 PlayerData 등록 후 위치 반환
    public Vector3 SetPlayerPos(PlayerData playerData)
    {
        if (!_deployedCharacters.TryGetValue(playerData.SpawnLine, out FormationSlot[] slots))
            return Vector3.zero;

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsOccupied)
            {
                slots[i].IsOccupied = true;
                slots[i].PlayerData = playerData;
                return slots[i].Position;
            }
        }

        return Vector3.zero; // 빈 슬롯 없음
    }

    // 실제 GameObject 생성 및 슬롯에 저장
    public void SpawnPlayerAtPosition(PlayerData playerData, Vector3 spawnPos)
    {
        if (!_deployedCharacters.TryGetValue(playerData.SpawnLine, out FormationSlot[] slots))
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsOccupied && slots[i].PlayerData.Key == playerData.Key && slots[i].PlayerCharacter == null)
            {
                GameObject prefab = Resources.Load<GameObject>(playerData.CharacterPrefabPath);
                GameObject player = Instantiate(prefab, transform);

                player.name = playerData.EngName;
                player.layer = LayerMask.NameToLayer(playerData.Layer);
                player.transform.position = spawnPos;

                slots[i].PlayerCharacter = player;
                break;
            }
        }
    }
}
