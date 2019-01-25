using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Range(0,1)]
    public int iStartPlayer = 0;

    AI ai;

    public Text textAi1;
    public Text textAi2;
    public Text[] arTextTotalStones = new Text[2]; 
    public static GameManager singleton;
    public bool[] arBAiEnabled = new bool[2];

    public Color colAi = Color.green;
    public Color colBack = Color.black;
    public Color colHighlighted = Color.white;

    [SerializeField]
    BaoButton[,] arBaoButtons = new BaoButton[2,16];

    public BoardState boardStateCurrent = new BoardState();

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Start()
    {
        textAi1.color = colAi;
        textAi2.color = Color.Lerp(colAi, colBack, 0.2f);
        arTextTotalStones[0].color = Color.Lerp(colHighlighted, colBack, 0.8f);
        arTextTotalStones[1].color = Color.Lerp(colHighlighted, colBack, 0.8f);
        ai = AI.singleton;
        foreach (BaoButton baoButton in FindObjectsOfType<BaoButton>())
            baoButton.RegisterOnStart();
        StartNewGame();
        if (arBAiEnabled[boardStateCurrent.iTurnOfPlayer])
            ai.StartSearchingForBestMove(boardStateCurrent);
        else
        {
            textAi1.text = "";
            textAi2.text = "";
            ai.StopSearchingForBestMove();
        }
    }

    public void RegisterBaoButton(BaoButton _baoButton, int _iPlayer, int _iNr)
    {
        arBaoButtons[_iPlayer, _iNr] = _baoButton;
    }

    public void StartNewGame()
    {
        boardStateCurrent.ResetToDefault();
        ApplyBoardState(boardStateCurrent);
    }

    public void ApplyBoardState(BoardState _boardState)
    {
        arTextTotalStones[0].text = _boardState.arITotalStones[0].ToString();
        arTextTotalStones[1].text = _boardState.arITotalStones[1].ToString();
        for (int i = 0; i < 16; i++)
        {
            arBaoButtons[0, i].ApplyStones(_boardState.arIStoneSetup[0, i], _boardState.iTurnOfPlayer, _boardState.iBestMoveFound);
            arBaoButtons[1, i].ApplyStones(_boardState.arIStoneSetup[1, i], _boardState.iTurnOfPlayer, _boardState.iBestMoveFound);
        }
    }

    public void OnBaoButtonClick(int _iPlayer, int _iNr)
    {
        int iStonesOnButton = boardStateCurrent.arIStoneSetup[_iPlayer, _iNr];

        if ((boardStateCurrent.iTurnOfPlayer == _iPlayer) && (iStonesOnButton > 1))
        {
            boardStateCurrent.ExecuteMove(_iPlayer, _iNr);
            boardStateCurrent.iBestMoveFound = -1;
            ApplyBoardState(boardStateCurrent);
            if (arBAiEnabled[boardStateCurrent.iTurnOfPlayer])
                ai.StartSearchingForBestMove(boardStateCurrent);
            else
            {
                textAi1.text = "";
                textAi2.text = "";
                ai.StopSearchingForBestMove();
            }
        }
    }
}
