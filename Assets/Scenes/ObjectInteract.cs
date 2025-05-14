using UnityEngine;

public class ObjectInteract : MonoBehaviour
{
    public string objectName = "Item";

    public void DoInteraction()
    {
        Debug.Log("Player berinteraksi dengan: " + objectName);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Movement movement = player.GetComponent<Movement>();
            if (movement != null)
            {
                switch (objectName.ToLower())
                {
                    case "wortel":
                        movement.BoostLight();
                        break;
                    case "sepatu":
                        movement.ActivatePowerJump();
                        break;
                    case "bayam":
                        movement.ActivateInvincibility(5f);
                        break;
                }
            }
        }

        Destroy(gameObject);
    }
}
