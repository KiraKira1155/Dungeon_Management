using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseClickManager : Singleton<MouseClickManager>
{
    //RaycastAll�̈���
    private PointerEventData pointData;

    private void Awake()
    {
        Init();
        pointData = new PointerEventData(EventSystem.current);
    }

    public Image CheckCursorOnTheImage(Image[] images)
    {
        //RaycastAll�̌��ʊi�[�p�̃��X�g�쐬
        var RayResult = new List<RaycastResult>();

        //PointerEvenData�ɁA�}�E�X�̈ʒu���Z�b�g
        pointData.position = Input.mousePosition;
        //RayCast�i�X�N���[�����W�j
        EventSystem.current.RaycastAll(pointData, RayResult);

        foreach (RaycastResult result in RayResult)
        {
            // �N���b�N���ꂽ�I�u�W�F�N�g��Image�R���|�[�l���g�������Ă��邩�m�F
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
        //RaycastAll�̌��ʊi�[�p�̃��X�g�쐬
        var RayResult = new List<RaycastResult>();

        //PointerEvenData�ɁA�}�E�X�̈ʒu���Z�b�g
        pointData.position = Input.mousePosition;
        //RayCast�i�X�N���[�����W�j
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
