using UnityEngine;

public static class Transforms
{
    public static void DestroyChildren(this Transform t, bool destroyImmediatly = false)
    {
        foreach (Transform item in t)
        {
            if (destroyImmediatly)
            {
                MonoBehaviour.DestroyImmediate(item.gameObject);

            }
            else
            {
                MonoBehaviour.Destroy(item.gameObject);
            }
        }
    }
}