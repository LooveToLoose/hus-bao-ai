using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class AI : MonoBehaviour
{
    GameManager gameManager;

    public static AI singleton;

    IEnumerator coroCurrent;
    bool bCoroRunning = true;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        gameManager = GameManager.singleton;
    }

    public void StartSearchingForBestMove(BoardState _boardStateStart)
    {
        coroCurrent = FindBestMove(_boardStateStart);
        bCoroRunning = true;
    }

    public void StopSearchingForBestMove()
    {
        coroCurrent = null;
        bCoroRunning = false;
    }

    public void Update()
    {
        if (coroCurrent != null)
        {
            if (bCoroRunning)
                coroCurrent.MoveNext();
            else
                coroCurrent = null;
        }
    }

    IEnumerator FindBestMove(BoardState _boardStateStart)
    {
        gameManager.textAi1.text = "";
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int iDepth = 1; iDepth <= 20; iDepth++)
        {
            _boardStateStart = _boardStateStart.Duplicate();

            _boardStateStart.bCoroRunning = true;
            IEnumerator coroAiStep = _boardStateStart.AiStepFindBestMove(iDepth);
            while (_boardStateStart.bCoroRunning)
            {
                while (stopwatch.ElapsedMilliseconds < 50)
                {
                    coroAiStep.MoveNext();
                }
                yield return null;
                stopwatch.Restart();
            }

            gameManager.boardStateCurrent.iBestMoveFound = _boardStateStart.iBestMoveFound;
            gameManager.ApplyBoardState(gameManager.boardStateCurrent);

            if (iDepth <= 6)
                gameManager.textAi1.text = "Hmm...";
            else if(iDepth <= 6)
                gameManager.textAi1.text = "Hmm... Maybe...";
            else if (iDepth <= 7)
                gameManager.textAi1.text = "Maybe this move?";
            else if (iDepth <= 8)
                gameManager.textAi1.text = "This move looks pretty solid.";
            else if (iDepth <= 9)
                gameManager.textAi1.text = "Yeah, I think this is the best one.";
            else
                gameManager.textAi1.text = "Now I'm certain. This move!";

            if (_boardStateStart.fBestMoveScore > 500)
                gameManager.textAi1.text = "This is the winning move!!!";
            if (_boardStateStart.fBestMoveScore < -500)
                gameManager.textAi1.text = "Oh, oh. I'm in serious trouble!!!";

            if (_boardStateStart.fBestMoveScore > 0)
                gameManager.textAi2.text = _boardStateStart.fBestMoveScore.ToString() + " point advantage in " + iDepth.ToString() + " moves";
            else
                gameManager.textAi2.text = _boardStateStart.fBestMoveScore.ToString() + " point disadvantage in " + iDepth.ToString() + " moves";

            if ((_boardStateStart.fBestMoveScore > 500) || (_boardStateStart.fBestMoveScore < -500))
                break;

            yield return null;
        }

        bCoroRunning = false;
    }
}
