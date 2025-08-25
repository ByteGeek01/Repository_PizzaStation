using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Camera main;

    public float interactionDistance = 2f;

    public GameObject interactionUI;
    public TextMeshProUGUI text;

    private void Update()
    {
        InteractionRay();
    }

    void InteractionRay()
    {
        Ray ray = main.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;

        bool hitSometing = false;

        if(Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if(interactable != null)
            {
                hitSometing = true;
                text.text = interactable.GetInteractionText();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
        interactionUI.SetActive(hitSometing);
    }
}