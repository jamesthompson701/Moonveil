using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    public GameObject objectToHighlight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HighlightObject();
            Debug.Log("this be working");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(objectToHighlight.GetComponent<Outline>());
    }

    private void HighlightObject()
    {
        var outline = objectToHighlight.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 2f;
    }
}
