using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public CharacterController controller;
    int points;
    public GameObject gameOver;
    public Text Score;
    public Transform cam;
    private Vector3 Vec;
    public int health;
    public int maxhealth = 100;
    private bool isGrounded;
    private bool jumpingPhase;
    private float momentum;
    public float jumpSpeed;
    public float playerSpeed;
    private float gravityValue = 9.81f;
    public float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    public int playerDamage;
    public LayerMask enemyLayer;
    public float reach;
    public Transform attackpoint;
    private bool playerUnderAttack;
    private Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        points = 0;
        health = maxhealth;
        isGrounded = true;
        jumpingPhase = false;
        attackpoint = transform;
        playerUnderAttack = false;
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.SetInt("SavedScene", SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene("MainMenu");
        }
        if (playerUnderAttack && Random.Range(0,10) > 7.5f)
        {
            //playerAnimator.SetTrigger("Hurt");
            Hurt(1);
        }
        if (Input.GetMouseButton(0))
        {
            playerAnimator.SetTrigger("Attack");
            Attack();
        }
        Move();
    }
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            playerUnderAttack = false;
        }
    }
    void OnTriggerEnter(Collider triggerCollider)
    {

        if (triggerCollider.gameObject.tag == "Wall" && jumpingPhase == true)
        {
            Debug.Log("Didnt jump");

            controller.Move( new Vector3(0f,
                triggerCollider.gameObject.transform.localPosition.y + triggerCollider.gameObject.transform.localScale.y / 2 + transform.localScale.y / 2,
                0f));
            isGrounded = true;
            jumpingPhase = false;
        }
        else if (triggerCollider.gameObject.tag == "Collectable")
        {
            points += (int)Random.Range(0.0f, 10.0f);
            triggerCollider.gameObject.SetActive(false);
            setScore();
        }
        else if (triggerCollider.gameObject.tag == "Enemy")
        {
            Hurt(triggerCollider.gameObject.GetComponent<EnemyController>().damage);
            playerUnderAttack = true;
            playerAnimator.SetTrigger("Hurt");

        }
        else if (triggerCollider.gameObject.tag == "Trap")
        {
            Hurt(50);
            triggerCollider.gameObject.SetActive(false);
            playerAnimator.SetTrigger("Hurt");
        }
        else if (triggerCollider.gameObject.tag == "Door")
        {
            SceneManager.LoadScene("Free");
            gameOver.SetActive(false);
        }

        else if (triggerCollider.gameObject.tag == "Food")
        {
            if (health < maxhealth)
            {
                if (health < (maxhealth - 30))
                {
                    health += 30;
                }
                else
                {
                    health = maxhealth;
                }
                triggerCollider.gameObject.SetActive(false);
            }
        }
    }
    void Move()
    {
        if (health > 0)
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                momentum = jumpSpeed;
                isGrounded = false;
                jumpingPhase = true;
                playerAnimator.SetTrigger("Jump");
            }
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            attackpoint.position = transform.position + direction * 1.5f;
            if (jumpingPhase)
            {
                momentum = momentum - gravityValue * Time.deltaTime;
                Debug.Log("momentum");
            }
            if (direction.magnitude >= 0.1f)
            {
                playerAnimator.SetBool("Walk", true);
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angleSmoothed = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angleSmoothed, 0f);
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                if (jumpingPhase)
                {
                    controller.Move(moveDirection.normalized * playerSpeed * Time.deltaTime + Vector3.up * momentum * Time.deltaTime);
                    Debug.Log("Jump: " + transform.localPosition.y);
                }
                else
                {
                    controller.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
                }
            }
            else if (jumpingPhase)
            {
                controller.Move(Vector3.up * momentum * Time.deltaTime);
            }
            else
            {
                playerAnimator.SetBool("Walk", false);
            }
            if (transform.localPosition.y < 0f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 1f, transform.localPosition.z);
                isGrounded = true;
                Debug.Log("Jumping ended");
                jumpingPhase = false;
            }
        }
    }
    private void Attack()
    {
        //animation
        Collider[] enemiesHit = Physics.OverlapSphere(attackpoint.position, reach, enemyLayer);
        Debug.Log("Attacking "+enemiesHit.Length);

        if (enemiesHit.Length > 0)
        {
            foreach(Collider enemy in enemiesHit)
            {
                enemy.GetComponent<EnemyController>().EnemyHurt(playerDamage);
            }
        }
        Collider[] enemiesRemaining = Physics.OverlapSphere(attackpoint.position, reach, enemyLayer);
        if(enemiesRemaining.Length == 0)
        {
            playerUnderAttack = false;
        }
    }
    void OnDrawGizmosSelected()
    {
        if(attackpoint == null) { return; }
        Gizmos.DrawWireSphere(attackpoint.position, reach);
    }

    private void Hurt(int damage)
    {
        //Animation
        health -= damage;
        if (health <= 0)
        {
            //GameObject.FindWithTag("Player").SetActive(false);
            playerAnimator.SetBool("Dead", true);
            gameOver.SetActive(true);
        }
    }
    public void lootDeadEnemies()
    {
        Debug.Log("Points: "+points);
        points += (int)Random.Range(5, 15);
        if(Random.Range(0,10) > 6)
        {
            Debug.Log("Health: "+health);
            health += (int)Random.Range(10, 30);
        }
        setScore();
    }
    public void setScore()
    {
        if (Score != null)
        {
            Score.text = "Loot: " + points + " gold";
        }
    }
}
