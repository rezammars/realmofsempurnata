using UnityEngine;

public class ObjectInteract : MonoBehaviour
{
    public string objectName = "Item";

    // Fungsi ini bisa dipanggil dari skrip lain (misalnya PlayerInteract.cs)
    public void DoInteraction()
    {
        Debug.Log("Player berinteraksi dengan: " + objectName);

        // Jika objek adalah wortel, beri efek peningkatan cahaya
        if (objectName.ToLower() == "wortel")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Movement movement = player.GetComponent<Movement>();
                if (movement != null)
                {
                    movement.BoostLight(); // Pastikan fungsi ini ada di skrip Movement.cs
                }
            }
        }

        // Hancurkan objek setelah diambil
        Destroy(gameObject);
    }
}
