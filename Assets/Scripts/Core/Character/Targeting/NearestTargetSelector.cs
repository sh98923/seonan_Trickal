using UnityEngine;

public class NearestTargetSelector : TargetSelector
{
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
            if (!IsTargetable(_characters[i], self.tag))
            { 
                continue; 
            }

            float dist = Vector2.Distance(self.transform.position, _characters[i].Object.transform.position);
            
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