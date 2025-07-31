using System.Collections;
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

    private readonly int _poolSize = 5;

    private int _aliveMonsterCount = 0;
    private int _startMonsterIndex;
    private int a = 0;

    private bool _spawned = false;

    private void Start()
    {
        _startMonsterIndex = SpawnManager.Instance.StartMonsterSpawnKey;
        LoadMonsterPool();
        LoadMonsterPos();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnReroll += HandleReroll;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= HandleReroll;
    }

    private void HandleReroll()
    {
        if (_spawned) return;

        int waveKey = GameManager.Instance.WaveKey + a;
        StageData wave = StageManager.Instance.GetStageData(waveKey);
        Dictionary<int, WaveData> waveMonsters = WaveManager.Instance.GetWaveMonster(waveKey);

        SpawnWaveMonsters(waveMonsters, wave.Wave);
        _spawned = true;
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

    private void LoadMonsterPool()
    {
        int waveCnt = GameManager.Instance.WaveCount;
        List<MonsterData> stageMonsters = new List<MonsterData>();
        HashSet<int> stageMonsterKeys = WaveManager.Instance.GetStageMonster(waveCnt);

        foreach (int monsterKey in stageMonsterKeys)
        {
            stageMonsters.Add(CharacterManager.Instance.GetMonsterData(monsterKey));
        }

        // 그 스테이지에서 나오는 몬스터만 풀링
        foreach (MonsterData monsterData in stageMonsters)
        {
            GameObject prefab = Resources.Load<GameObject>(monsterData.PrefabPath);
            PoolingManager.Instance.Add(monsterData.Name, _poolSize, prefab, transform);
        }
    }

    private void SpawnWaveMonsters(Dictionary<int, WaveData> waveMonsters, int wave)
    {
        _aliveMonsterCount = 0;

        foreach (WaveData data in waveMonsters.Values)
        {
            Vector3[] linePositions = _formationPositions[(FormationLayer)data.SpawnLine];
            MonsterData monsterData = CharacterManager.Instance.GetMonsterData(data.MonsterKey);

            for (int i = 0; i < data.Count && i < linePositions.Length; i++)
            {
                GameObject monsterObj = PoolingManager.Instance.Pop(monsterData.Name);
                monsterObj.transform.position = linePositions[i];
                Vector3 finalPos = monsterObj.transform.position;
                finalPos.z = 0.0f;
                monsterObj.transform.localPosition = finalPos;

                Monster monster = monsterObj.GetComponent<Monster>();
                monster.SetMonsterStat(monsterData, wave);
                monster.OnDie -= OnMonsterDie;
                monster.OnDie += OnMonsterDie;

                _aliveMonsterCount++;
            }
        }
    }

    private void OnMonsterDie(Character ch)
    {
        _aliveMonsterCount--;

        if (_aliveMonsterCount <= 0)
        {
            Debug.Log("Wave cleared.");
            BattleStateManager.Instance.SetState(BattleState.MonstersDefeated);
            StartCoroutine(MonsterSpawnDelay());
            a++;
        }
    }

    private IEnumerator MonsterSpawnDelay()
    {
        yield return new WaitForSeconds(4.0f);
        _spawned = false;
        BattleStateManager.Instance.RerollEvent();
    }
}