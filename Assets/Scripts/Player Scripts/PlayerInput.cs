using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    public bool canJump=true;
    private Player player;


    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!player.controlling) {
            Vector2 directionalInput;
            if (Input.GetButton("Fire1"))
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
