using Unity.Cinemachine;
using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    public GameObject objectToHighlight;
    public GameObject leftClickIcon;

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
        leftClickIcon.SetActive(false);

    }

    private void HighlightObject()
    {
        leftClickIcon.SetActive(true);
        var outline = objectToHighlight.AddComponent<Outline>();

        if (objectToHighlight.CompareTag("Dialogue")) //Change to NPC if NPC tag is made
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.blue;
            outline.OutlineWidth = 5f;
        }
        else
        {
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 5f;
        }


    }
}
