using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneManager : MonoBehaviour
{
    public float waitUntilWaveStart = 3f;
    public List<Transform> Enemys = new List<Transform>();
    public int amount = 0;
    public bool startSpawn = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCoroutine(Wave());
    }

    //wait3Sec and random spawn
    IEnumerator Wave(){
        yield return new WaitForSeconds(waitUntilWaveStart);

        if(!startSpawn){
            amount += Random.Range(1,3);
            startSpawn =  true;
            for(int i = 0; i < amount; i++){
                StartCoroutine(CreateEnemy());
            }
        }
    }

    public Vector2 SpawnPoint{
        get{
            Vector2 p;
            p.x = Random.Range(-0.5f,0.5f);
            p.y = Random.Range(-0.5f,0.5f);
            return transform.TransformPoint(p);
        }
    }

    IEnumerator CreateEnemy(){
        float t = Random.Range(2f,5f);
        yield return new WaitForSeconds(t);
        int _enemy = Random.Range(0,Enemys.Count);
        Transform instance = Instantiate(Enemys[_enemy]);
        instance.localPosition = SpawnPoint;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero,Vector3.one);
    }
}
