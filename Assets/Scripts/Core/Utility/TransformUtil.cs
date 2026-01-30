using UnityEngine;

public static class TransformUtil
{
    public static Transform FindDirectChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
        }

        return null;
    }

    public static T FindDirectChildComponent<T>(
    this Transform parent, string childName) where T : Component
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child.GetComponent<T>();
        }
        return null;
    }

    // GetcomponentInChildren 이랑 같은거
    public static T FindChildComponentRecursive<T>(
    this Transform parent, string childName) where T : Component
    {
        foreach (Transform child in parent)
        {
            // 1. 지금 자식에서 찾기
            if (child.name == childName)
                return child.GetComponent<T>();

            // 2. 자식의 자식으로 내려가기
            T found = child.FindChildComponentRecursive<T>(childName);
            if (found != null)
                return found;
        }

        return null;
    }
}