using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTool
{
    public static ThreadPriority threadPriority = ThreadPriority.High;
    public static bool LoadingDone;
    private static AsyncOperation async;

    /// <summary>
    /// Carrega a cena de forma assíncrona em segundo plano.
    /// </summary>
    public static IEnumerator LoadSceneAsyncSwitch(string scene)
    {
        Time.timeScale = 1f;
        Application.backgroundLoadingPriority = threadPriority;

        async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
                break;
            }
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Carrega a cena de forma assíncrona em segundo plano.
    /// </summary>
    public static IEnumerator LoadSceneAsyncSwitch(int scene)
    {
        Time.timeScale = 1f;
        Application.backgroundLoadingPriority = threadPriority;

        async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
                break;
            }
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Carrega a cena de forma assíncrona em segundo plano sem ativação da cena quando o carregamento é concluído.
    /// </summary>
    public static IEnumerator LoadSceneAsync(string scene, LoadSceneMode loadSceneMode)
    {
        Time.timeScale = 1f;
        Application.backgroundLoadingPriority = threadPriority;

        async = SceneManager.LoadSceneAsync(scene, loadSceneMode);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                LoadingDone = true;
                break;
            }
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Carrega a cena de forma assíncrona em segundo plano sem ativação da cena quando o carregamento é concluído.
    /// </summary>
    public static IEnumerator LoadSceneAsync(int scene, LoadSceneMode loadSceneMode)
    {
        Time.timeScale = 1f;
        Application.backgroundLoadingPriority = threadPriority;

        async = SceneManager.LoadSceneAsync(scene, loadSceneMode);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                LoadingDone = true;
                break;
            }
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Ativa a cena carregada quando o carregamento é concluído.
    /// </summary>
    public static void AllowSceneActivation()
    {
        if (LoadingDone)
        {
            async.allowSceneActivation = true;
        }
    }

    public static string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }
}