using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController2D : MonoBehaviour
{

    public float TurnSpeed = 20f;
    public int currentCharge = 50;
    public int maxCharge = 100;
    public bool controlling = false;

    [HideInInspector]
    public Transform mTarget;
    public Transform barrel;
    public Transform laserDirection;
    public Transform line;
    public LayerMask gunLayer;

    private Player player;
    private Transform mTransform;
    private EnemyController enemyControlled;
    private LineRenderer mLineRenderer;

    void Start()
    {
        player = GetComponentInParent<Player>();
        mTransform = GetComponent<Transform>();
        mLineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {

        line.GetComponent<LineRenderer>().SetPosition(0, barrel.position);

        RaycastHit2D hit = Physics2D.Linecast(barrel.position, laserDirection.position/*, gunLayer*/);
        if (hit.collider != null)
        {
            line.GetComponent<LineRenderer>().SetPosition(1, hit.point);
            mTarget = hit.transform;
        }
        else
        {
            line.GetComponent<LineRenderer>().SetPosition(1, laserDirection.position);
            mTarget = null;
        }

        if (!player.controlling)
        {
            //rotazione della pistola
            if (Input.GetButton("Vertical"))
            {
                float mVertical = Input.GetAxis("Vertical");

                mTransform.Rotate(new Vector3(0f, 0f, mVertical * TurnSpeed * Time.deltaTime));

                //mantiene la sprite dell'arma nel verso giusto
                if (mTransform.rotation.eulerAngles.z % 270 < 90 && mTransform.rotation.eulerAngles.z % 270 > 0)
                    GetComponent<SpriteRenderer>().flipY = false;
                else
                    GetComponent<SpriteRenderer>().flipY = true;
            }

            if (Input.GetButtonDown("Fire1") && currentCharge < maxCharge)
            {
                if (mTarget != null)
                {
                    if (mTarget.CompareTag(Tags.light))
                    {
                        LightController2D currentLight = mTarget.GetComponent<LightController2D>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentLight.changingStatus)
                            if (currentCharge >= currentLight.lightCharge || currentLight.lightStatus)
                                mTarget.GetComponent<LightController2D>().SwitchOnOff(transform);
                    }
                    else if (mTarget.CompareTag(Tags.machinery))
                    {
                        MachineryController currentMachinery = mTarget.GetComponent<MachineryController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentMachinery.changingStatus)
                            if (currentCharge >= currentMachinery.powerCharge || currentMachinery.powered)
                                currentMachinery.SwitchOnOff(transform);
                    }
                    else if (mTarget.CompareTag(Tags.enemy))
                    {
                        enemyControlled = mTarget.GetComponent<EnemyController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && currentCharge >= enemyControlled.controlCost)
                            enemyControlled.ControlledOnOff(transform);
                    }
                }
            }
        }
    }

    public bool InLineOfSight(Collider2D target)
    {
        if (target != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(barrel.position, (target.transform.position - transform.position), 1000f, gunLayer);
            if (hit.collider != null && hit.collider.gameObject.name == target.gameObject.name)
                return true;
        }
        return false;
    }
}
