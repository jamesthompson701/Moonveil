using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    //Ingredients
    public ItemSO ingr1;
    public ItemSO ingr2;
    public ItemSO ingr3;

    //Output Item
    public ItemSO output;

}
