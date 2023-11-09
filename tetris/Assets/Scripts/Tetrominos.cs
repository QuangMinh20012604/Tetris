using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrominos : MonoBehaviour
{
    float fall = 0;
    public float fallSpeed;

    public bool AllowRotation = true;
    public bool LimitRotation = false;

    private float continuousVerticalSpeed = 0.03f;
    private float continuousHorizontalSpeed = 0.04f;
    private float buttonDownWaitMax = 0.02f;

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownTimerHorizontal = 0;
    private float buttonDownTimerVertical = 0;

    public AudioClip moveSound; //- Sound for when tetrimino is moved
    public AudioClip rotateSound; //- Sound for when tetrimino is rotate
    public AudioClip landSound; //- Sound for when tetrimino lands

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fallSpeed = GameObject.Find("GAMEScript").GetComponent<GAME>().fallSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GAME.GameIsPause)
        {
            CheckInputPlayer();
        }
        
    }

    private void CheckInputPlayer()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false; 
            horizontalTimer = 0;
            buttonDownTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();   
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall > fallSpeed)
        {
            MoveDown();
        }
    }

    /// <summary>
    /// Move the left.
    /// </summary>
    private void MoveLeft()
    {

        if (movedImmediateHorizontal)
        {
            if (buttonDownTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            movedImmediateHorizontal = true;
        }

        horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);
        if (CheckIsValidPosition())
        {
            FindObjectOfType<GAME>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    /// <summary>
    /// Move the right.
    /// </summary>
    private void MoveRight()
    {

        if (movedImmediateHorizontal)
        {
            if (buttonDownTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            movedImmediateHorizontal = true;
        }

        horizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);
        if (CheckIsValidPosition())
        {
            FindObjectOfType<GAME>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    /// <summary>
    /// Move down.
    /// </summary>
    private void MoveDown()
    {

        if (movedImmediateVertical)
        {
            if (buttonDownTimerVertical < buttonDownWaitMax)
            {
                buttonDownTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            movedImmediateVertical = true;
        }

        verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);
        if (CheckIsValidPosition())
        {
            FindObjectOfType<GAME>().UpdateGrid(this);
            if (Input.GetKey(KeyCode.DownArrow))
            {
                PlayMoveAudio();
            }
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<GAME>().DeleteRow();
            if (FindObjectOfType<GAME>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<GAME>().GameOver();
            }
            GameObject.FindObjectOfType<GAME>().currentScore += 10;
            FindObjectOfType<GAME>().UpdateHighscore();

            PlayLandAudio();
            FindObjectOfType<GAME>().SpawnNextTetromino();

            enabled = false;

        }

        fall = Time.time;
    }

    /// <summary>
    /// Roteta this instance.
    /// </summary>
    private void Rotate()
    {

        if (AllowRotation)
        {

            if (LimitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                    if (CheckIsValidPosition())
                    {
                        FindAnyObjectByType<GAME>().UpdateGrid(this);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                    if (CheckIsValidPosition())
                    {
                        FindAnyObjectByType<GAME>().UpdateGrid(this);
                        PlayRotateAudio();
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                    }
                }
            }
            else
            {
                transform.Rotate(0, 0, -90);
                if (CheckIsValidPosition())
                {
                    FindAnyObjectByType<GAME>().UpdateGrid(this);
                    PlayRotateAudio();
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
        }
    }


    /// <summary>
    /// Plays audio clip when tetromino is moved left, right, down
    /// </summary>
    void PlayMoveAudio()
    {
        audioSource.PlayOneShot(moveSound);
    }

    /// <summary>
    /// Plays audio clip when tetromino is rotated
    /// </summary>
    void PlayRotateAudio()
    {
        audioSource.PlayOneShot(rotateSound);
    }
    /// <summary>
    /// Plays audio clip when tetromino lands
    /// </summary>
    void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<GAME>().Round(mino.position);
            if (FindObjectOfType<GAME>().CheckIsInsideGrid(pos) == false)
            {
                return false;
            }
            if (FindObjectOfType<GAME>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<GAME>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}
