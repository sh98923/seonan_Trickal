using UnityEngine;
using System.Collections.Generic;

public class NearestTargetSelector : TargetSelector
{
    private void Awake()
    {
        _characters = InGameManager.Instance.Trackables;
    }

    public override Character GetTarget(Character self)
    {
        Character target = GetCurrentTarget();

        if (target != null)
        { 
            return target;
        }

        float nearestDist = float.MaxValue;

        for (int i = 0; i < _characters.Count; i++)
        {
            if (!_characters[i].IsColliderEnable || !_characters[i].Object.activeSelf)
                continue;

            // 탐색하는 캐릭터의 태그와 같으면 넘어감 tag : (Player, Monster)
            if (_characters[i].Object.tag == self.tag)
                continue;

            float dist = Vector2.Distance(self.transform.position, _characters[i].Object.transform.position);
            
            // 거리 5 이하인 경우만 고려
            if (dist > _findTargetRange)
                continue;

            if (dist < nearestDist)
            {
                nearestDist = dist;
                target = _characters[i].Object.GetComponent<Character>();
            }
        }

        _currentTarget = target;

        return target;
    }
}