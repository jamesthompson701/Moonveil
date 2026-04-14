using Unity.Cinemachine;
using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    public GameObject objectToHighlight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HighlightObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(objectToHighlight.GetComponent<Outline>());

    }

    private void HighlightObject()
    {
        var outline = objectToHighlight.AddComponent<Outline>();

        if (objectToHighlight.CompareTag("Dialogue")) //Change to NPC if NPC tag is made
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.blue;
            outline.OutlineWidth = 2f;
        }
        else
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 2f;
        }


    }
}
