using System.Collections;
using TMPro;
using UnityEngine;

public class Order : MonoBehaviour, IInteractable
{
    [TextArea] public string orderText;
    [TextArea] public string responseText;
    public TextMeshProUGUI dialogueText;

    private bool hasInteractedOnce = false;

    public string GetInteractionText()
    {
        return hasInteractedOnce ? responseText : orderText;
    }

    public void Interact()
    {
        if (dialogueText == null)
        {
            Debug.LogWarning("No se asignó dialogueText.");
            return;
        }

        if (!hasInteractedOnce)
        {
            dialogueText.text = orderText;
            hasInteractedOnce = true;
            //StartCoroutine(Finish());

        }
        else
        {
            dialogueText.text = responseText;
            hasInteractedOnce = false;
        }
    }

    IEnumerator Finish()
    {
        yield return new WaitForSeconds(1f);
        dialogueText = null;
    }
}
