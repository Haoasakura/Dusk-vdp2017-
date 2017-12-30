using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour {

    public float offsetX;
    public float offsetY;

    private Vector3 cameraPosition;
    private float newX;
    private float newY;
    private float newZ;
    private float collX;
    private float collY;
    private BoxCollider2D coll;
    private UnityAction saveCameraPosition;
    private UnityAction returnOldCamera;
    private UnityAction returnOldFromEnemy;


    private void Start() {
        coll = GetComponent<BoxCollider2D>();
        collX = coll.size.x;
        collY = coll.size.y;
        ActivateEnemies();
        saveCameraPosition = new UnityAction(SaveCameraPosition);
        returnOldCamera = new UnityAction(ReturnOldCamera);
        returnOldFromEnemy = new UnityAction(ReturnOldFromEnemy);
        EventManager.StartListening("EnemyControlled", saveCameraPosition);
        EventManager.StartListening("EnemyDestroyed", returnOldFromEnemy);
        EventManager.StartListening("PlayerDied", returnOldCamera);
        EventManager.StartListening("PlayerDiedFromFall", returnOldCamera);
        SaveCameraPosition();
    }

    private void ReturnOldFromEnemy()
    {
        StartCoroutine("DontReturnSuddenly");
    }

    IEnumerator DontReturnSuddenly()
    {
        yield return new WaitForSeconds(2);
        SoundManager.Instance.Laugh();
        ReturnOldCamera();
    }

    private void ReturnOldCamera()
    {
        transform.position = cameraPosition;
    }

    private void SaveCameraPosition()
    {
        cameraPosition = transform.position;
    }

    private void OnTriggerExit2D(Collider2D collision) {

        newX = transform.position.x;
        newY = transform.position.y;
        newZ = transform.position.z;

        if ((collision.CompareTag(Tags.player) && !collision.gameObject.GetComponent<Player>().controlling) || (collision.CompareTag(Tags.enemy) && collision.gameObject.GetComponent<EnemyController>().controlled)) {
            Transform current = collision.transform;

            //Traslazioni nel caso il giocatore esca dal collider
            if ((Mathf.Abs(transform.position.x - current.position.x) > collX / 2) && Mathf.Sign(transform.position.x - current.position.x) < 0)
            {
                newX = transform.position.x + collX - offsetX;
            }
            else if ((Mathf.Abs(transform.position.x - current.position.x) > collX / 2) && Mathf.Sign(transform.position.x - current.position.x) > 0)
            {
                newX = transform.position.x - collX + offsetX;
            }
            else if ((Mathf.Abs(transform.position.y - current.position.y) > collY / 2) && Mathf.Sign(transform.position.y - current.position.y) < 0)
            {
                newY = transform.position.y + collY - offsetY;
            }
            else if ((Mathf.Abs(transform.position.y - current.position.y) > collY / 2) && Mathf.Sign(transform.position.y - current.position.y) > 0)
            {
                newY = transform.position.y - collY + offsetY;
            }

            newZ = transform.position.z;

            transform.position= new Vector3 (newX, newY, newZ);
        }

        if ((collision.CompareTag(Tags.player) && !collision.gameObject.GetComponent<Player>().controlling))
        {
            SaveCameraPosition();
        }

        ActivateEnemies();  
    }

    public void ActivateEnemies() {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.enemy)) {
            if (coll.bounds.Contains(enemy.transform.position + new Vector3(0, 0, -10))) {
                if (enemy.gameObject.GetComponent<Animator>().GetBool("Idle")) {
                    enemy.gameObject.GetComponent<Animator>().SetBool("Idle",false);
                }
            }
        }
    }
}
