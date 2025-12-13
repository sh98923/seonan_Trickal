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
            if (!IsTargetable(_characters[i], self.tag))
            {
                continue;
            }

            float dist = Vector2.Distance(self.transform.position, _characters[i].Object.transform.position);

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