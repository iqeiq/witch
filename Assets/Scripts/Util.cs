using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;

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

    public static IObservable<Type> ToObservable<Type>(this MonoBehaviour self, Func<Type> pred)
    {
        return self.UpdateAsObservable().Select(_ => pred());
    }

    public static IEnumerator FrameTimer(float ms, Action<float> updater, Action oncomp = null)
    {
        var prev = DateTime.UtcNow;
        var total = 0f;
        while (total < ms)
        {
            updater(total / ms);
            yield return new WaitForEndOfFrame();
            var now = DateTime.UtcNow;
            total += (float)(now - prev).TotalMilliseconds;
            prev = now;
        }
        if (oncomp != null) oncomp();
    }

    public static void FadeIn(this MonoBehaviour self, float ms, Action oncomp = null)
    {
        var fi = self.GetComponentInChildren<FadeImage>();

        self.StartCoroutine(FrameTimer(ms, (t) => {
            fi.Range = 1f - t;
        }, () => {
            fi.Range = 0f;
            if (oncomp != null) oncomp();
        }));
    }

    public static void FadeOut(this MonoBehaviour self, float ms, Action oncomp = null)
    {
        var fi = self.GetComponentInChildren<FadeImage>();

        self.StartCoroutine(FrameTimer(ms, (t) => {
            fi.Range = t;
        }, () => {
            fi.Range = 1f;
            if (oncomp != null) oncomp();
        }));
    }
}

