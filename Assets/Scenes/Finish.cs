using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level selesai!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}