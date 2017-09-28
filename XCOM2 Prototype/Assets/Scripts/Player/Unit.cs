﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    public TileMap tileMap;
    public Color[] color;
    public Image[] healthBar;
    public Text healthText;
    public Slider healthSlider;
    public bool isFriendly;
    [SerializeField, Range(0, 100)]
    public int health;
    int healthMax;
    [SerializeField, Range(0, 100)]
    public int damage;

    public int actions = 2;
    Unit target;

    public TurnSystem turnSystem;
    public bool isSelected = false;

    void Start () {
        //Sets color of healthbar
        if (!isFriendly)
        {
            for(int i = 0; i <= 1; i++)
            {
                healthBar[i].color = color[i];
            }
        }
        healthMax = health;
        healthText.text = health + "/" + healthMax;
    }

    void Update()
    {
        if (isSelected && actions > 0)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        healthText.text = health + "/" + healthMax;
        healthSlider.value = health;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
