using System.Collections.Generic;
using UnityEngine;

public class UltUIPanel : MonoBehaviour
{
    private Transform _spawnPlayerTransform;
    private InGamePlayerSpawn _spawnParent; 
    private List<UltPanel> _ultPanels = new List<UltPanel>();

    private const float _activeY = 0.0f;
    private const float _inactiveY = -100.0f;
    private const int _contentRootIndex = 0;

    private int _lastActivePlayerCount = 0;

    private void Start()
    {
        _spawnPlayerTransform = GetComponentInParent<InGameUIPanel>().SpawnParent;
        _spawnParent = _spawnPlayerTransform.GetComponent<InGamePlayerSpawn>();

        GameObject ultPrefab = Resources.Load<GameObject>("Prefabs/UI/UltPanel");

        int maxPlayerCount = InGameManager.Instance.Players.Count;
        // 나중에 룰렛 들어오면 배치가 되어 있을 애들이 있으니
        // 걔들은 바로 활성화 해야함
        for (int i = 0; i < maxPlayerCount; i++)
        {
            GameObject ultObj = Instantiate(ultPrefab, transform);
            ultObj.SetActive(false);
            SetUltContentY(ultObj.transform, _inactiveY);

            UltPanel panel = ultObj.GetComponent<UltPanel>();
            _ultPanels.Add(panel);
        }

        RerollUIController.OnCardAction += SetUltUIPos;
        BattleStateManager.Instance.OnBattle += ResumeAllUltCooldowns;
        //BattleStateManager.Instance.OnEnteringBattle += SetUltUIPos; 
        BattleStateManager.Instance.OnEnteringReroll += PauseAllUltCooldowns;
    }

    private void OnDisable()
    {
        RerollUIController.OnCardAction -= SetUltUIPos;
        BattleStateManager.Instance.OnBattle -= ResumeAllUltCooldowns;
        //BattleStateManager.Instance.OnEnteringBattle -= SetUltUIPos;
        BattleStateManager.Instance.OnEnteringReroll -= PauseAllUltCooldowns;
    }

    private void SetUltUIPos()
    {
        int activeCount = _spawnParent.GetActivePlayerCount();

        ActivateNewUltObjects(activeCount);
        ApplyUltUnlockStates(activeCount);

        _lastActivePlayerCount = activeCount;
    }

    private void ActivateNewUltObjects(int activeCount)
    {
        for (int i = _lastActivePlayerCount; i < activeCount; i++)
        {
            _ultPanels[i].gameObject.SetActive(true);
        }
    }

    private void ApplyUltUnlockStates(int activeCount)
    {
        List<Player> activePlayers = _spawnParent.GetActivePlayers();

        for (int i = 0; i < activeCount; i++)
        {
            Player player = activePlayers[i];
            GameObject ultObj = _ultPanels[i].gameObject;

            UltPanel ultPanel = ultObj.GetComponent<UltPanel>();
            ultPanel.BindPlayer(player);

            int key = player.Data.PlayerKey;

            if (BattleUnitManager.Instance.IsUltimateUnlocked(key))
            {
                SetUltContentY(ultObj.transform, _activeY);
            }
            else
            {
                SetUltContentY(ultObj.transform, _inactiveY);
            }
        }
    }

    private void SetUltContentY(Transform ult, float y)
    {
        Transform content = ult.GetChild(_contentRootIndex);
        RectTransform rt = content.GetComponent<RectTransform>();

        Vector2 pos = rt.anchoredPosition;
        pos.y = y;
        rt.anchoredPosition = pos;
    }

    private void PauseAllUltCooldowns()
    {
        foreach (UltPanel ult in _ultPanels)
        { 
            ult.PauseCooldown();
        }
    }

    private void ResumeAllUltCooldowns()
    {
        foreach (UltPanel ult in _ultPanels)
        {
            ult.ResumeCooldown();
        }
    }
}