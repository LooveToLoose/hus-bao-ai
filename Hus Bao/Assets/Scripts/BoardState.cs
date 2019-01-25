using System.Collections;

[System.Serializable]
public class BoardState
{
    public int[,] arIStoneSetup = new int[2, 16];
    public int[] arITotalStones = new int[2];
    public int iTurnOfPlayer = 0;
    public int iMove = 1;

    public int iBestMoveFound = -1;
    public float fBestMoveScore = -10000;
    public bool bCoroRunning = false;

    public BoardState Duplicate()
    {
        BoardState boardState = new BoardState();

        boardState.iMove = iMove;

        boardState.iTurnOfPlayer = iTurnOfPlayer;
        boardState.arITotalStones[0] = arITotalStones[0];
        boardState.arITotalStones[1] = arITotalStones[1];

        for (int i = 0; i < 16; i++)
        {
            boardState.arIStoneSetup[0, i] = arIStoneSetup[0, i];
            boardState.arIStoneSetup[1, i] = arIStoneSetup[1, i];
        }

        return boardState;
    }

    public void ResetToDefault()
    {
        iBestMoveFound = -1;
        fBestMoveScore = -10000;
        bCoroRunning = false;
        iMove = 1;

        iTurnOfPlayer = GameManager.singleton.iStartPlayer;
        arITotalStones[0] = 0;
        arITotalStones[1] = 0;

        for (int i = 0; i < 16; i++)
        {
            if (i <= 3)
            {
                arIStoneSetup[0, i] = 0;
                arIStoneSetup[1, i] = 0;
            }
            else
            {
                arIStoneSetup[0, i] = 2;
                arIStoneSetup[1, i] = 2;
                arITotalStones[0] += 2;
                arITotalStones[1] += 2;
            }
        }
    }

    public void ExecuteMove(int _iPlayer, int _iNr)
    {
        // Take stones into hand
        int iStonesInHand = arIStoneSetup[_iPlayer, _iNr];
        arIStoneSetup[_iPlayer, _iNr] = 0;
        int iCurrentField = _iNr;

        // While you have stones in hand...
        int iEndlessExit = 1000;
        while (iStonesInHand > 0)
        {
            // Go to next field
            iCurrentField = (iCurrentField + 1) % 16;

            // Drop one stone
            arIStoneSetup[_iPlayer, iCurrentField] += 1;
            iStonesInHand -= 1;
            
            // If no more stones in hand...
            if (iStonesInHand == 0)
            {
                // Grab all stones if there are any on this field
                if (arIStoneSetup[_iPlayer, iCurrentField] > 1)
                {
                    iStonesInHand = arIStoneSetup[_iPlayer, iCurrentField];
                    arIStoneSetup[_iPlayer, iCurrentField] = 0;

                    // Grab all enemy stones on the opposite field if there are any
                    if (iCurrentField <= 7)
                    {
                        int iCurrentFieldMirroredFront = 7 - iCurrentField;
                        int iStonesOnFieldMirroredFront = arIStoneSetup[1 - iTurnOfPlayer, iCurrentFieldMirroredFront];
                        if (iStonesOnFieldMirroredFront >= 1)
                        {
                            iStonesInHand += iStonesOnFieldMirroredFront;
                            arITotalStones[iTurnOfPlayer] += iStonesOnFieldMirroredFront;
                            arITotalStones[1 - iTurnOfPlayer] -= iStonesOnFieldMirroredFront;
                            arIStoneSetup[1 - iTurnOfPlayer, iCurrentFieldMirroredFront] = 0;

                            // Grab the ones behind that as well
                            int iCurrentFieldMirroredBack = iCurrentField + 8;
                            int iStonesOnFieldMirroredBack = arIStoneSetup[1 - iTurnOfPlayer, iCurrentFieldMirroredBack];
                            if (iStonesOnFieldMirroredBack >= 1)
                            {
                                iStonesInHand += iStonesOnFieldMirroredBack;
                                arITotalStones[iTurnOfPlayer] += iStonesOnFieldMirroredBack;
                                arITotalStones[1 - iTurnOfPlayer] -= iStonesOnFieldMirroredBack;
                                arIStoneSetup[1 - iTurnOfPlayer, iCurrentFieldMirroredBack] = 0;
                            }
                        }
                    }
                }
            }



            // Moves can actually continue for ever. This should prevent the program from getting stuck:
            iEndlessExit--;
            if (iEndlessExit <= 0)
            {
                break;
            }
        }

        // Switch Player Turn
        iTurnOfPlayer = 1 - iTurnOfPlayer;
        iMove++;
    }

    public bool BIsValidMove(int _iPlayer, int _iNr)
    {
        return ((arIStoneSetup[_iPlayer, _iNr] > 1) && (_iPlayer == iTurnOfPlayer));
    }

    public float FEvaluatePosition(int _player)
    {
        return (arITotalStones[_player] - arITotalStones[1 - _player]);
    }

    // AI
    public IEnumerator AiStepFindBestMove(int _iDepth)
    {
        // Set starting best move score
        fBestMoveScore = -1100 + iMove;

        // For every move
        for (int i = 0; i < 16; i++)
        {
            // For every VALID move
            if (BIsValidMove(iTurnOfPlayer, i))
            {
                // Try the move out
                BoardState boardStateTry = Duplicate();
                boardStateTry.ExecuteMove(iTurnOfPlayer, i);
                float fMoveScore = 0;

                // If the depth ends here, evaluate the move right away
                if (_iDepth <= 1)
                {
                    fMoveScore = boardStateTry.FEvaluatePosition(iTurnOfPlayer);
                }
                else // Otherwise evaluate the move by going deeper
                {
                    boardStateTry.bCoroRunning = true;
                    IEnumerator coroAiStep = boardStateTry.AiStepFindBestMove(_iDepth-1);
                    while (boardStateTry.bCoroRunning)
                    {
                        coroAiStep.MoveNext();
                        yield return null;
                    }
                    fMoveScore = -boardStateTry.fBestMoveScore;
                }

                // Save the move if it's the best one evaluated so far
                if (fMoveScore > fBestMoveScore)
                {
                    iBestMoveFound = i;
                    fBestMoveScore = fMoveScore;
                }
            }
        }

        bCoroRunning = false;
        yield return null;
    }
}
