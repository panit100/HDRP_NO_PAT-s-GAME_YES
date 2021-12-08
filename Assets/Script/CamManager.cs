using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamManager : MonoBehaviour
{
    public SpawnZoneManager spawnZoneManager;
    public Transform warpPos;
    public GameObject blackScreen;
    public GameObject playerScreen;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            if(FindObjectOfType<Enemy>() == null){
                StartCoroutine(SetCamera(0,true));
                StartCoroutine(SetCamera(1f,false));
                other.transform.position = warpPos.position;
                spawnZoneManager.startSpawn = false;
            }
        }
    }

    IEnumerator SetCamera(float time,bool _bool){
        yield return new WaitForSeconds(time);
        blackScreen.SetActive(_bool);
        playerScreen.SetActive(!_bool);
    }
}
