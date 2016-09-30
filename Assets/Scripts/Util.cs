using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

//namespace Util
//{

    public static class Util
    {
        public static Type CreateAndGetComponent<Type>(GameObject prefab, Transform parent = null)
        {
            var g = MonoBehaviour.Instantiate(prefab) as GameObject;
            return __CreateAndGetComponent<Type>(g, parent);
        }

        public static Type CreateAndGetComponent<Type>(GameObject prefab, Vector2 pos, Transform parent = null)
        {
            var g = MonoBehaviour.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
            return __CreateAndGetComponent<Type>(g, parent);
        }

        static Type __CreateAndGetComponent<Type>(GameObject g, Transform parent = null)
        {
            if (parent != null)
            {
                g.transform.parent = parent;
                g.transform.position = parent.position;
            }
            return g.GetComponent<Type>();
        }

        public static IObservable<Unit> InputAsObservable(this MonoBehaviour self, string name)
        {
            return self.UpdateAsObservable().Where(_ => Input.GetButtonDown(name));
        }

        public static IObservable<Unit> InputAsObservable(this MonoBehaviour self, Func<bool> pred)
        {
            return self.UpdateAsObservable().Where(_ => pred());
        }
    }

//}
