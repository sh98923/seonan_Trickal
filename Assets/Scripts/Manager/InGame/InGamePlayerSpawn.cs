using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePlayerSpawn : MonoBehaviour
{
    private List<PlayerData> _deployableDatas = new List<PlayerData>();
    public List<PlayerData> DeployableData
    {
        get { return _deployableDatas; }
    }

    private List<Player> _registeredPlayers = new List<Player>();

    private List<Player> _activePlayers = new List<Player>();

    private int _nextWaveReadyCount = 0;
    private int _totalCharacters = 0;

    private void Awake()
    {
        LoadPlayerPos();

        // 룰렛 만들게되면 룰렛에 나온 애들만 스폰
        InGameManager.Instance.RegisterDeckUnits(transform);

        for(int i = 0; i < transform.childCount; i++)
        {
            Player player = transform.GetChild(i).GetComponent<Player>();
            _registeredPlayers.Add(player);
        }

        InGameCam.Instance.RegisterPlayerCharacter(_registeredPlayers);
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnReroll += RespawnActiveDeckPlayers;
        BattleStateManager.Instance.OnWaveAdvance += SetAlivePlayersCount;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= RespawnActiveDeckPlayers;
        BattleStateManager.Instance.OnWaveAdvance -= SetAlivePlayersCount;
    }

    private void OnDestroy()
    {
        if(BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnReroll -= RespawnActiveDeckPlayers;
            BattleStateManager.Instance.OnWaveAdvance -= SetAlivePlayersCount;
        }
    }

    private void LoadPlayerPos()
    {
        List<PlayerData> datas = GameManager.Instance.SpawnablePlayerDatas;

        for(int i = 0; i < datas.Count; i++)
        {
            _deployableDatas.Add(datas[i]);
        }
    }

    public void SetActivePlayer(GameObject playerCharacter)
    {
        Player player = playerCharacter.GetComponent<Player>();

        if(player != null)
        {
            _activePlayers.Add(player);
        }
        else
        {
            Debug.LogError("활성화된 플레이어 등록 실패");
        }
    }

    public List<Player> GetActivePlayers()
    {
        return _activePlayers;
    }

    private void RespawnActiveDeckPlayers()
    {
        float camPosX = InGameCam.Instance.transform.position.x;

        foreach (Player player in _activePlayers)
        {
            if (!player.gameObject.activeSelf)
            {
                player.gameObject.SetActive(true);
                player.ReviveAnim();
                player.Health.ReviveHp();
                player.RevivePlayer(camPosX);  // 상태 초기화

                /*// 콜라이더도 필요하면 켜주기
                Collider2D collider = player.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = true;
                }*/
            }
        }
    }

    private void SetAlivePlayersCount()
    {
        _totalCharacters = 0;

        foreach (Player player in _registeredPlayers)
        {
            player.ResetArrivalState();

            if (!player.gameObject.activeSelf)
            {
                continue;
            }

            // 이거 다시 봐야해 waveadvace때 충돌체 꺼버리는 바람에 이상해짐
            Collider2D collider = player.GetComponent<Collider2D>();
            if (collider.enabled)
            {
                _totalCharacters++;
            }
        }

        print("Total Alive : " + _totalCharacters);
        //StartCoroutine(CheckTotalAlivePlayer());
    }

    /*private IEnumerator CheckTotalAlivePlayer()
    {
        yield return new WaitForSeconds(1.5f);

        _totalCharacters = 0; 

        foreach (Player player in _registeredPlayers)
        {
            player.ResetArrivalState();

            if (!player.gameObject.activeSelf)
            { 
                continue;
            }

            // 이거 다시 봐야해 waveadvace때 충돌체 꺼버리는 바람에 이상해짐
            Collider2D collider = player.GetComponent<Collider2D>();
            if (collider.enabled)
            { 
                _totalCharacters++;
            }
        }

        print("Total Alive : " + _totalCharacters);
    }*/

    public void CheckNextWaveReady()
    {
        _nextWaveReadyCount++;

        // 모두 준비됐는지 검사
        if (_nextWaveReadyCount >= _totalCharacters)
        {
            print("살아있는 캐릭 : " + _nextWaveReadyCount);
            _nextWaveReadyCount = 0;
            BattleStateManager.Instance.SetState(BattleState.Reroll);
            Debug.Log("모든 캐릭터가 도착 → Reroll 상태 전환");
        }
    }
}