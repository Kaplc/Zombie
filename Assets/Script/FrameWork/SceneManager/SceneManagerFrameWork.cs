using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManagerFrameWork : BaseSingleton<SceneManagerFrameWork>
{
    public void LoadScene(string sceneName, UnityAction callBack = null)
    {
        SceneManager.LoadScene(sceneName);
        callBack?.Invoke();
    }

    public void LoadSceneAsync(string sceneName, UnityAction callBack = null)
    {
        MonoManager.Instance.StartCoroutineFrameWork(LoadSceneAsyncCoroutine(sceneName, callBack));

    }

    public IEnumerator LoadSceneAsyncCoroutine(string sceneName, UnityAction callBack)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone)
        {
            EventCenter.Instance.TriggerEvent<float>("进度条更新", ao.progress);
            yield return ao;
        }
        callBack?.Invoke();
    }
}
