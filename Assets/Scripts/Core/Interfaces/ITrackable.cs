
using UnityEngine;

public interface ITrackable
{
    GameObject Object { get; }
    Character Self { get; }
    bool IsColliderEnable { get; }
}