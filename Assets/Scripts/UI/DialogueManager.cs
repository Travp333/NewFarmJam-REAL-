using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
public class DialogueManager : MonoBehaviour
{


    [SerializeField]
    Movement movement;
    [SerializeField]
    OrbitCamera cameraMovement;
    public Text nameText;
    public Text dialogueText;
    private Queue<string> sentences;
    public GameObject dialogueBox;
    float gate;
    PlayerInteraction playerInt;
    [SerializeField]
    GameObject interactText;
    public bool isDialoging;

    void Start()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (g.GetComponent<PlayerInteraction>() != null)
            {
                playerInt = g.GetComponent<PlayerInteraction>();
            }
        }

        sentences = new Queue<string>();
    }

    public void StartDialogue(AltDialogue dialogue)
    {
        isDialoging = true;
        interactText.SetActive(false);
        movement.blockMovement();
        cameraMovement.enabled = false;
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
        nameText.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        dialogueBox.SetActive(true);
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (!pause.isPaused)
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }
            string curSentence = sentences.Dequeue();
            StopAllCoroutines(); 
            StartCoroutine(TypeSentence(curSentence));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {

        dialogueText.text = "";
        gate = 5;
        foreach (char letter in sentence.ToCharArray())
        {
            if (gate == 5)
            {
                gate = 0;
            }
            else
            {
                gate += 1;
            }
            dialogueText.text += letter;
            yield return null;
        }
    }
    public void EndDialogue()
    {
        isDialoging = false;
        playerInt.inDialogue = false;
        movement.unblockMovement();
        cameraMovement.enabled = true;

        dialogueBox.SetActive(false); //makes dialogue box disapear
        Cursor.lockState = CursorLockMode.Locked;
        if (!playerInt.nonDiagPopUp)
        {
            if (playerInt.interactiveConvo)
            {
                playerInt.interactiveConvo = false;
            }
        }
    }

}
*/