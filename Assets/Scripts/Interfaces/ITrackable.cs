
using UnityEngine;

public interface ITrackable
{
    GameObject Object { get; }
    bool IsColliderEnable { get; }
}