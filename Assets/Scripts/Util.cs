using UnityEngine;


public class Util : MonoBehaviour
{
    public static Type CreateAndGetComponent<Type>(GameObject prefab, Transform parent = null)
    {
        var g = Instantiate(prefab) as GameObject;
        return __CreateAndGetComponent<Type>(g, parent);
    }

    public static Type CreateAndGetComponent<Type>(GameObject prefab, Vector2 pos, Transform parent = null)
    {
        var g = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
        return __CreateAndGetComponent<Type>(g, parent);
    }

    static Type __CreateAndGetComponent<Type>(GameObject g, Transform parent = null)
    {
        if (parent != null) g.transform.parent = parent;
        return g.GetComponent<Type>();
    }

}
