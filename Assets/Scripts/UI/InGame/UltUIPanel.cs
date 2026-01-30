using System.Collections.Generic;
using UnityEngine;

public class UltUIPanel : MonoBehaviour
{
    private List<UltPanel> _ultPanels = new List<UltPanel>();

    private const float _activeY = 0.0f;
    private const float _inactiveY = -100.0f;
    private const int _contentRootIndex = 0;

    private int _lastActivePlayerCount = 0;

    private void Start()
    {
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

        InGameEventManager.OnUnitActivated += HandleUnitActivated; 
        InGameEventManager.OnUnitUpdated += HandleUnitUpdated;

        BattleStateManager.Instance.OnBattle += ResumeAllUltCooldowns;
        BattleStateManager.Instance.OnEnteringReroll += PauseAllUltCooldowns;
    }

    private void OnDisable()
    {
        InGameEventManager.OnUnitActivated -= HandleUnitActivated;
        InGameEventManager.OnUnitUpdated -= HandleUnitUpdated;

        BattleStateManager.Instance.OnBattle -= ResumeAllUltCooldowns;
        BattleStateManager.Instance.OnEnteringReroll -= PauseAllUltCooldowns;
    }

    private void HandleUnitActivated(Player player)
    {
        int index = _lastActivePlayerCount;

        UltPanel panel = _ultPanels[index];
        panel.gameObject.SetActive(true);
        panel.BindPlayer(player);

        ApplyUltUnlock(panel);

        _lastActivePlayerCount++;
    }

    private void HandleUnitUpdated(int playerKey)
    {
        foreach (UltPanel panel in _ultPanels)
        {
            // 아직 플레이어 바인딩 안 된 패널 스킵
            if (panel == null)
                continue;

            // 여기서 바로 비교 가능
            if (panel.BoundPlayerKey != playerKey)
                continue;

            // 이 패널만 갱신
            ApplyUltUnlock(panel);
            break;
        }
    }

    private void ApplyUltUnlock(UltPanel panel)
    {
        bool unlocked = BattleUnitManager.Instance.IsUltimateUnlocked(panel.BoundPlayerKey);
        SetUltContentY(panel.transform, unlocked ? _activeY : _inactiveY);
    }

    private void SetUltContentY(Transform ult, float y)
    {
        RectTransform rt = ult.FindDirectChildComponent<RectTransform>("Conent");

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