using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : Singleton<FadeManager>
{
    [SerializeField] private Image fadeImage;
    private bool isFadeIn;
    private bool isFadeOut;

    private float alpha;
    [SerializeField] float fadeSpeed = 0.2f;

    public bool isFade { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        if (GameManager.I.test)
        {
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else
        {
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    void Update()
    {
        if (isFadeIn)
        {
            alpha -= Time.fixedDeltaTime / fadeSpeed;
            if (alpha <= 0.0f)//透明になったら、フェードインを終了
            {
                isFadeIn = false;
                alpha = 0.0f;
                isFade = false;
            }
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
        else if (isFadeOut)
        {
            alpha += Time.fixedDeltaTime / fadeSpeed;
            if (alpha >= 1.0f)//真っ黒になったら、フェードアウトを終了
            {
                isFadeOut = false;
                alpha = 1.0f;
                isFade = false;
            }
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
    }

    public void fadeNone()
    {
        fadeImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void fadeIn()
    {
        fadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        isFade = true;

        isFadeIn = true;
        isFadeOut = false;
    }

    public void fadeOut()
    {
        fadeImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        isFade = true;

        isFadeOut = true;
        isFadeIn = false;
    }

    public float GetFadeTime()
    {
        return fadeSpeed;
    }
}
