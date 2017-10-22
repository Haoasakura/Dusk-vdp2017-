using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public Transform endPosition;
    public float secondsForOneLength = 5f;
    public float mSpeed = 10f;
    public bool controlled = false;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!controlled) {
            float lastX = transform.position.x;
            transform.position = Vector3.Lerp(startPosition, endPosition.position, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time / secondsForOneLength, 1f)));
            if (transform.position.x > lastX)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }
        else {
           float mHorizontal = Input.GetAxisRaw("Horizontal");
           float mVertical = Input.GetAxisRaw("Vertical");

            GetComponent<Rigidbody2D>().velocity = new Vector2(mHorizontal*mSpeed,0f);

            if(Input.GetKey(KeyCode.F)) {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy() {
        if(controlled) {
            GameObject.FindGameObjectWithTag(Tags.player).GetComponent<GunController>().inControl = false;
        }
    }
}
