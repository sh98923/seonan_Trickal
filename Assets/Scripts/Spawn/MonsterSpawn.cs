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

    private readonly int _poolSize = 3;

    private int _startMonsterIndex;
    private int a = 0;

    private bool _spawned = false;

    private void Start()
    {
        _startMonsterIndex = SpawnManager.Instance.StartMonsterSpawnKey;
        LoadMonsterPool();
        LoadMonsterPos();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            _spawned = false;
            BattleStateManager.Instance.SetState(BattleState.Battle);
            PoolingManager.Instance.InActiveAll();
            a++;
        }

        if (!_spawned && BattleStateManager.Instance.IsReroll)
        {
            int waveKey = StageManager.Instance.StageStartKey + a;
            StageData wave = StageManager.Instance.GetStageData(waveKey);
            Dictionary<int, WaveData> waveMonsters = WaveManager.Instance.GetWaveMonster(waveKey);

            SpawnWaveMonsters(waveMonsters, wave.Wave);

            _spawned = true;
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            BattleStateManager.Instance.SetState(BattleState.Battle);
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

    private void LoadMonsterPool()
    {
        //몬스터 종류마다 한 번씩만 등록
        foreach (MonsterData monsterData in CharacterManager.Instance.AllMonsterDatas)
        {
            GameObject prefab = Resources.Load<GameObject>(monsterData.PrefabPath);
            PoolingManager.Instance.Add(monsterData.Name, _poolSize, prefab, transform);
        }
    }

    private void SpawnWaveMonsters(Dictionary<int, WaveData> waveMonsters, int wave)
    {
        foreach (WaveData data in waveMonsters.Values)
        {
            Vector3[] linePositions = _formationPositions[(FormationLayer)data.SpawnLine];
            MonsterData monsterData = CharacterManager.Instance.GetMonsterData(data.MonsterKey);

            for (int i = 0; i < data.Count && i < linePositions.Length; i++)
            {
                GameObject monsterObj = PoolingManager.Instance.Pop(monsterData.Name);
                monsterObj.GetComponent<Monster>().SetCharacterStat(monsterData, wave);

                monsterObj.transform.position = linePositions[i];
                Vector3 finalPos = monsterObj.transform.position;
                finalPos.z = 0.0f;
                monsterObj.transform.localPosition = finalPos;
            }
        }
    }
}