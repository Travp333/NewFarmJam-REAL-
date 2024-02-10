using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    float interactRange = 2.5f;
    public bool inDialogue;
    DialogueManager manager;
    public bool interactiveConvo;
    public bool nonDiagPopUp;
    

    private void Start()
    {
        manager = GameObject.Find("Dialogue Manager").GetComponentInChildren<DialogueManager>();
    }

    private void Update()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (Collider collider in colliderArray)
            {
                if (collider.gameObject.tag == "Interactable" || collider.gameObject.tag == "NPC")
                {
                    if (collider.TryGetcomponent(out Interactable interactable))
                    {
                        interactable.Interact();
                    }
                }

                if (collider TryGetComponent(out NPCInteractable npcInteractable))
                    {
                    if (!inDialogue)
                    {
                        nonDiagPopUp = false;
                        npcInteractable.NPCInteract();
                        inDialogue = true;
                        if (npcInteractable.hasEvent)
                        {
                            interactiveConvo = true;
                        }
                    }
                    else
                    {
                        if (npcInteractable.nonDiagPopUp)
                        {
                            nonDiagPopUp = true;
                        }
                        else
                        {
                            nonDiagPopUp = false;
                            manager.DisplayNextSentence();
                        }
                    }
                }
            }
        }
    }

    public Interactable GetInteractableObject()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.gameObject.tag == "Interactable" || collider.gameObject.tag == "NPC")
            {
                if (collider.TryGetComponent(out Interactable interactable))
                {
                    return interactable;
                }
            }
        }
        return null;
    }

    public NPCInteractable GetNPCInteractableObject()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.gameObject.tag == "Interactable" || collider.gameObject.tag == "NPC")
            {
                if (collider.TryGetComponent(out NPCInteractable npcInteractable))
                {
                    return npcInteractable;
                }
            }
        }
        return null;
    }
}