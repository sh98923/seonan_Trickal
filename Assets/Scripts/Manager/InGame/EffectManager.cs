using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    private readonly string _key = "Effect";
    private readonly int _poolSize = 200;

    private void Awake()
    {
        GameObject prefab = Resources.Load<GameObject>("Effect/Effect");

        PoolingManager.Instance.Add(_key, _poolSize, prefab);
    }

    public void PlayEffect(Transform characterTransform, string clipName, float time)
    {
        GameObject obj = PoolingManager.Instance.Pop(_key);
        obj.GetComponent<Effect>().Play(characterTransform, clipName, time);
    }
}