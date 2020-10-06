using UnityEngine;

public class MovingSphere_Physics : MonoBehaviour
{

	[SerializeField, Range(0f, 100f)]
	float maxSpeed = 10f;

	[SerializeField, Range(0f, 100f)]
	float maxAcceleration = 10f;

	[SerializeField, Range(0f, 1f)]
	float bounciness = 0.5f;

	[SerializeField]
	Rect allowedArea = new Rect(-4.5f, -4.5f, 9f, 9f);

	Vector3 velocity, desiredVelocity;

	Rigidbody body;

    void Awake ()
    {
		body = GetComponent<Rigidbody>();
    }

	void Update()
	{
		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity =
			new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

	}

    private void FixedUpdate()
    {
		velocity = body.velocity;
		float maxSpeedChange = maxAcceleration * Time.deltaTime;
		velocity.x =
			Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		velocity.z =
			Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

		body.velocity = velocity;
	}
}