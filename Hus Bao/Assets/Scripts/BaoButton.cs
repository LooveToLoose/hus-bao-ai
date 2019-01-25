using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaoButton : MonoBehaviour
{
    List<GameObject> liGoChildren = new List<GameObject>();

    GameManager gameManager;
    Button myButton;
    Text myText;

    [SerializeField]
    int iPlayer = 0;
    [SerializeField]
    int iNr = 0;

    private void Start()
    {
        //myButton = GetComponent<Button>();
        //myText = GetComponentInChildren<Text>();
    }

    public void RegisterOnStart()
    {
        gameManager = GameManager.singleton;
        gameManager.RegisterBaoButton(this, iPlayer, iNr);
        for(int i = 0; i < transform.childCount; i++)
        {
            liGoChildren.Add(transform.GetChild(i).gameObject);
        }
    }

    void OnEnable()
    {
        myButton = GetComponent<Button>();
        myText = GetComponentInChildren<Text>();
        myButton.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        myButton.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        gameManager.OnBaoButtonClick(iPlayer, iNr);
    }

    public void ApplyStones(int _iStones, int _iTurnOfPlayer, int _iBestMove)
    {
        // Children
        for (int i = 0; i < liGoChildren.Count; i++)
        {
            liGoChildren[i].SetActive(_iStones > i);
        }

        // Text
        myText.text = _iStones.ToString();
        if (_iStones == 0)
            myText.text = "";
        myText.fontSize = 25 + _iStones * 6;

        // Button
        if ((iPlayer == _iTurnOfPlayer) && (_iStones > 1))
        {
            if (iNr == _iBestMove)
            {
                ColorBlock colBlockActive = myButton.colors;
                colBlockActive.normalColor = gameManager.colAi;
                colBlockActive.pressedColor = gameManager.colAi;
                colBlockActive.highlightedColor = gameManager.colAi;
                myButton.colors = colBlockActive;
            }
            else
            {
                ColorBlock colBlockActive = myButton.colors;
                colBlockActive.normalColor = gameManager.colHighlighted;
                colBlockActive.pressedColor = Color.Lerp(gameManager.colHighlighted, gameManager.colBack, 0.5f);
                colBlockActive.highlightedColor = gameManager.colHighlighted;
                myButton.colors = colBlockActive;
            }
        }
        else
        {
            if (iPlayer == _iTurnOfPlayer)
            {
                ColorBlock colBlockDisabled = myButton.colors;
                colBlockDisabled.normalColor = Color.Lerp(gameManager.colHighlighted, gameManager.colBack, 0.6f);
                colBlockDisabled.pressedColor = Color.Lerp(gameManager.colHighlighted, gameManager.colBack, 0.6f);
                colBlockDisabled.highlightedColor = Color.Lerp(gameManager.colHighlighted, gameManager.colBack, 0.6f);
                myButton.colors = colBlockDisabled;
            }
            else
            {
                ColorBlock colBlockDisabled = myButton.colors;
                colBlockDisabled.normalColor = Color.Lerp(gameManager.colHighlighted, gameManager.colBack, 0.6f);
                colBlockDisabled.pressedColor = Color.Lerp(gameManager.colHighlighted, gameManager.colBack, 0.6f);
                colBlockDisabled.highlightedColor = Color.Lerp(gameManager.colHighlighted, gameManager.colBack, 0.6f);
                myButton.colors = colBlockDisabled;
            }
        }
    }
}
