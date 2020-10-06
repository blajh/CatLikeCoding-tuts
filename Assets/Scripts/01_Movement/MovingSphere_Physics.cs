using UnityEngine;

public class MovingSphere_Physics : MonoBehaviour
{

	[SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
	[SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;
	[SerializeField, Range(0F, 10F)] float jumpHeight = 2f;
	Vector3 velocity, desiredVelocity;
	Rigidbody body;
	bool desiredJump;
	bool onGround;

    void Awake () {
		body = GetComponent<Rigidbody>();
    }

	void Update() {
		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity =
			new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
		desiredJump |= Input.GetButtonDown("Jump");
	}

    private void FixedUpdate() {
		velocity = body.velocity;
		float maxSpeedChange = maxAcceleration * Time.deltaTime;
		velocity.x =
			Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		velocity.z =
			Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        if (desiredJump) {
			Jump();
			desiredJump = false;
        }

		body.velocity = velocity;
		onGround = false;
	}

    void Jump(){
        if (onGround) { 
		velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
		}
	}

    void OnCollisionEnter(Collision collision) {
		EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
		EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision) {
        for (int i = 0; collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f;
		}
    }
}