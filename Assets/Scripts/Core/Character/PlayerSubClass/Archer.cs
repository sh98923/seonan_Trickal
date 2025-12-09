using UnityEngine;

public class Archer : Player
{
    private void Awake()
    {
        base.Awake();

        _targets[(int)ActionSlot.Attack] = GetComponent<NearestTargetSelector>();
        _targets[(int)ActionSlot.Skill] = GetComponent<FarthestTargetSelector>();
        _targets[(int)ActionSlot.Ult] = GetComponent<FarthestTargetSelector>();
    }
}
