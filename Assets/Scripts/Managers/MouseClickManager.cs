using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseClickManager : Singleton<MouseClickManager>
{
    //RaycastAllの引数
    private PointerEventData pointData;

    private void Awake()
    {
        Init();
        pointData = new PointerEventData(EventSystem.current);
    }

    public Image CheckCursorOnTheImage(Image[] images)
    {
        //RaycastAllの結果格納用のリスト作成
        var RayResult = new List<RaycastResult>();

        //PointerEvenDataに、マウスの位置をセット
        pointData.position = Input.mousePosition;
        //RayCast（スクリーン座標）
        EventSystem.current.RaycastAll(pointData, RayResult);

        foreach (RaycastResult result in RayResult)
        {
            // クリックされたオブジェクトがImageコンポーネントを持っているか確認
            foreach (var clickImage in images)
            {
                if (clickImage == result.gameObject.GetComponent<Image>())
                {
                   
                    return clickImage;
                }
            }
        }

        return null;
    }
    public Image CheckCursorOnTheImage(Image image)
    {
        //RaycastAllの結果格納用のリスト作成
        var RayResult = new List<RaycastResult>();

        //PointerEvenDataに、マウスの位置をセット
        pointData.position = Input.mousePosition;
        //RayCast（スクリーン座標）
        EventSystem.current.RaycastAll(pointData, RayResult);

        foreach (RaycastResult result in RayResult)
        {
            if (image == result.gameObject.GetComponent<Image>())
            {
                return image;
            }
        }

        return null;
    }

    public bool PauseImageClick(Image target)
    {
        if (MouseClickManager.I.CheckCursorOnTheImage(target))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Sound.SoundManager.I.PlayButtonSE(SoundManager.buttonAudioID.pauseButton);

                return true;
            }
        }

        return false;
    }

    public bool ResultImageClick(Image target)
    {
        if (MouseClickManager.I.CheckCursorOnTheImage(target))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Sound.SoundManager.I.PlayButtonSE(SoundManager.buttonAudioID.resultButton);

                return true;
            }
        }

        return false;
    }


}
