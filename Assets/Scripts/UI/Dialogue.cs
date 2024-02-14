using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    public TextMeshProGUI textComponent;
    [SerializeField]
    public GameObject diagBackground;

    public string[] lines;

    public float textSpeed;

    private int index;

    [SerializeField]
    private GameObject diagBoxGameObject;

    [SerializeField]
    private PlayerInteraction playerInteration;

    
   private void Start()
    {
        textComponent.text = string.Empty;
        Hide();
    }


    private void Update()
    {
        if (GetKeydown(KeyCode.F))
        {
            Show();

            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                Hide();
            }
        }
    }

    public void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    public IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        diagBackground.SetActive(true);
        textComponent.gameObject.SetActive(true);
    }

    private void Hide()
    {
        diagBackground.SetActive(false);
        texstComponent.gameObject.SetActive(false);
    }
}
