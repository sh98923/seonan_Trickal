using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
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

    private Dictionary<int, CharacterFullData> _monsterFullDatas = new Dictionary<int, CharacterFullData>();

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

        int waveKey = GameManager.Instance.WaveKey + _waveStep;
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
            CharacterData characterData = CharacterManager.Instance.GetCharacterData(monsterData.CharacterKey);

            CharacterFullData fullData = new CharacterFullData(characterData, monsterData);
            _monsterFullDatas.Add(monsterKey, fullData);
        }
    }

    private void CreateMonsterPools()
    {
        foreach (CharacterFullData fullData in _monsterFullDatas.Values)
        {
            GameObject prefab = Resources.Load<GameObject>(fullData.PrefabPath);
            PoolingManager.Instance.Add(fullData.EngName, _poolSize, prefab, transform);
        }
    }

    private void RegisterMonsterEvents()
    {
        foreach (CharacterFullData fullData in _monsterFullDatas.Values)
        {
            string key = fullData.EngName;
            List<GameObject> objects = PoolingManager.Instance.GetObjects(key);

            foreach (GameObject obj in objects)
            {
                Monster monster = obj.GetComponent<Monster>();
                monster.OnDie += OnMonsterDie;
            }
        }
    }

    private void SpawnWaveMonsters(Dictionary<int, WaveData> waveMonsters, int wave)
    {
        _aliveMonsterCount = 0;

        foreach (WaveData data in waveMonsters.Values)
        {
            CharacterFullData fullData = MonsterManager.Instance.GetMonsterFullData(data.MonsterKey);

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
            BattleStateManager.Instance.SetState(BattleState.MonstersDefeated);
            StartCoroutine(MonsterSpawnDelay());
            _waveStep++;
        }
    }

    private IEnumerator MonsterSpawnDelay()
    {
        yield return new WaitForSeconds(4.0f);
        _spawned = false;
        BattleStateManager.Instance.RerollEvent();
    }
}