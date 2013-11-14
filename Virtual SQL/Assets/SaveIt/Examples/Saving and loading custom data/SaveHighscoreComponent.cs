using UnityEngine;
using System.Collections;
using SaveIt;

public class SaveHighscoreComponent : MonoBehaviour
{
    public string FileName = "highscore.dat";
    public PointsComponent PointsComponent;

    private string playerName = "Unnamed Player";

    void OnGUI()
    {
        GUI.Label(new Rect(20, 100, 80, 30), "Playername: ");
        playerName = GUI.TextField(new Rect(100, 100, 120, 30), playerName);

        if (GUI.Button(new Rect(20, 140, 200, 30), "Save Highscore"))
        {
            if (PointsComponent.Points == PointsComponent.Highscore)
                PointsComponent.HighscoreHolder = playerName;

            SaveHighscore();
        }

        if (GUI.Button(new Rect(20, 180, 200, 30), "New Game (Load)"))
        {
            LoadHighscore();
        }
    }

    private void LoadHighscore()
    {
        LoadContext context;
        if (Application.isWebPlayer)
            context = LoadContext.FromPlayerPrefs(FileName);
        else
            context = LoadContext.FromFile(FileName);

        PointsComponent.Points = 0;
        PointsComponent.Highscore = context.Load<int>("Highscore");
        PointsComponent.HighscoreHolder = context.Load<string>("Holder");
    }

    private void SaveHighscore()
    {
        SaveContext context;
        if (Application.isWebPlayer)
            context = SaveContext.ToPlayerPrefs(FileName);
        else
            context = SaveContext.ToFile(FileName);

        context.Save(PointsComponent.Highscore, "Highscore");
        context.Save(PointsComponent.HighscoreHolder, "Holder");
        context.Flush();
    }
}
