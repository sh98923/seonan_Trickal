using UnityEngine;

public class FarthestTargetSelector : TargetSelector
{
    public override Character GetTarget(Character self)
    {
        Character target = GetCurrentTarget();

        if (target != null)
        {
            return target;
        }

        float farthestDist = float.MinValue;

        for (int i = 0; i < _characters.Count; i++)
        {
            if (!_characters[i].IsColliderEnable || !_characters[i].Object.activeSelf)
                continue;

            // 탐색하는 캐릭터의 태그와 같으면 넘어감 tag : (Player, Monster)
            if (_characters[i].Object.tag == self.tag)
                continue;

            float dist = Vector2.Distance(self.transform.position, _characters[i].Object.transform.position);

            /*// 거리 8 이하인 경우만 고려
            if (dist > _findTargetRange)
                continue;*/

            if (dist > farthestDist)
            {
                farthestDist = dist;
                target = _characters[i].Object.GetComponent<Character>();
            }
        }

        _currentTarget = target;

        return target;
    }
}