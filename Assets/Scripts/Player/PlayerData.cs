using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerData : MonoBehaviour
{
    [SerializeField]
    private byte maxHP = 5;
    private byte hp;

    [SerializeField]
    private Image barHP;

    [SerializeField]
    private TMP_Text textHP;

    private void Start()
    {
        hp = maxHP;
        textHP.text = $"{hp}/{maxHP}";
        Application.targetFrameRate = 120;
    }

    public void Damage(byte damage)
    {
        hp -= damage;
        barHP.fillAmount = hp / maxHP;
        textHP.text = $"{hp}/{maxHP}";

        if (hp < maxHP)
            Debug.Log("Dead");
    } 
}
