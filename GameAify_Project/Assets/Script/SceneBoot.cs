// Assets/Scripts/Utils/SceneBoot.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneBoot
{
    // 지정한 씬을 "처음 부팅처럼" 로드
    public static void LoadFresh(string sceneName)
    {
        Time.timeScale = 1f;
        PurgeDontDestroyOnLoad();               // ★ DDOL 비우기
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // 현재 씬을 완전 새로고침(이름으로)
    public static void ReloadFresh()
    {
        Time.timeScale = 1f;
        PurgeDontDestroyOnLoad();
        var active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.name, LoadSceneMode.Single);
    }

    // DontDestroyOnLoad 영역의 루트 오브젝트 전체 제거
    static void PurgeDontDestroyOnLoad()
    {
        var probe = new GameObject("~DDOL_Probe");
        Object.DontDestroyOnLoad(probe);
        var ddolScene = probe.scene;

        foreach (var go in ddolScene.GetRootGameObjects())
            if (go != probe) Object.Destroy(go);

        Object.Destroy(probe);
    }
}
