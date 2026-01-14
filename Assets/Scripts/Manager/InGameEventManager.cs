using System;
/*public enum UnitChangeType
{
    Deploy,
    Upgrade
}
public struct UnitChangeInfo
{
    public int unitKey;
    public UnitChangeType changeType;
}*/

public static class InGameEventManager
{
    public static Action<Player> OnUnitActivated;
    public static Action<int> OnUnitUpdated;
    public static Action OnUltUIRefreshRequested;
}