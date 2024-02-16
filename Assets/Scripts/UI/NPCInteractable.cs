using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

public class NPCInteractable : MonoBehaviour
{
    [SerializeField]
    GameObject UIPrompt;

    public NpcDialogue npcDiag;
    public bool hasEvent;
    GameObject player;

    [SerializeField]
    public bool nonDiagPopUp = false;

    public void DismissUI()
    {
        UIPrompt.SetActive(false);
        notEnoughKitsUI.SetActive(false);
        player.GetComponent<Movement>().unblockMovement();
        cameraMovement.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        player.GetComponent<PlayerInteraction>().inDialogue = false;
        nonDiagPopUp = false;
    }

    private void Start()
    {
        cameraMovement = GameObject.Find("3rd Person Camera Empty").GetComponent < 3rdPersonCamera > ();
        npcDiag = this.GetComponent<NpcDialogue>();
    }

    public void Interact()
    {
        diag.StartDialogue(npcDiag.hubWorldDialogue);
    }
}
*/