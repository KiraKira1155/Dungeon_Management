using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image decisionButton;

    private void Start()
    {
        StartCoroutine(GameManager.I.UnLoadPreviousScene(false));
        inputField.text = "Player_" + Random.Range((int)1454, (int)2147483647);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (MouseClickManager.I.CheckCursorOnTheImage(decisionButton))
            {
                StartCoroutine(GetID());
                GameManager.I.ChengeNewScene(GameSystem.SceneController.GameScene.title, false);
            }
        }
    }

    private IEnumerator GetID()
    {
        GameManager.I.signupHandler.SetData(0, text.text);
        var task = GameManager.I.signupHandler.PostCoroutine();
        yield return task;

        var data = new SaveData()
        {
            id = GameManager.I.signupHandler.GetID(),
        };

        SaveDataManager.Save(data);
    }


    public void InputText()
    {
        text.text = inputField.text;
    }


}
