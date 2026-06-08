using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldTreePageUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    [Header("Tree Menu Content Pages")]
    public Transform treeOneContent;
    public Transform treeTwoContent;
    public Transform treeThreeContent;
    public Transform treeFourContent;

    [Header("World Trees")]
    public WorldTree treeOne;
    public WorldTree treeTwo;
    public WorldTree treeThree;
    public WorldTree treeFour;

    [Header("Book Page Content")]
    public Transform bookContentParent;

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (titleText != null)
            titleText.text = "WORLD TREES";

        ClearBookWidgets();

        CopyTreeSection(treeOneContent, treeOne, "World Tree 1");
        CopyTreeSection(treeTwoContent, treeTwo, "World Tree 2");
        CopyTreeSection(treeThreeContent, treeThree, "World Tree 3");
        CopyTreeSection(treeFourContent, treeFour, "World Tree 4");
    }

    private void CopyTreeSection(Transform sourceContent, WorldTree tree, string fallbackName)
    {
        if (sourceContent == null)
            return;

        wQuestItem[] questItems = sourceContent.GetComponentsInChildren<wQuestItem>(true);

        if (questItems.Length == 0)
            return;

        GameObject headerObject = new GameObject("Tree Header");
        headerObject.transform.SetParent(bookContentParent, false);

        TextMeshProUGUI headerText = headerObject.AddComponent<TextMeshProUGUI>();

        string treeName = tree != null ? tree.name : fallbackName;
        string level = "?";

        if (tree != null && tree.levelText != null)
            level = tree.levelText.text;

        headerText.text = treeName + " - Level " + level;
        headerText.fontSize = 28;
        headerText.alignment = TextAlignmentOptions.Center;

        foreach (wQuestItem originalQuestItem in questItems)
        {
            GameObject copiedWidget = Instantiate(originalQuestItem.gameObject, bookContentParent);
            copiedWidget.SetActive(true);

            wQuestItem copiedQuestItem = copiedWidget.GetComponent<wQuestItem>();
            if (copiedQuestItem != null)
                copiedQuestItem.myWorldTree = null;

            Button button = copiedWidget.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.interactable = false;
            }
        }
    }

    private void ClearBookWidgets()
    {
        if (bookContentParent == null)
            return;

        for (int i = bookContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(bookContentParent.GetChild(i).gameObject);
        }
    }
}