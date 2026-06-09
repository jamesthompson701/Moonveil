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

    [Header("Book Tree Containers")]
    // These are child containers inside the book page Content object.
    // Each one should have its own Vertical Layout Group.
    // Example:
    // Content
    // ├── FirstTree
    // ├── SecondTree
    // ├── ThirdTree
    // └── FourthTree
    public Transform firstTreeContainer;
    public Transform secondTreeContainer;
    public Transform thirdTreeContainer;
    public Transform fourthTreeContainer;

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
        ClearAllTreeContainers();

        // Copy each tree's current quest item widgets into its matching book page container.
        CopyTreeSection(treeOneContent, treeOne, firstTreeContainer, "World Tree 1");
        CopyTreeSection(treeTwoContent, treeTwo, secondTreeContainer, "World Tree 2");
        CopyTreeSection(treeThreeContent, treeThree, thirdTreeContainer, "World Tree 3");
        CopyTreeSection(treeFourContent, treeFour, fourthTreeContainer, "World Tree 4");
    }

    private void CopyTreeSection(
        Transform sourceContent,
        WorldTree tree,
        Transform targetContainer,
        string fallbackName)
    {
        // If this tree/content slot is not assigned, skip it.
        if (sourceContent == null)
            return;

        // If this book page container is not assigned, skip it.
        if (targetContainer == null)
            return;

        // Find all existing wQuestItem widgets under this tree menu's content object.
        // true means it will also find inactive children.
        wQuestItem[] questItems = sourceContent.GetComponentsInChildren<wQuestItem>(true);

        // If this tree has not generated any quest widgets yet, skip it.
        if (questItems.Length == 0)
            return;

        // Create a simple text header inside this tree's container.
        GameObject headerObject = new GameObject("Tree Header");
        headerObject.transform.SetParent(targetContainer, false);

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

        // Copy each existing quest widget into this tree's book page container.
        foreach (wQuestItem originalQuestItem in questItems)
        {
            GameObject copiedWidget = Instantiate(originalQuestItem.gameObject, targetContainer);
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

            // Remove button behavior while keeping the widget visually normal.
            Button button = copiedWidget.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.transition = Selectable.Transition.None;
                button.interactable = true;
            }
        }
    }

    // Clears every tree container before rebuilding the page.
    private void ClearAllTreeContainers()
    {
        ClearContainer(firstTreeContainer);
        ClearContainer(secondTreeContainer);
        ClearContainer(thirdTreeContainer);
        ClearContainer(fourthTreeContainer);
    }

    // Deletes all copied widgets/headers from one specific tree container.
    private void ClearContainer(Transform container)
    {
        if (container == null)
            return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }
}