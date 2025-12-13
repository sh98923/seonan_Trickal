using System.Collections.Generic;
using UnityEngine;

public class MonsterTargetSelector : TargetSelector
{
    public override Character GetTarget(Character self)
    {
        Character target = GetCurrentTarget();

        if (target != null)
        {
            return target;
        }

        List<Character> front = new List<Character>();
        List<Character> middle = new List<Character>();
        List<Character> back = new List<Character>();

        for (int i = 0; i < _characters.Count; i++)
        {
            if (!IsTargetable(_characters[i], self.tag))
            {
                continue;
            }

            Character character = _characters[i].Self;

            if (character == null)
            { 
                continue; 
            }

            BattleLine battleline = (BattleLine)character.Data.SpawnLine;

            switch(battleline)
            {
                case BattleLine.Front:
                    front.Add(character);
                    break;
                case BattleLine.Middle:
                    middle.Add(character);
                    break;
                case BattleLine.Back:
                    back.Add(character);
                    break;
            }
        }

        if (front.Count > 0)
        {
            target = front[Random.Range(0, front.Count)];
        }
        else if (middle.Count > 0)
        {
            target = middle[Random.Range(0, middle.Count)];
        }
        else if (back.Count > 0)
        {
            target = back[Random.Range(0, back.Count)];
        }

        _currentTarget = target;

        return target;
    }
}
