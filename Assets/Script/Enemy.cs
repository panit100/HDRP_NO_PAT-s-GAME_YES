using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour, IDamageable
{
    Rigidbody2D rigidbody;
    
    [Header("PathFinding")]
    public AIPath aIPath;
    public AIDestinationSetter aIDestinationSetter;

    [Header("Status")]
    public float maxHP = 10f;
    public float currentHP = 10f;
    public float currentSpeed = 0f;
    public float moveSpeed = 0f;

    [Header("Combat")]

    public float damage = 0f;
    public float attackRadius = 0f;
    public float waitBeforeAttack = 2f;
    public bool attemptedAttack = false;
    public LayerMask playerLayer = 0;
    public Transform attackOrigin;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().GetComponent<Player>();

        aIDestinationSetter.target = player.transform;

        currentHP = maxHP;
        currentSpeed = moveSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ReachedToPlayer();
        FaceToPlayer();
    }

    public void ApplyDamage(float amount)
    {
        Hit(amount);
        if(currentHP <= 0){
            Die();
        }
    }

    void Hit(float amount){
        currentHP -= amount;
    }

    public void Die(){
        player.score++;
        Destroy(this.gameObject);
    }

    void ReachedToPlayer(){
        aIPath.maxSpeed = currentSpeed;

        if(aIPath.whenCloseToDestination == CloseToDestinationMode.Stop && aIPath.reachedEndOfPath){
            currentSpeed = 0;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack(){
        if(!attemptedAttack){
            attemptedAttack = true;
            yield return new WaitForSeconds(waitBeforeAttack);
            if(attemptedAttack){
                Collider2D overlapCollider = Physics2D.OverlapCircle(attackOrigin.position,attackRadius,playerLayer);
                if(overlapCollider != null){
                    Player player = overlapCollider.GetComponent<Player>();
                    if(player != null){
                        Debug.Log("Attack by Enemy" + Time.time);
                        player.Hit(damage);
                    }
                }
            }
            currentSpeed = moveSpeed;
            attemptedAttack = false; 
        }
    }

    void FaceToPlayer(){
        if(attemptedAttack){
            return;
        }

        Vector3 diraction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(diraction.y,diraction.x) * Mathf.Rad2Deg;
        rigidbody.rotation = angle;
    }

    private void OnDrawGizmosSelected() {
        if(attackOrigin != null){
            Gizmos.DrawWireSphere(attackOrigin.position,attackRadius);
        }
    }
}
