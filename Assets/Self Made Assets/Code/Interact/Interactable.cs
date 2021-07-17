using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract string GetDescription(GameObject player);
    public abstract void Interact(GameObject player);
}
