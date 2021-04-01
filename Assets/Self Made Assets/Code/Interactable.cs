using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float radius = 2.5f;

    public GameObject interactUI;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public abstract void interact();

}
