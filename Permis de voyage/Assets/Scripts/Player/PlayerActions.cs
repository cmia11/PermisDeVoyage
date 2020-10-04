using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Vector3 forward = new Vector3(1,0,0);
    private bool push = false;

    void Update()
    {
        // Get current player forward direction
        forward = new Vector3(1,0,0) * Input.GetAxisRaw("Horizontal");

        // Detect push action button press
        push = Input.GetButton("Fire1");
    }

    public float hitboxSize = 0.5f;
    void FixedUpdate()
    {
        // Cast a ray in the direction facing the player to detect movable objects
        // in front of him at hitbox distance.
        // hitbox is small because we want to detect near contact with a movable object
        LayerMask mask = LayerMask.GetMask("Movable");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, forward, hitboxSize, mask);

        // If we are near a movable object
        if (hit.collider != null)
        {
            handleInteractions(hit.collider.gameObject);
        }
    }

    public uint power = 100;
    void handleInteractions(GameObject movableObj)
     {
        
        // Push action
        if (push)
        {
            Rigidbody2D body = movableObj.GetComponent<Rigidbody2D>();
            if (body)
            {
                body.AddForce(forward * power,ForceMode2D.Impulse);
                push = false;
            }
        }
     }
}