using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{


    public class PlayerStats : CharacterStats
    {
       
        public HealthBar healthBar;

        ActorController actorController;
        public PlayerManager playerManager;
        private void Awake()
        {
            actorController = GetComponentInChildren<ActorController>();
            playerManager = GetComponent<PlayerManager>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);
            
            maxStamina = SetMaxStaminaFromHealthLevel();
            currentStamina= maxStamina;
            
            
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        private int SetMaxStaminaFromHealthLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }
        public void TakeDamage(int damage)
        {
            if (playerManager.isInvulerable == false)
            {
                return;
            }
            currentHealth = currentHealth - damage;
            healthBar.SetCurrentHealth(currentHealth);
            actorController.PlayTargetAnimImmediately("05002",Time.deltaTime);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                actorController.PlayTargetAnimImmediately("17002death",Time.deltaTime);
            }
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina = currentStamina - damage;
            
        }
    }
}