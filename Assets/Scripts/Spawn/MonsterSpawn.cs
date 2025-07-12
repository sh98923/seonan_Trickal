using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    private enum FormationLayer
    {
        Front, Middle, Back
    }

    private Dictionary<FormationLayer, Vector3[]> _formationPositions = new Dictionary<FormationLayer, Vector3[]>
    {
        { FormationLayer.Front, new Vector3[3] },
        { FormationLayer.Middle, new Vector3[3] },
        { FormationLayer.Back, new Vector3[3] }
    };

    private int _startMonsterIndex;
    private int a = 0;
    private bool _spawned = false;

    private void Start()
    {
        _startMonsterIndex = SpawnManager.Instance.StartMonsterSpawnKey;
        LoadMonsterPos();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            _spawned = false;
            BattleStateManager.Instance.IsBattleStart = true;
            a++;
        }

        if (!_spawned && BattleStateManager.Instance.IsBattleStart)
        {
            int waveKey = StageManager.Instance.StageStartKey + a;

            Dictionary<int, WaveData> waveMonsters = WaveManager.Instance.GetWaveMonster(waveKey);

            SpawnWaveMonsters(waveMonsters);

            _spawned = true;
        }
    }

    private void LoadMonsterPos()
    {
        int index = 0;
        Vector3 origin = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

        foreach (Vector3[] positions in _formationPositions.Values)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                SpawnPosData spawnData = SpawnManager.Instance.GetMonsterData(_startMonsterIndex + index);
                Vector3 newPos = new Vector3(origin.x * spawnData.Ratio.x, origin.y * spawnData.Ratio.y, 0.0f);

                positions[i] = Camera.main.ScreenToWorldPoint(newPos);
                index++;
            }
        }
    }

    private void SpawnWaveMonsters(Dictionary<int, WaveData> waveMonsters)
    {
        foreach (WaveData data in waveMonsters.Values)
        {
            Vector3[] linePositions = _formationPositions[(FormationLayer)data.SpawnLine];
            MonsterData monsterData = CharacterManager.Instance.GetMonsterData(data.MonsterKey);

            for (int i = 0; i < data.Count && i < linePositions.Length; i++)
            {
                GameObject prefab = Resources.Load<GameObject>(monsterData.PrefabPath);
                GameObject monster = Instantiate(prefab, transform);
                monster.transform.position = linePositions[i];

                Vector3 finalPos = monster.transform.position;
                finalPos.z = 0.0f;
                monster.transform.localPosition = finalPos;
            }
        }
    }
}