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
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

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
