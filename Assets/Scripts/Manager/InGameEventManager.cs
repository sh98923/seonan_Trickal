using System;
using UnityEngine;

public static class InGameEventManager
{
    private static event Action _onUltUIRefreshRequested;
    public static event Action OnUltUIRefreshRequested
    {
        add { _onUltUIRefreshRequested += value; }
        remove { _onUltUIRefreshRequested -= value; }
    }
    private static event Action<int> _onUnitUpdated;
    public static event Action<int> OnUnitUpdated
    {
        add { _onUnitUpdated += value; }
        remove { _onUnitUpdated -= value; }
    }
    private static event Action<Player> _onUnitActivated;
    public static event Action<Player> OnUnitActivated
    {
        add { _onUnitActivated += value; }
        remove { _onUnitActivated -= value; }
    }

    public static void TriggerUltUIRefresh()
    { 
        _onUltUIRefreshRequested?.Invoke();
    }

    public static void TriggerUnitUpdated(int unitKey)
    {
        _onUnitUpdated?.Invoke(unitKey);
    }

    public static void TriggerUnitActivated(Player player)
    {
        _onUnitActivated?.Invoke(player);}
}