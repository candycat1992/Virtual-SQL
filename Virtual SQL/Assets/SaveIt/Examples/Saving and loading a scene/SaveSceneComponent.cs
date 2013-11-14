using UnityEngine;
using System.Collections;
using SaveIt;

public class SaveSceneComponent : MonoBehaviour
{
    public string FileName = "scene.dat";

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 100, 100, 30), "Save"))
        {
            SaveScene();
        }

        if (GUI.Button(new Rect(20, 140, 100, 30), "Load"))
        {
            LoadScene();
        }

        if (GUI.Button(new Rect(20, 180, 100, 30), "Restart Scene"))
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
    }

    private void LoadScene()
    {
        if (Application.isWebPlayer)
            Scene.LoadFromPlayerPrefs(FileName);
        else
            Scene.LoadFromFile(FileName);
    }

    private void SaveScene()
    {
        if (Application.isWebPlayer)
            Scene.SaveToPlayerPrefs(FileName);
        else
            Scene.SaveToFile(FileName);
    }
}
