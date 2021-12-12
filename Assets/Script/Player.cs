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
    

    [Header("SwipeDetect")]
    public bool isSpecialAttackOn = false;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public Vector2 direction2D;
    float startTime;
    float endTime;
    public float dashSpeed = 0;
    public float dashTime = 0;
    public float startDashTime = 0;
    public float specialDelay = 0f;
    public float timeUntilSpecialReadied = 0f;
    public float SpecialTime = 0;
    public bool isDash = false;
    InputManager inputManager;

    private void Awake() {
        inputManager = InputManager.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        joystick = FindObjectOfType<Joystick>().GetComponent<Joystick>();

        currentHP = maxHP;
        dashTime = startDashTime;
    }

    private void Update() {
        HPUI();
        UpdateScore();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isSpecialAttackOn){
            Move();
            Rotate();
            HandleAttack();

        }else{
            UpdateSpecial();
        }

        // FaceToEnemy();
        timeUntilAttackReadied -= Time.deltaTime;
        timeUntilSpecialReadied -= Time.deltaTime;

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

    //-------SpacialAttack--------
    private void OnEnable() {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable() {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    void SwipeStart(Vector2 position, float time){
        startPosition = position;
        startTime = time;
    }

    void SwipeEnd(Vector2 position, float time){
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    void DetectSwipe(){
        float distance =  Vector3.Distance(startPosition,endPosition);
        float totalTime = endTime - startTime;
        if(isSpecialAttackOn){
            Debug.DrawLine(startPosition,endPosition,Color.red,5f);
            Vector3 direction3D = endPosition - startPosition;
            Vector2 direction2D = new Vector2(direction3D.x,direction3D.y).normalized;
            HandleSpecial(direction2D);
        }
    }


    void HandleSpecial(Vector2 dir){
        if(!isDash){
            rigidbody.velocity = dir * dashSpeed;
            isDash = true;
        }
    }

    void UpdateSpecial(){

        if(isDash){
            if(dashTime <= 0){
                isDash = false;
                dashTime = startDashTime;
                rigidbody.velocity = Vector2.zero;
            }else{
                dashTime -= Time.deltaTime;
        }
        }
        
    }

    public void Special(){
        if(timeUntilSpecialReadied <= 0){
            TurnOnSpecial();
        }
    }

    void TurnOnSpecial(){
        isSpecialAttackOn = true;
        Invoke("TurnOffSpecial",SpecialTime);
    }
    void TurnOffSpecial(){
        isSpecialAttackOn = false;
        timeUntilSpecialReadied = specialDelay;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(isSpecialAttackOn){
            if(other.gameObject.CompareTag("Enemy")){
                other.gameObject.GetComponent<Enemy>().Die();
            }
        }
    }
    //----------------------------
}
