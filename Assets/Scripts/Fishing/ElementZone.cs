using UnityEngine;

public enum ElementType {Water, Fire, Earth, Air}
public class ElementZone : MonoBehaviour
{
    public ElementType element;
    public MineralType requiredElement;

}