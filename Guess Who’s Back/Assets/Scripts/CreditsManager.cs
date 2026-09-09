using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[System.Serializable]
public struct Credit
{
    public string title;
    public string name;
    public bool guessed;
}

public class CreditsManager : MonoBehaviour
{
    [Header("Credits")]
    [SerializeField] List<Credit> credits; //W credits:)
    
    [SerializeField] TMP_Text titleTxt;
    [SerializeField] TMP_Text nameTxt;

    [Header("UI")]
    [SerializeField] GameObject MainUI;
    [SerializeField] GameObject GuessUI;

    private string hiddenWord;
    private int currentCreditNR = -1;
    private bool isGuessing = false;

    private void MakeHiddenWord(int creditNr)
    {
        hiddenWord = "";
        for (int i = 0; i < credits[creditNr].name.Length; i++)
        {
            char letter = credits[creditNr].name[i];

            if (char.IsWhiteSpace(letter))
            {
                hiddenWord += "";
                continue;
            }
            hiddenWord += "_";
        }

        nameTxt.text = hiddenWord;
    }
    public void StartGuessCredit(int nr)
    {
        if (credits[nr].guessed) return; //if already guessed, do nothing

        isGuessing = true;
        SwapGuessAndMainUI();
        Debug.Log("Guessing credit: " + credits[nr].title + " - " + credits[nr].name);

        currentCreditNR = nr;   
        MakeHiddenWord(currentCreditNR);
        titleTxt.text = credits[currentCreditNR].title;
    }

    private void OnGUI()
    {
        if (isGuessing == false) return; //if no credit is being guessed, do nothing

        Event e = Event.current;

        if(e.type == EventType.KeyDown && e.keyCode.ToString().Length==1) //e.keyCode.ToString().Length==1 to prevent the "None" 
        {
            string keyPressed = e.keyCode.ToString();
            string result = "";
            if (credits[currentCreditNR].name.ToUpper().Contains(keyPressed.ToUpper()))
            {
                if (hiddenWord.Contains(keyPressed))
                {
                    StartCoroutine(ColorFlash(Color.yellow, Color.white)); //same letter guessed again, flash yellow
                }
                else
                {
                    for(int i = 0; i < credits[currentCreditNR].name.Length; i++)
                    {
                        if (credits[currentCreditNR].name.ToUpper()[i].ToString() == keyPressed)
                        {
                            result += keyPressed;
                        }
                        else
                        {
                            result += hiddenWord[i];
                        }
                    }
                    hiddenWord = result;
                    nameTxt.text = hiddenWord;
                    if(hiddenWord.ToUpper() == credits[currentCreditNR].name.ToUpper()) //word guessed correctly
                    {
                        StartCoroutine(ColorFlash(Color.green, Color.white));
                        StartCoroutine(Win());
                    }
                }
            }
            else
            {
                StartCoroutine(ColorFlash(Color.red, Color.white));
            }
        }
        
    }
    private IEnumerator ColorFlash(Color colSwap, Color colEnd)
    {
        nameTxt.color = colSwap;
        yield return new WaitForSeconds(0.5f);
        nameTxt.color = colEnd;
    }
    private IEnumerator Win()
    {
        isGuessing = false;

        yield return new WaitForSeconds(2f);

        Credit temp = credits[currentCreditNR];
        temp.guessed = true;
        credits[currentCreditNR] = temp;
        
        SwapGuessAndMainUI();
    }
    private void SwapGuessAndMainUI()
    {
        MainUI.SetActive(!MainUI.activeSelf);
        GuessUI.SetActive(!GuessUI.activeSelf);
    }
}
