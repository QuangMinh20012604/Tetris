using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GAME : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingAtLevelZero;
    public static int startingLevel;

    public int scoreOneLine = 100;
    public int scoreTwoLine = 225;
    public int scoreThreeLine = 350;
    public int scoreFourLine = 500;

    public int currentLevel = 0;
    public int numberLinesCleared = 0;

    public float fallSpeed = 1.0f;
    public static bool GameIsPause = false;

    public AudioClip ClearedLineSound;

    private AudioSource audioSource;
    public Text hud_score;
    public Text hud_level;
    public Text hud_line;

    

    public GameObject pauseMusic;
    public GameObject pauseMenuUI;
    public GameObject pauseScore;
    public GameObject pauseLevel;
    public GameObject pauseLine;
    public GameObject help;

    private int numberOfRowsThisTurn = 0;
    public int currentScore = 0;
    [SerializeField] private int currentLine = 0;

    private int startHighScore1;
    private int startHighScore2;
    private int startHighScore3;
    private int startHighScore4;
    private int startHighScore5;

    private GameObject previewTetromino;
    private GameObject nextTetromino;

    
    private bool gameStarted = false;

    private Vector2 previewTetrominoPosition = new Vector2(-8f, 17f);


    void Start()
    {
        GameIsPause = false;
        Time.timeScale = 1.0f;

        currentScore = 0;
        hud_score.text = "0";
        currentLevel = startingLevel;
        hud_level.text = currentLevel.ToString();
        hud_line.text = "0";

        SpawnNextTetromino();
        audioSource = GetComponent<AudioSource>();

        startHighScore1 = PlayerPrefs.GetInt("highscore1");
        startHighScore2 = PlayerPrefs.GetInt("highscore2");
        startHighScore3 = PlayerPrefs.GetInt("highscore3");
        startHighScore4 = PlayerPrefs.GetInt("highscore4");
        startHighScore5 = PlayerPrefs.GetInt("highscore5");
    }

    private void Update()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
        CheckUserInput();
    }

    void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPause)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        pauseMusic.SetActive(true);
        pauseScore.SetActive(true);
        pauseLevel.SetActive(true);
        pauseLine.SetActive(true);

        Time.timeScale = 1.0f;
        GameIsPause = false;
    }
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        pauseMusic.SetActive(false);
        pauseScore.SetActive(false);
        pauseLevel.SetActive(false);
        pauseLine.SetActive(false);

        Time.timeScale = 0f;
        GameIsPause = true;
    }

    void UpdateLevel()
    {
        if ((startingAtLevelZero == true) || (startingAtLevelZero == false && numberLinesCleared / 10 > startingLevel))
        {
            if (currentLevel < 9)
            {
                currentLevel = numberLinesCleared / 10;
            }
            if (currentLevel >= 9)
            {
                currentLevel = 9;
            }
        }

    }

    void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_line.text = currentLine.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                currentScore += scoreOneLine;
                numberLinesCleared++;
                currentLine += numberOfRowsThisTurn;
            }
            else if (numberOfRowsThisTurn == 2)
            {
                currentScore += scoreTwoLine;
                numberLinesCleared += 2;
                currentLine += numberOfRowsThisTurn;
            }
            else if (numberOfRowsThisTurn == 3)
            {
                currentScore += scoreThreeLine;
                numberLinesCleared += 3;
                currentLine += numberOfRowsThisTurn;
            }
            else if (numberOfRowsThisTurn == 4)
            {
                currentScore += scoreFourLine;
                numberLinesCleared += 4;
                currentLine += numberOfRowsThisTurn;
            }
            numberOfRowsThisTurn = 0;
            FindObjectOfType<GAME>().UpdateHighscore();
            PlayLineClearedSound();
        }
    }

    public void PlayLineClearedSound() 
    {
        audioSource.PlayOneShot(ClearedLineSound);
    }

    public bool CheckIsValidPosition(GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (!CheckIsInsideGrid(pos))
            {
                return false;
            }
            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckIsAboveGrid(Tetrominos pieceOverTetris)
    {
        for (int x = 0; x <= gridHeight; x++)
        {
            foreach (Transform square in pieceOverTetris.transform)
            {
                Vector2 position = Round(square.position);

                if (position.y > gridHeight)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRowAt(int y)
    {
        //- The parameter y, is the row we will iterate over in the grid array to check each x position for a transform
        for (int x = 0; x < gridWidth; ++x)
        {
            //- If we find a position that returns NUll instead of the transform, then we know that the row is not full
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        //- Since we found a full row, we increment the full row variable
        numberOfRowsThisTurn++;

        //- If we iterated over the entire loop and didn't  encounter any NULL positions, then we return true
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowDown(y + 1);
                --y;
            }
        }
    }

    public void UpdateGrid(Tetrominos tetromino)
    {
        for(int y = 0; y < gridHeight; ++y)
        {
            for(int x = 0; x < gridWidth; ++x) 
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;

            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetrominos>().enabled = false;

        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetrominos>().enabled = true;

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetrominos>().enabled = false;
        }
    }

    

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y > 0);
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    
    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 9);
        string randomTetrominoName = "Prefabs/Tetromino_I";
        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/Tetromino_I";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Tetromino_I4";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Tetromino_J";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Tetromino_L";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Tetromino_O";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Tetromino_S";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Tetromino_T";
                break;
            case 8:
                randomTetrominoName = "Prefabs/Tetromino_Z";
                break;
        }
        return randomTetrominoName;
    }

    public void UpdateHighscore()
    {
        if (currentScore > startHighScore1)
        {
            PlayerPrefs.SetInt("highscore5", startHighScore4);
            PlayerPrefs.SetInt("highscore4", startHighScore3);
            PlayerPrefs.SetInt("highscore3", startHighScore2);
            PlayerPrefs.SetInt("highscore2", startHighScore1);
            PlayerPrefs.SetInt("highscore1", currentScore);
        }
        else if (currentScore > startHighScore2)
        {
            PlayerPrefs.SetInt("highscore5", startHighScore4);
            PlayerPrefs.SetInt("highscore4", startHighScore3);
            PlayerPrefs.SetInt("highscore3", startHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }
        else if (currentScore > startHighScore3)
        {
            PlayerPrefs.SetInt("highscore5", startHighScore4);
            PlayerPrefs.SetInt("highscore4", startHighScore3);
            PlayerPrefs.SetInt("highscore3", currentScore);
        }
        else if (currentScore > startHighScore4)
        {
            PlayerPrefs.SetInt("highscore5", startHighScore4);
            PlayerPrefs.SetInt("highscore4", currentScore);
        }
        else if (currentScore > startHighScore5)
        {
            PlayerPrefs.SetInt("highscore5", currentScore);
        }
        PlayerPrefs.SetInt("lastScore", currentScore);
    }

    public void GameOver()
    {
        UpdateHighscore();
        SceneManager.LoadScene("GAMEOVER");
    }
}
