// http://notargs.com/blog/blog/2015/02/03/unity%E3%81%8B%E3%81%A3%E3%81%93%E3%82%88%E3%81%8F%E6%B2%A2%E5%B1%B1%E3%81%AE%E5%86%86%E3%82%92%E8%A1%A8%E7%A4%BA%E3%81%99%E3%82%8B%E3%81%9F%E3%82%81%E3%81%AE%E3%82%B7%E3%82%A7%E3%83%BC%E3%83%80/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// リングの構造体
/// </summary>
public struct Ring
{
    /// <summary>
    /// 位置
    /// </summary>
    public Vector3 pos;

    /// <summary>
    /// 回転角
    /// </summary>
    public Quaternion rotate;

    /// <summary>
    /// スケール
    /// </summary>
    public Vector3 scale;

    /// <summary>
    /// 内円の割合
    /// </summary>
    public float innerPercentage;

    /// <summary>
    /// 扇型の角度
    /// </summary>
    public float fanAngle;

    /// <summary>
    /// 色
    /// </summary>
    public Color color;
}

/// <summary>
/// リングのレンダラ
/// エディットモードでも実行する
/// </summary>
[ExecuteInEditMode()]
public class RingRenderer : MonoBehaviour
{

    /// <summary>
    /// シェーダ
    /// </summary>
    public Shader shader;

    /// <summary>
    /// マテリアル
    /// </summary>
    Material material;

    /// <summary>
    /// リングのデータを渡すための演算バッファ
    /// </summary>
    ComputeBuffer buffer;

    /// <summary>
    /// リングのリスト
    /// </summary>
    List<Ring> rings;

    /// <summary>
    /// 初期化
    /// </summary>
    void Awake()
    {
        if (material == null)
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
        }
        if (buffer == null)
            buffer = new ComputeBuffer(10000, Marshal.SizeOf(typeof(Ring)));
        if (rings == null)
            rings = new List<Ring>();

    }

    /// <summary>
    /// 破棄
    /// </summary>
    void OnDisable()
    {
        if (buffer != null)
            buffer.Dispose();
        buffer = null;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        Awake();
        rings.Clear();
    }

    /// <summary>
    /// レンダリング
    /// </summary>
    void OnRenderObject()
    {

        Awake();
        // レンダリングを開始
        material.SetPass(0);

        material.SetBuffer("Rings", buffer);

        buffer.SetData(rings.ToArray());
        Graphics.DrawProcedural(MeshTopology.Points, rings.Count);
    }

    /// <summary>
    /// 現在のタイムステップにおけるリングを追加する
    /// </summary>
    /// <param name="ring">Ring.</param>
    public void Push(Ring ring)
    {
        rings.Add(ring);
    }

}
