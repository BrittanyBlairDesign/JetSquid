


namespace Engine.System;
public class SaveFile
{
    public double score { get; set; }
    public double HighScore { get; set; }

    public void SetScores(double newScore)
    {
        if (newScore > HighScore)
        {
            HighScore = newScore;
            score = newScore;
        }
        else
        {
            score = newScore;
        }
    }

}
