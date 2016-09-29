using UnityEngine;
using System.Collections;

/// <summary>
/// リングのGameObject用スクリプト
/// </summary>
[ExecuteInEditMode()]
public class RingObject : MonoBehaviour
{

    /// <summary>
    /// レンダラ
    /// </summary>
    RingRenderer ringRenderer;

    /// <summary>
    /// 内円の割合
    /// </summary>
    [Range(0, 1)]
    public float innerPercentage = 0.8f;

    /// <summary>
    /// 扇型の角度
    /// </summary>
    [Range(0, Mathf.PI * 2 + 0.01f)]
    public float fanAngle = 1.5f;

    /// <summary>
    /// 色
    /// </summary>
    public Color color = Color.black;

    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        // レンダラを探す
        ringRenderer = GameObject.Find("RingRenderer").GetComponent<RingRenderer>();
    }

    /// <summary>
    /// 更新
    /// </summary>
    void LateUpdate()
    {
        PushToRenderer();
    }

    /// <summary>
    /// レンダラに追加する
    /// </summary>
    void PushToRenderer()
    {
        var ring = new Ring();

        ring.pos = transform.position;
        ring.rotate = transform.rotation;
        ring.scale = transform.localScale;
        ring.innerPercentage = innerPercentage;
        ring.fanAngle = fanAngle;
        ring.color = color;

        ringRenderer.Push(ring);
    }

}
