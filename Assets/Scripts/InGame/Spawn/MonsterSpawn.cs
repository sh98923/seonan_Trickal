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

    private Dictionary<int, CharacterData> _monsterDatas = new Dictionary<int, CharacterData>();

    private readonly int _poolSize = 5;

    private int _aliveMonsterCount = 0;
    private int _startMonsterIndex;
    private int _waveStep = 0;

    private bool _spawned = false;

    private void Start()
    {
        _startMonsterIndex = SpawnManager.Instance.StartMonsterSpawnKey;
        LoadMonsterPool();
        LoadMonsterPos();
        BattleStateManager.Instance.RerollEvent();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnReroll += SpawnMonstersOnReroll;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= SpawnMonstersOnReroll;
    }

    private void SpawnMonstersOnReroll()
    {
        if (_spawned) return;

        int waveKey = GameManager.Instance.StageKey + _waveStep;
        StageData wave = StageManager.Instance.GetStageData(waveKey);

        if (_waveStep >= wave.MaxWave) return;

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
                SpawnPosData spawnData = SpawnManager.Instance.GetMonsterSpawnData(_startMonsterIndex + index);
                Vector3 newPos = new Vector3(origin.x * spawnData.Ratio.x, origin.y * spawnData.Ratio.y, 0.0f);

                positions[i] = Camera.main.ScreenToWorldPoint(newPos);
                index++;
            }
        }
    }

    private void LoadMonsterPool()
    {
        InitMonsterFullData();        // 데이터 로드
        CreateMonsterPools();         // 풀링 생성
        RegisterMonsterEvents();      // OnDie 이벤트 연결
    }

    private void InitMonsterFullData()
    {
        int waveCnt = GameManager.Instance.WaveCount;
        HashSet<int> stageMonsterKeys = WaveManager.Instance.GetStageMonster(waveCnt);

        foreach (int monsterKey in stageMonsterKeys)
        {
            MonsterData monsterData = MonsterManager.Instance.GetMonsterData(monsterKey);

            CharacterData fullData = new CharacterData(monsterData);
            _monsterDatas.Add(monsterKey, fullData);
        }
    }

    private void CreateMonsterPools()
    {
        foreach (CharacterData fullData in _monsterDatas.Values)
        {
            GameObject prefab = Resources.Load<GameObject>(fullData.PrefabPath);
            PoolingManager.Instance.Add(fullData.EngName, _poolSize, prefab, transform);
        }
    }

    private void RegisterMonsterEvents()
    {
        foreach (CharacterData fullData in _monsterDatas.Values)
        {
            string key = fullData.EngName;
            List<GameObject> objects = PoolingManager.Instance.GetObjects(key);

            foreach (GameObject obj in objects)
            {
                Monster monster = obj.GetComponent<Monster>();
                monster.SetCharacterData(fullData);
                monster.SetCharacterActionInit();
                monster.OnDie += OnMonsterDie;
            }
        }
    }

    private void SpawnWaveMonsters(Dictionary<int, WaveData> waveMonsters, int wave)
    {
        _aliveMonsterCount = 0;

        foreach (WaveData data in waveMonsters.Values)
        {
            CharacterData fullData = MonsterManager.Instance.GetMonsterFullData(data.MonsterKey);

            Vector3[] linePositions = _formationPositions[(FormationLayer)data.SpawnLine];

            for (int i = 0; i < data.Count && i < linePositions.Length; i++)
            {
                GameObject monsterObj = PoolingManager.Instance.Pop(fullData.EngName);
                monsterObj.transform.position = linePositions[i];
                Vector3 finalPos = monsterObj.transform.position;
                finalPos.z = 0.0f;
                monsterObj.transform.localPosition = finalPos;

                Monster monster = monsterObj.GetComponent<Monster>();
                monster.SetCharacterData(fullData);
                monster.WaveUpgrade(wave);

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
            //BattleStateManager.Instance.SetState(BattleState.MonstersDefeated);

            _waveStep++;
            _spawned = false;
            BattleStateManager.Instance.SetState(BattleState.Reroll);

            //StartCoroutine(MonsterSpawnDelay());
        }
    }

    private IEnumerator MonsterSpawnDelay()
    {
        BattleStateManager.Instance.SetState(BattleState.Reroll);

        //BattleStateManager.Instance.RerollEvent();
        yield return new WaitForSeconds(4.0f);
        _spawned = false;
    }
}