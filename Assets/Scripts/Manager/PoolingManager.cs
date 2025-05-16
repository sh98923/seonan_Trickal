using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : SingletonScript<PoolingManager>
{
    private Dictionary<string, List<GameObject>> _totalObject = new Dictionary<string, List<GameObject>>();

    public List<GameObject> GetObjects(string key) { return _totalObject[key]; }

    public void Add(string key, int poolSize, GameObject prefab, Transform parent = null)
    {
        List<GameObject> objects = new List<GameObject>(poolSize);

        if (parent == null)
        {
            GameObject parentObject = new GameObject(key);
            parent = parentObject.transform;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.name = prefab.name;
            obj.SetActive(false);
            objects.Add(obj);
        }

        _totalObject.Add(key, objects);
    }

    public GameObject Pop(string key)
    {
        foreach (GameObject obj in _totalObject[key])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }
}