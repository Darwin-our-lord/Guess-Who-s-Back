using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[System.Serializable]
public struct Credit
{
    public string title;
    public string name;
}

public class CreditsManager : MonoBehaviour
{
    [Header("idk yet")]
    public List<Credit> credits = new List<Credit>();

    public TMP_Text titleTxt;
    public TMP_Text nameTxt;

    private string hiddenWord;
    private int creditsCompleted = -1;
    private void Awake()
    {
        creditsCompleted++;
        MakeHiddenWord(creditsCompleted);
        titleTxt.text = credits[creditsCompleted].title;
    }
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
    private IEnumerator StartNextCredit()
    {
        if(creditsCompleted >= credits.Count)
        {
            yield return new WaitForSeconds(2);
            //end credits
        }
        else
        {
            yield return new WaitForSeconds(2);
            creditsCompleted++;
            MakeHiddenWord(creditsCompleted);
            titleTxt.text = credits[creditsCompleted].title;
        }

    }
    private void OnGUI()
    {
        Event e = Event.current;

        if(e.type == EventType.KeyDown && e.keyCode.ToString().Length==1) //e.keyCode.ToString().Length==1 to prevent the "None" 
        {

            string keyPressed = e.keyCode.ToString();
            string result = "";
            if (credits[creditsCompleted].name.Contains(keyPressed))
            {
                if (hiddenWord.Contains(keyPressed))
                {
                    StartCoroutine(ColorFlash(Color.yellow, Color.white));
                }
                else
                {
                    for(int i = 0; i < credits[creditsCompleted].name.Length; i++)
                    {
                        if (credits[creditsCompleted].name.ToUpper()[i].ToString() == keyPressed)
                        {
                            result+=keyPressed;
                        }
                        else
                        {
                            result += hiddenWord[i];
                        }
                    }
                    hiddenWord = result;
                    nameTxt.text = hiddenWord;
                    if(hiddenWord == credits[creditsCompleted].name)
                    {
                        StartCoroutine(ColorFlash(Color.green, Color.white));
                        StartCoroutine(StartNextCredit());
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

}
