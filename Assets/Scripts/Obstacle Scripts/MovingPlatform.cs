using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    [SerializeField]
    private float speed=5f;
    private Vector3 mDirection;
    private Transform mDestination;
    private Rigidbody2D platformRigidbody2D;

    [SerializeField]
    private Transform platform;
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;

    void Start () {
        platformRigidbody2D = platform.GetComponent<Rigidbody2D>();
        platform.position = startPoint.position;
	}
	
	void FixedUpdate () {
        //platform.position = Vector2.Lerp(startPoint.position, endPoint.position, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time / speed, 1f)));
        platformRigidbody2D.MovePosition(Vector2.Lerp(startPoint.position, endPoint.position, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time / speed, 1f))));
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPoint.position, platform.GetComponent<BoxCollider2D>().size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(endPoint.position, platform.GetComponent<BoxCollider2D>().size);
    }
}
