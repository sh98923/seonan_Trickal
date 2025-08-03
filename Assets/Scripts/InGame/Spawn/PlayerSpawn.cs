using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    private struct FormationSlot
    {
        public Vector3 Position;
        public bool IsOccupied;
    }

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

    private string _sceneName;
    private int _startPlayerIndex;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;

        if (_sceneName == "InGameScene")
        {
            LoadPlayerPos();
            // 룰렛 만들게되면 룰렛에 나온 애들만 스폰
            GameManager.Instance.RegisterDeckUnits(transform);
        }
    }

    private void Start()
    {
        _startPlayerIndex = SpawnManager.Instance.StartPlayerSpawnKey;
        InitDeployPos();
    }

    private void InitDeployPos()
    {
        int index = 0;
        Vector3 origin = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

        foreach (FormationSlot[] positions in _deployedCharacters.Values)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                SpawnPosData spawnData = SpawnManager.Instance.GetPlayerSpawnData(_startPlayerIndex + index);
                Vector3 newPos = new Vector3(origin.x * spawnData.Ratio.x, origin.y * spawnData.Ratio.y, Camera.main.nearClipPlane);

                positions[i].Position = Camera.main.ScreenToWorldPoint(newPos);
                positions[i].IsOccupied = false;
                index++;
            }
        }
    }

    private void LoadPlayerPos()
    {
        List<PlayerData> datas = GameManager.Instance.SpawnablePlayerDatas;

        for(int i = 0; i < datas.Count; i++)
        {
            _deployedDatas.Add(datas[i]);
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