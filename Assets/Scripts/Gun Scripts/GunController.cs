using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public float TurnSpeed = 20f;
    public Transform lineOfSight;
    public int currentCharge = 50;
    public int maxCharge = 100;
    public LayerMask lightLayer;
    public bool inControl = false;

    private Transform mTransform;
    private EnemyController enemyControlled;
    

	void Start () {
        mTransform = GetComponent<Transform>();
	}
	
	void Update () {
        if (!inControl) {
            //rotazione della pistola
            if (Input.GetButton("Vertical")) {

                float mVertical = Input.GetAxis("Vertical");

                mTransform.Rotate(new Vector3(0f, 0f, mVertical * TurnSpeed * Time.deltaTime));

                //mantiene la sprite dell'arma nel verso giusto
                if (mTransform.rotation.eulerAngles.z % 270 < 90 && mTransform.rotation.eulerAngles.z % 270 > 0) {
                    GetComponent<SpriteRenderer>().flipY = false;
                }
                else {
                    GetComponent<SpriteRenderer>().flipY = true;
                }
            }

            if (Input.GetButtonDown("Fire1") && currentCharge < maxCharge) {

                //Nummero di collisioni che il fucile percepisce
                Collider2D[] pointsOfContact = new Collider2D[25];
                ContactFilter2D contactFilter = new ContactFilter2D();
                contactFilter.useTriggers = true;
                int numberOfContacts = lineOfSight.GetComponent<PolygonCollider2D>().OverlapCollider(contactFilter, pointsOfContact);

                for (int i = 0; i < numberOfContacts; i++) {
                    if (pointsOfContact[i].gameObject != null && pointsOfContact[i].gameObject.CompareTag(Tags.light)) {
                        if (InLineOfSight(pointsOfContact[i]) && !pointsOfContact[i].GetComponent<LightController>().changingStatus)
                            pointsOfContact[i].GetComponent<LightController>().SwitchOnOff(transform);
                        break;
                    }
                    else if (pointsOfContact[i].gameObject != null && pointsOfContact[i].gameObject.CompareTag(Tags.machinery)) {
                        //activate the machinery
                    }
                    else if (pointsOfContact[i].gameObject != null && pointsOfContact[i].gameObject.CompareTag(Tags.enemy)) {
                        enemyControlled = pointsOfContact[i].GetComponent<EnemyController>();
                        enemyControlled.controlled = true;
                        inControl = !inControl;

                    }


                }
            }
        }
	}

    public bool InLineOfSight(Collider2D target) {
        if (target != null) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.transform.position - transform.position),1000f,lightLayer);
            if (hit.collider != null)
                if (hit.collider.gameObject.name == target.gameObject.name) 
                    return true;
        }
        return false;
    }


}
