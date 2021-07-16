using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract string GetDescription();
    public abstract void Interact(GameObject player);
}
