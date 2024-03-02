using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Dialogue : MonoBehaviour
{
	public TextMeshProUGUI textComponent;

	public string[] lines;
	public string[] altLines;
	public bool useAltLines;
	public float textSpeed;
	[SerializeField]
	GameObject bgPanel;

	private int index;
	public Interact input;
	private void Start()
	{
		textComponent.text = string.Empty;
		bgPanel.SetActive(false);
	}
	public void ContinueDialogue() {
		if (!useAltLines)
		{
			if (textComponent.text == lines[index]) 
			{
				NextLine();
			}
		}
		if (useAltLines)
		{
			if (textComponent.text == altLines[index])
			{
				NextLine();
			}
		}
	}
	public void StartDialogue() {
		textComponent.text = string.Empty;
		bgPanel.SetActive(true);
		index = 0;
		if (!useAltLines) {
			StartCoroutine(TypeLine(lines));
		}
		if (useAltLines) {
			StartCoroutine(TypeLine(altLines));
		}
	}
	IEnumerator TypeLine(string[] s) {
		foreach (char c in s[index].ToCharArray()) {
			textComponent.text += c;
			yield return new WaitForSeconds(textSpeed);
		}
	}
	void NextLine() {
		if (!useAltLines)
		{
			if (index < lines.Length - 1)
			{
				index++;
				textComponent.text = string.Empty;
				StartCoroutine(TypeLine(lines));
			}
			else {
				if (input != null)
				{
					input.CloseDialogue();
					
				}
				bgPanel.SetActive(false);
				textComponent.text = string.Empty;
			}

		}
		if (useAltLines)
		{
			if (index < altLines.Length - 1)
			{
				index++;
				textComponent.text = string.Empty;
				StartCoroutine(TypeLine(altLines));
			}
			else
			{
				if (input != null) {
					input.CloseDialogue();
					
				}
				bgPanel.SetActive(false);
				textComponent.text = string.Empty;
			}
		}
	}
}
