using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI hpText;

    public void UpdateHP(int currentHP, int maxHP)
    {
        hpText.text = "HP: " + currentHP + " / " + maxHP;
    }
}
