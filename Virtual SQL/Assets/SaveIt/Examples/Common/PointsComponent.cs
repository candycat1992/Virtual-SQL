using UnityEngine;
using System.Collections;

public class PointsComponent : MonoBehaviour
{
    public int Points;
    public int Highscore;
    public string HighscoreHolder = string.Empty;
    public bool DisplayStats = true;

    public void AddPoints(int points)
    {
        Points += points;
        
        if (Points > Highscore)
        {
            Highscore = Points;
        }
    }

    void OnGUI()
    {
        if (!DisplayStats)
            return;

        GUI.Label(new Rect(20, 20, 200, 30), "Points: " + Points.ToString());
        if (!string.IsNullOrEmpty(HighscoreHolder))
            GUI.Label(new Rect(20, 50, 200, 30), "Highscore: " + Highscore.ToString() + " by " + HighscoreHolder);
    }
}
