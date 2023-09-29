﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public abstract class BasePanel : MonoBehaviour
{
    private bool showFade;
    private bool hideFade;
    private CanvasGroup canvasGroup;
    public float fadeSpeed = 3f;
    public UnityAction hideCallBack;


    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    protected virtual void Start()
    {
        Init();
    }

    protected abstract void Init();

    protected virtual void Update()
    {
        Fade();
    }

    public virtual void Show()
    {
        showFade = true;
        canvasGroup.alpha = 0;
    }

    public virtual void Hide(UnityAction callBack)
    {
        hideFade = true;
        canvasGroup.alpha = 1;
        hideCallBack += callBack; // 淡出成功的回调
    }

    private void Fade()
    {
        if (showFade)
        {
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                showFade = false;
                return;
            }

            // 淡入
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
        }

        if (hideFade)
        {
            // 淡出
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                hideFade = false;
                // 淡出成功执行回调
                hideCallBack?.Invoke();
                return;
            }

            // 淡入
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
        }
    }
}