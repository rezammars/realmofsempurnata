using UnityEngine;

public class ObjectInteract : MonoBehaviour
{
    public string objectName = "Item";

    public void DoInteraction()
    {
        Debug.Log("Player berinteraksi dengan: " + objectName);
        Destroy(gameObject);
    }
}