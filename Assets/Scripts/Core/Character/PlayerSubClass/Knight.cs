using UnityEngine;

public class Knight : Player
{
    private void Awake()
    {
        base.Awake();

        _targets[(int)ActionSlot.Attack] = GetComponent<NearestTargetSelector>();
        _targets[(int)ActionSlot.Skill] = GetComponent<NearestTargetSelector>();
        _targets[(int)ActionSlot.Ult] = GetComponent<NearestTargetSelector>();
    }
}