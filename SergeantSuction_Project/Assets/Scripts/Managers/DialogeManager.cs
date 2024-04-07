using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogeManager : MonoBehaviour
{
    [SerializeField]
    private Image lieutenantImage;
    [SerializeField]
    private Image sergeantImage;
    [SerializeField]
    private Text textbox;
    [SerializeField]
    private Image textbackground;
    [SerializeField]
    private List<string> dialogueLines;
    [SerializeField]
    private float typingTime;
    [SerializeField]
    private float activationDelay;

    private int currentLineIndex = 0;
    private string currentLine;
    private bool isTyping = false;

    // Start is called before the first frame update
    void Start()
    {
        if(activationDelay > 0)
        {
            WaitForSeconds wait = new WaitForSeconds(activationDelay);
        }
        ShowDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!isTyping)
            {
                //currentLineIndex++;
                if(currentLineIndex < dialogueLines.Count)
                {
                    ShowDialogue();
                    SwapFace();
                }
                else
                {
                    isTyping = false;
                    textbox.gameObject.SetActive(false);
                    sergeantImage.gameObject.SetActive(false);
                    lieutenantImage.gameObject.SetActive(false);
                    textbackground.gameObject.SetActive(false);
                }
            }
            else
            {
                StopAllCoroutines();
                isTyping = false;
                textbox.text = currentLine;
            }
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(isTyping)
            {
                StopAllCoroutines();
                isTyping = false;
                textbox.text = currentLine;
            }
            while (currentLineIndex < dialogueLines.Count)
            {
                textbox.text += dialogueLines[currentLineIndex];
                currentLineIndex++;
            }
        }
    }

    void ShowDialogue()
    {
        currentLine = dialogueLines[currentLineIndex];
        StartCoroutine(TypeText(currentLine));
        currentLineIndex++;
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        textbox.text = "";
        foreach (char letter in text)
        {
            textbox.text += letter;
            yield return new WaitForSeconds(typingTime);
        }
        isTyping = false;
    }

    private void SwapFace()
    {
        if(lieutenantImage.gameObject.activeSelf)
        {
            lieutenantImage.gameObject.SetActive(false);
            sergeantImage.gameObject.SetActive(true);
        }
        else
        {
            lieutenantImage.gameObject.SetActive(true);
            sergeantImage.gameObject.SetActive(false);
        }
    }
}
