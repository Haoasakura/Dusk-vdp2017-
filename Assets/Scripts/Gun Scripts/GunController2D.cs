using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController2D : MonoBehaviour
{

    public float TurnSpeed = 20f;
    public int currentCharge = 50;
    public int maxCharge = 100;
    public bool controlling = false;
    public bool isLocked = false;

    [HideInInspector]
    public Transform mTarget;
    public Transform barrel;
    public Transform laserDirection;
    public Transform line;
    public LayerMask gunLayer;
    public LayerMask untraversableLayers;

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

        RaycastHit2D hit = Physics2D.Linecast(barrel.position, laserDirection.position, untraversableLayers);
        if (hit.collider != null && !hit.collider.gameObject.layer.Equals(9))
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

            float a = Input.GetAxis("HorizontalGun");
            float b = Input.GetAxis("VerticalGun");
            float c = Mathf.Sqrt(Mathf.Pow(a,2) + Mathf.Pow(b,2));
            //rotazione della pistola
            if (Mathf.Abs(c) > 0.9 && !isLocked)
            {
                float angleRot = - Mathf.Sign(b)*Mathf.Rad2Deg*Mathf.Acos(a / c);
          
                mTransform.rotation = Quaternion.Euler(0f, 0f, angleRot);

                //mantiene la sprite dell'arma nel verso giusto
                if (mTransform.rotation.eulerAngles.z % 270 < 90 && mTransform.rotation.eulerAngles.z % 270 > 0)
                {
                    GetComponent<SpriteRenderer>().flipY = false;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = false;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipY = true;
                    transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipY = true;
                }
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
                            {
                                mTarget.GetComponent<LightController2D>().SwitchOnOff(transform);
                                isLocked = true;
                            }
                    }
                    else if (mTarget.CompareTag(Tags.machinery))
                    {
                        MachineryController currentMachinery = mTarget.GetComponent<MachineryController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && !currentMachinery.changingStatus)
                            if (currentCharge >= currentMachinery.powerCharge || currentMachinery.powered)
                            {
                                currentMachinery.SwitchOnOff(transform);
                                isLocked = true;
                            }
                    }
                    else if (mTarget.CompareTag(Tags.enemy))
                    {
                        enemyControlled = mTarget.GetComponent<EnemyController>();
                        if (InLineOfSight(mTarget.GetComponent<Collider2D>()) && currentCharge >= enemyControlled.controlCost)
                        {
                            enemyControlled.ControlledOnOff(transform);
                            isLocked = true;
                        }
                    }
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                isLocked = false;
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
