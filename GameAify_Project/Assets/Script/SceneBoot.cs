// Assets/Scripts/Utils/SceneBoot.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneBoot
{
    // ������ ���� "ó�� ����ó��" �ε�
    public static void LoadFresh(string sceneName)
    {
        Time.timeScale = 1f;
        PurgeDontDestroyOnLoad();               // �� DDOL ����
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // ���� ���� ���� ���ΰ�ħ(�̸�����)
    public static void ReloadFresh()
    {
        Time.timeScale = 1f;
        PurgeDontDestroyOnLoad();
        var active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.name, LoadSceneMode.Single);
    }

    // DontDestroyOnLoad ������ ��Ʈ ������Ʈ ��ü ����
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
