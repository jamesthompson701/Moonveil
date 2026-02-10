
using UnityEngine;
using UnityEngine.UI;

public class PageArrow : MonoBehaviour
{
    //TODO: Make tab buttons set currentIdx
    //TODO: Disable arrows when at end of page


    public GameObject[] pages;
    public int currentIdx = 0;

    public GameObject forwardArrow;
  
    public void ForwardArrow()
    {
        if (currentIdx > pages.Length)
        {
            currentIdx = pages.Length;
            forwardArrow.SetActive(false);
        }
        else
        {
            pages[currentIdx].SetActive(false);
            currentIdx++;
            pages[currentIdx].SetActive(true);
        }

    }

    public void BackArrow()
    {
        pages[currentIdx].SetActive(false);
        currentIdx++;
        pages[currentIdx].SetActive(true);

        if (currentIdx >= pages.Length)
        {
            forwardArrow.SetActive(false);
        }
    }
}
