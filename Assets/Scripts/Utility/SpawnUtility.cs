using UnityEngine;

public static class SpawnUtility
{
    public static GameObject SpawnEntity(string prefabPath, Vector3 screenRatio, string layerName, Transform parent = null)
    {
        Vector3 origin = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        GameObject entity = GameObject.Instantiate(prefab, parent);

        Vector3 newPos = new Vector3(origin.x * screenRatio.x, origin.y * screenRatio.y, 0.0f);
        entity.transform.position = Camera.main.ScreenToWorldPoint(newPos);
        entity.transform.position = new Vector3(entity.transform.position.x, entity.transform.position.y, 0.0f);

        entity.layer = LayerMask.NameToLayer(layerName);
        return entity;
    }
}