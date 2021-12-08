using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{   
    [Header("Movement")]
    public float moveSpeed = 0;
    float horizontal = 0;
    float vertical = 0;
    Rigidbody2D rigidbody;
    public Joystick joystick;
    
    [Header("Rotate")]
    float rotateAngle = 0;

    [Header("Status")]
    public float maxHP = 0;
    public float currentHP = 0;
    public float damage = 0;
    public Image HPbar;


    [Header("Combat")]
    public float attackRadius = 0f;
    public float attackDelay = 0f;
    public float timeUntilAttackReadied = 0f;
    public bool attemptedAttack = false;
    public LayerMask enemyLayer = 0;
    public Transform projectile;
    public Transform gunBarrel;
    public Transform attackOrigin;
    Collider2D facingEnemy = null;

    [Header("UI")]
    public int score = 0;
    public GameObject UI;
    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        joystick = FindObjectOfType<Joystick>().GetComponent<Joystick>();

        currentHP = maxHP;
    }

    private void Update() {
        HPUI();
        UpdateScore();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Rotate();
        HandleAttack();

        // FaceToEnemy();
        timeUntilAttackReadied -= Time.deltaTime;
    }

    void Move(){
        // horizontal = Input.GetAxis("Horizontal");
        // vertical = Input.GetAxis("Vertical");
        horizontal = Mathf.RoundToInt(joystick.Horizontal);
        vertical = Mathf.RoundToInt(joystick.Vertical);

        Vector2 diraction = new Vector2(horizontal,vertical).normalized;

        rigidbody.velocity = diraction * moveSpeed;
    }

    void FaceToEnemy(){
        Collider2D[] overlapColliders = Physics2D.OverlapCircleAll(transform.position,attackRadius,enemyLayer);
        if(overlapColliders != null){
            for(int i = 0; i < overlapColliders.Length; i++){
                if(facingEnemy == null){
                    facingEnemy = overlapColliders[i];
                }else if((overlapColliders[i].transform.position - transform.position).magnitude < (facingEnemy.transform.position - transform.position).magnitude){
                    facingEnemy = overlapColliders[i];
                }
            }

            if(facingEnemy != null){
                Vector3 diraction = facingEnemy.transform.position - transform.position;
                float angle = Mathf.Atan2(diraction.y,diraction.x) * Mathf.Rad2Deg;
                rigidbody.rotation = angle;

            }
    
        }
    }

    void Rotate(){
        if(horizontal == 0 && vertical == 1){
            rotateAngle = 0;
        }
        
        if(horizontal == 1 && vertical == 1){
            rotateAngle = -45f;
        }

        if(horizontal == 1 && vertical == 0){
            rotateAngle = -90f;
        }

        if(horizontal == 1 && vertical == -1){
            rotateAngle = -135;
        }

        if(horizontal == 0 && vertical == -1){
            rotateAngle = -180;
        }

        if(horizontal == -1 && vertical == -1){
            rotateAngle = -225f;
        }

        if(horizontal == -1 && vertical == 0){
            rotateAngle = -270f;
        }
        if(horizontal == -1 && vertical == 1){
            rotateAngle = -315f;
        }

        gunBarrel.rotation = Quaternion.Euler(0f,0f,rotateAngle);

        
    }

    public void StartAttack(){
        attemptedAttack = true;
    }
    public void StopAttack(){
        attemptedAttack = false;
    }

    void HandleAttack(){
        if(attemptedAttack && timeUntilAttackReadied <= 0){
            //Do Attack
            Transform _projectile = Instantiate(projectile,attackOrigin.position,attackOrigin.rotation);
            _projectile.GetComponent<Projectile>().damage = damage;
            // attemptedAttack = false;
            timeUntilAttackReadied = attackDelay;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position,attackRadius);
    }

    void HPUI(){
        HPbar.fillAmount = currentHP / maxHP;
    }

    public void ApplyDamage(float amount)
    {
        currentHP -= amount;
    }

    public void Hit(float amount){
        ApplyDamage(amount);

        if(currentHP <= 0){
            UI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }

    void UpdateScore(){
        scoreText.text = "Score : " + score;
    }
}
