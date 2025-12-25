
using UnityEngine;

public interface ITrackable : ICharacterObject
{
    GameObject Object { get; }
    bool IsColliderEnable { get; }
}