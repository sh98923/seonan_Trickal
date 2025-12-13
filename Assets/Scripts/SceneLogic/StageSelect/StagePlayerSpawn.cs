using System.Collections.Generic;
using UnityEngine;

public enum BattleLine
{
    Front, Middle, Back
}

public class StagePlayerSpawn : MonoBehaviour
{
    private struct FormationSlot
    {
        public GameObject PlayerCharacter;
        public Vector3 Position;
        public PlayerData PlayerData;
        public bool IsOccupied;
    }

    // 각 슬롯을 키:SlotIndex 형태로 저장, 각 슬롯 안에 PlayerData와 GameObject 포함
    private Dictionary<int, FormationSlot[]> _deployedCharacters = new Dictionary<int, FormationSlot[]>
    {
        { (int)BattleLine.Front, new FormationSlot[3] },
        { (int)BattleLine.Middle, new FormationSlot[3] },
        { (int)BattleLine.Back, new FormationSlot[3] }
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

                ref FormationSlot slot = ref slots[i]; // ref로 배열 안 구조체 직접 수정
                slot.Position = Camera.main.ScreenToWorldPoint(newPos);
                slot.IsOccupied = false;
                slot.PlayerData = default;
                slot.PlayerCharacter = null;

                index++;
            }
        }
    }

    // 슬롯에서 배치된 캐릭터 제거
    public bool CheckAndRemoveDeployed(PlayerData data)
    {
        foreach (KeyValuePair<int, FormationSlot[]> layerEntry in _deployedCharacters)
        {
            FormationSlot[] slots = layerEntry.Value;
            for (int i = 0; i < slots.Length; i++)
            {
                ref FormationSlot slot = ref slots[i];
                if (slot.IsOccupied && slot.PlayerData.Key == data.Key)
                {
                    if (slot.PlayerCharacter != null)
                    {
                        Destroy(slot.PlayerCharacter);
                        slot.PlayerCharacter = null;
                    }

                    slot.IsOccupied = false;
                    slot.PlayerData = default;
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
            ref FormationSlot slot = ref slots[i];
            if (!slot.IsOccupied)
            {
                slot.IsOccupied = true;
                slot.PlayerData = playerData;
                return slot.Position;
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
            ref FormationSlot slot = ref slots[i];

            // 이미 생성된 경우 중복 방지
            if (slot.IsOccupied && slot.PlayerData.Key == playerData.Key && slot.PlayerCharacter == null)
            {
                GameObject prefab = Resources.Load<GameObject>(playerData.CharacterPrefabPath);
                GameObject player = Instantiate(prefab, transform);

                player.name = playerData.EngName;
                player.layer = LayerMask.NameToLayer(playerData.Layer);
                player.transform.position = spawnPos;

                slot.PlayerCharacter = player;
                break;
            }
        }
    }
}
