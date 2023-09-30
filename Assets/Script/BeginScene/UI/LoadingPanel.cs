using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public RectTransform imgLoading;
    public Text txProcess;
    public float time;

    private Coroutine loading;
    private AsyncOperation ao;

    public void UpdateLoadingImg(float process)
    {
        process = Mathf.Clamp(process, 0, 1f);
        imgLoading.sizeDelta = new Vector2(process * 1000, 50);
        txProcess.text = $"Loading...{(int)(process * 100)}%";

        if (process >= 1)
        {
            ao.allowSceneActivation = true;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (ao.progress < 0.9f)
        {
            time += Time.deltaTime * 0.25f;
            UpdateLoadingImg(time);
        }
        
    }

    protected override void Init()
    {
        loading = StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        ao = SceneManager.LoadSceneAsync("GameSceneCountrySide");
        // 关闭自动进入场景
        ao.allowSceneActivation = false;

        ao.completed += operation =>
        {
            UIManager.Instance.Hide<LoadingPanel>(false);
            UIManager.Instance.Show<GamePanel>();
        };

        while (!ao.isDone)
        {
            if (ao.progress >= 0.9f)
            {
                UpdateLoadingImg(1);
            }

            yield return null;
        }
    }
}