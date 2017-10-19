using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public float TurnSpeed = 20f;
    public Transform lineOfSight;
    public int currentCharge = 50;
    public int maxCharge = 100;

    private Transform mTransform;

	void Start () {
        mTransform = GetComponent<Transform>();
	}
	
	void Update () {
		
        //rotazione della pistola
        if(Input.GetButton("Vertical")) {

            float mVertical = Input.GetAxis("Vertical");

            mTransform.Rotate(new Vector3(0f,0f,mVertical*TurnSpeed*Time.deltaTime));

            //mantiene la sprite dell'arma nel verso giusto
            if(mTransform.rotation.eulerAngles.z%270<90 && mTransform.rotation.eulerAngles.z%270 > 0) {
                GetComponent<SpriteRenderer>().flipY = false;
            }
            else {
                GetComponent<SpriteRenderer>().flipY = true;
            }
        }

        if (Input.GetButtonDown("Fire1")) {

            //Nummero di collisioni che il fucile percepisce
            Collider2D[] pointsOfContact = new Collider2D[25];
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;
            int numberOfContacts = lineOfSight.GetComponent<PolygonCollider2D>().OverlapCollider(contactFilter, pointsOfContact);

            for (int i = 0; i < numberOfContacts; i++) {
                if (pointsOfContact[i].gameObject != null && pointsOfContact[i].gameObject.CompareTag(Tags.light)) {
                    pointsOfContact[i].GetComponent<LightController>().SwitchOnOff();
                    break;
                }
                    

            }
        }
	}
}
