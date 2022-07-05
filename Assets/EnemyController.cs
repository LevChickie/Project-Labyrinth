using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float health;
    public float reach;
    public int damage;
    public bool pursuit;
    public FieldOfView fieldOfView;
    public GameObject player;
    public GameObject enemy;
    public float enemySpeed;
    public CharacterController controller;
    public Animator enemyAnimator;
    private void Start()
    {
        
    }

    private void Update()
    {
        if (fieldOfView.playerInSight)
        {
            enemyAnimator.SetBool("PlayerInSight", true);
            player = GameObject.FindWithTag("Player");
            //enemy.SetDestination(player.transform.position);
            Vector3 direction = (player.transform.position - transform.position).normalized;
            controller.Move(direction * enemySpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position, player.transform.position) < reach)
            {
                enemyAnimator.SetTrigger("AttackPlayer");
            }
            pursuit = true;
            //transform.LookAt(player.transform);
        }
        else if (pursuit)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            enemyAnimator.SetBool("PlayerInSight", false);
        }
    }

    public void EnemyHurt(int damage)
    {
        health -= damage;
        Debug.Log(health);
        if (health <= 0)
        {
            enemyAnimator.SetBool("IsDead", true);
            enemy.SetActive(false);
            //EnemyDie;
            player.GetComponent<PlayerScript>().lootDeadEnemies();
        }
    }
}
