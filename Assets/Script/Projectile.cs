using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public float bulletSpeed = 10f;
    public float damage = 1f;
    public int enemyLayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        Invoke("DestroyBullet",10f);
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = -transform.right * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == enemyLayer){
            IDamageable enemy = other.GetComponent<IDamageable>();
            if(enemy != null){
                enemy.ApplyDamage(damage);
            }

            Destroy(this.gameObject);
        }
    }

    void DestroyBullet(){
        Destroy(this.gameObject);
    }
}
