using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    public bool canJump=true;
    private Player player;
    private GunController gunController2D;

    private void Start()
    {
        player = GetComponent<Player>();
        gunController2D = transform.GetComponentInChildren<GunController>();
    }

    private void Update()
    {
        if (!player.controlling) {
            Vector2 directionalInput;
            if (gunController2D.isLocked)
            {
                directionalInput = new Vector2(0f, 0f);
                player.SetDirectionalInput(directionalInput);
            }
            else
            {
                directionalInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                player.SetDirectionalInput(directionalInput);
                if (canJump) {
                    if (Input.GetButtonDown("Jump")) {
                        player.OnJumpInputDown();
                    }

                    if (Input.GetButtonUp("Jump")) {
                        player.OnJumpInputUp();
                    }
                }
            }
        }
    }
}
