using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    public Text levelText;

    public Text highScoreText1;
    public Text highScoreText2;
    public Text highScoreText3;
    public Text highScoreText4;
    public Text highScoreText5;

    public Text lastScore;

    private void Start()
    {
        if (levelText != null)
        {
            levelText.text = "0";
        }
        //PlayerPrefs.SetInt("highscore1", 0);
        //PlayerPrefs.SetInt("highscore2", 0);
        //PlayerPrefs.SetInt("highscore3", 0);
        //PlayerPrefs.SetInt("highscore4", 0);
        //PlayerPrefs.SetInt("highscore5", 0);
        if (highScoreText1 != null)
        {
            highScoreText1.text = PlayerPrefs.GetInt("highscore1").ToString();
        }

        if (highScoreText2 != null)
        {
            highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();
        }

        if (highScoreText3 != null)
        {
            highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();
        }

        if (highScoreText4 != null)
        {
            highScoreText4.text = PlayerPrefs.GetInt("highscore4").ToString();
        }

        if (highScoreText5 != null)
        {
            highScoreText5.text = PlayerPrefs.GetInt("highscore5").ToString();
        }

        if (lastScore != null)
        {
            lastScore.text = PlayerPrefs.GetInt("lastScore").ToString();
        }

    }

    void Update()
    {      
        
    }

    public void ChangedValuie(float value)
    {
        GAME.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void RePlay()
    {
        SceneManager.LoadScene("Game");
    }
    public void PlayGame()
    {

        if (GAME.startingLevel == 0)
        {
            GAME.startingAtLevelZero = true;
        }
        else
        {
            GAME.startingAtLevelZero = false;
        }
        
        SceneManager.LoadScene("Game");
    }
    public void SceneMenu()
    {
        SceneManager.LoadScene("SceneMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
