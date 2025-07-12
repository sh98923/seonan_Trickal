using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    private enum FormationLayer
    {
        Front, Middle, Back
    }

    private Dictionary<FormationLayer, GameObject[]> _formationSlots = new Dictionary<FormationLayer, GameObject[]>
    {
        { FormationLayer.Front, new GameObject[3] },
        { FormationLayer.Middle, new GameObject[3] },
        { FormationLayer.Back, new GameObject[3] }
    };


}
