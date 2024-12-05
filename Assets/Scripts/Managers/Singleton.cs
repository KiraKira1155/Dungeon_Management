using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected virtual bool DestroyTargetGameObj => false;
    
    /// <summary>
    /// これを呼ぶことでクラスが取得できる。
    /// </summary>
    public static T I { get; private set; } = null;

    /// <summary>
    /// 初期化が完了しているかの確認用。
    /// </summary>
    /// <returns>初期化ができている場合はTrueが返る。</returns>
    public static bool IsValid() => I != null;

    private static GameObject thisObj = null;

    public virtual void BattleSceneInit() { }
    public virtual void BattleSceneClear() { }

    /// <summary>
    /// Awakeで呼び出す。
    /// 初めて呼ばれるとIのなかに呼んだクラスがはいる。
    /// </summary>
    protected void Init()
    {
        if (I == null)
        {
            I = this as T;
            thisObj = I.gameObject;
            return;
        }
        if(gameObject == thisObj)
        {
            return;
        }
        if (DestroyTargetGameObj)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (I == this)
        {
            I = null;
        }
        OnRelease();
    }

    /// <summary>
    /// 継承クラス用のDestroy関数
    /// </summary>
    protected virtual void OnRelease()
    {

    }
}