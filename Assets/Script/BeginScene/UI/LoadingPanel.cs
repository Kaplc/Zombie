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
    
    private AsyncOperation ao;
    private float process;

    public void UpdateLoadingImg(float value)
    {
        process = Mathf.Clamp(value, 0, 1f);
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

        if (process<1)
        {
            time += Time.deltaTime * 0.3f;
            UpdateLoadingImg(time);
        }
    }

    protected override void Init()
    {
       StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        ao = SceneManager.LoadSceneAsync("GameSceneCountrySide" +DataManager.Instance.nowMapID);
        // 关闭自动进入场景
        ao.allowSceneActivation = false;

        ao.completed += operation =>
        {
            UIManager.Instance.Hide<LoadingPanel>(false);
            UIManager.Instance.Show<GamePanel>();
        };

        while (!ao.isDone)
        {
            yield return ao;
        }
        UpdateLoadingImg(1);
        ao.allowSceneActivation = true;
        
    }
}