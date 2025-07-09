using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private SpawnPosData _playerData;
    private readonly int _spawnPosCount = 9;
    private int _startSpawnKey;

    private void Start()
    {
        _startSpawnKey = SpawnManager.Instance.StartPlayerSpawnKey;
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Vector3 originPos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
        Transform parent = GameObject.Find("SpawnPlayer").transform;
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Elf/ElfArcher");
        
        for(int i = 0; i < _spawnPosCount; i++)
        {
            GameObject player = Instantiate(playerPrefab, parent);
            _playerData = SpawnManager.Instance.GetPlayerData(_startSpawnKey + i);

            player.name += ("_" + i);
            player.layer = LayerMask.NameToLayer(_playerData.Layer);

            Vector3 newPos = new Vector3(originPos.x * _playerData.Ratio.x, originPos.y * _playerData.Ratio.y, 0.0f);

            player.transform.position = Camera.main.ScreenToWorldPoint(newPos);
            Vector3 pos = player.transform.position;
            pos.z = 0.0f;

            player.transform.position = pos;
        }
    }
}