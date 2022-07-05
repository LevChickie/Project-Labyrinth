using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    private Image HealthBar;
    public float currentHealth;
    private float maxHealth = 100f;
    PlayerScript player;

    private void Start() {
        HealthBar = GetComponent<Image>();
        player = FindObjectOfType<PlayerScript>();
        maxHealth = (float)player.maxhealth;
        currentHealth = (float)player.health;
    }
    private void Update()
    {
        currentHealth = player.health;
        HealthBar.fillAmount = currentHealth / maxHealth;
    }
}
