using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private string sceneName;
    private int _startPlayerIndex;

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        switch (sceneName)
        {
            case "StageSelectScene":
                _startPlayerIndex = SpawnManager.Instance.StartPlayerSpawnKey;
                LoadPlayerPos();
                break;
            case "InGameScene":
                // 룰렛 만들게되면 룰렛에 나온 애들만 스폰
                GameManager.Instance.RegisterDeckUnits();
                break;
        }
    }

    private void LoadPlayerPos()
    {
        int index = 0;
        Vector3 origin = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

        foreach (FormationSlot[] positions in _deployedCharacters.Values)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                SpawnPosData spawnData = SpawnManager.Instance.GetPlayerSpawnData(_startPlayerIndex + index);
                Vector3 newPos = new Vector3(origin.x * spawnData.Ratio.x, origin.y * spawnData.Ratio.y, 0.0f);

                positions[i].Position = Camera.main.ScreenToWorldPoint(newPos);
                positions[i].IsOccupied = false;
                index++;
            }
        }
    }

    public bool IsDataDeployed(PlayerData data)
    {
        for (int i = 0; i < _deployedDatas.Count; i++)
        {
            if (_deployedDatas[i].Key == data.Key)
                return true;
        }
        return false;
    }

    public Vector3 SetPlayerPos(PlayerData playerData)
    {
        CharacterData data = CharacterManager.Instance.GetCharacterData(playerData.CharacterKey);
        FormationSlot[] slots = _deployedCharacters[data.SpawnLine];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsOccupied) continue;

            _deployedDatas.Add(playerData);

            slots[i].IsOccupied = true;
            return slots[i].Position;
        }

        return Vector3.zero;
    }
}