using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldTreePageUI : MonoBehaviour
{
    // Main title text at the top of the World Tree book page.
    public TextMeshProUGUI titleText;

    [Header("Tree Menu Content Pages")]
    // These are the Content objects from each World Tree's menu.
    // The existing wQuestItem widgets are generated under these content objects.
    public Transform treeOneContent;
    public Transform treeTwoContent;
    public Transform treeThreeContent;
    public Transform treeFourContent;

    [Header("World Trees")]
    // References to the actual World Tree objects.
    // These are mainly used here to get the tree name and level text.
    public WorldTree treeOne;
    public WorldTree treeTwo;
    public WorldTree treeThree;
    public WorldTree treeFour;

    [Header("Book Page Content")]
    // This is the Content object inside the book page where copied widgets will appear.
    public Transform bookContentParent;

    // Runs every time this book page becomes active/opened.
    private void OnEnable()
    {
        RefreshUI();
    }

    // Rebuilds the World Tree page display from the current tree menu widgets.
    public void RefreshUI()
    {
        if (titleText != null)
            titleText.text = "WORLD TREES";

        // Remove old copied widgets so the page does not duplicate entries every time it opens.
        ClearBookWidgets();

        // Copy each tree's current quest item widgets into the book page.
        CopyTreeSection(treeOneContent, treeOne, "World Tree 1");
        CopyTreeSection(treeTwoContent, treeTwo, "World Tree 2");
        CopyTreeSection(treeThreeContent, treeThree, "World Tree 3");
        CopyTreeSection(treeFourContent, treeFour, "World Tree 4");
    }

    private void CopyTreeSection(Transform sourceContent, WorldTree tree, string fallbackName)
    {
        // If this tree/content slot is not assigned, skip it.
        if (sourceContent == null)
            return;

        // Find all existing wQuestItem widgets under this tree menu's content object.
        // true means it will also find inactive children.
        wQuestItem[] questItems = sourceContent.GetComponentsInChildren<wQuestItem>(true);

        // If this tree has not generated any quest widgets yet, skip it.
        if (questItems.Length == 0)
            return;

        // Create a simple text header for this tree section.
        GameObject headerObject = new GameObject("Tree Header");
        headerObject.transform.SetParent(bookContentParent, false);

        TextMeshProUGUI headerText = headerObject.AddComponent<TextMeshProUGUI>();

        // Use the actual tree object's name if possible.
        // Otherwise, use the fallback name.
        string treeName = tree != null ? tree.name : fallbackName;
        string level = "?";

        // Use the tree's existing level text if it exists.
        if (tree != null && tree.levelText != null)
            level = tree.levelText.text;

        headerText.text = treeName + " - Level " + level;
        headerText.fontSize = 28;
        headerText.alignment = TextAlignmentOptions.Center;

        // Copy each existing quest widget into the book page.
        foreach (wQuestItem originalQuestItem in questItems)
        {
            GameObject copiedWidget = Instantiate(originalQuestItem.gameObject, bookContentParent);
            copiedWidget.SetActive(true);

            // Make sure copied images are fully visible.
            Image[] images = copiedWidget.GetComponentsInChildren<Image>(true);

            foreach (Image img in images)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }

            // Remove the copied widget's connection to the real World Tree.
            // This keeps the book page display from controlling gameplay/tree menu logic.
            wQuestItem copiedQuestItem = copiedWidget.GetComponent<wQuestItem>();
            if (copiedQuestItem != null)
                copiedQuestItem.myWorldTree = null;

            // Disable the copied button's behavior while keeping its visuals normal.
            Button button = copiedWidget.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.transition = Selectable.Transition.None;
                button.interactable = true;
            }
        }
    }

    // Deletes all copied widgets/headers from the book page before rebuilding it.
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