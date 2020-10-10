using UnityEngine;

public class MovingSphere_Physics : MonoBehaviour {
	[SerializeField, Range(0f, 100f)] float maxSpeed = 10f, maxSnapSpeed = 100f;
	[SerializeField, Range(0f, 100f)] float maxAcceleration = 10f, maxAirAcceleration = 1f;
	[SerializeField, Range(0f, 10f)] float jumpHeight = 2f;
	[SerializeField, Range(0, 5)] int maxAirJumps = 0;
	[SerializeField, Range(0f, 90f)] float maxGroundAngle = 40f, maxStairsAngle = 50f;
	[SerializeField, Min(0f)] float probeDistance = 1f;
	[SerializeField] LayerMask probeMask = -1, stairsMask = -1;

	Vector3 velocity, desiredVelocity, contactNormal, steepNormal;
	Rigidbody body;
	float minGroundDotProduct, minStairsDotProduct;
	int jumpPhase, groundContactCount, stepsSinceLastGrounded, stepsSinceLastJump, steepContactCount;
	bool desiredJump;
	bool OnGround => groundContactCount > 0;
	bool OnSteep => steepContactCount > 0;

	void OnValidate() {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
	}

	void Awake() {
		body = GetComponent<Rigidbody>();
		OnValidate();
	}

	void Update() {
		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		desiredVelocity =
			new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
		desiredJump |= Input.GetButtonDown("Jump");

		GetComponent<Renderer>().material.SetColor(
			"_Color", OnGround ? Color.black : Color.white
		);
	}

	private void FixedUpdate() {
		UpdateState();
		AdjustVelocity();

		if (desiredJump) {
			Jump();
			desiredJump = false;
		}

		body.velocity = velocity;
		ClearState();
	}

	void ClearState() {
		groundContactCount = steepContactCount = 0;
		contactNormal = steepNormal = Vector3.zero;
	}

	void UpdateState() {
		stepsSinceLastGrounded += 1;
		stepsSinceLastJump += 1;
		velocity = body.velocity;
		if (OnGround || SnapToGround() || CheckSteepContacts()) {
			stepsSinceLastGrounded = 0;
			jumpPhase = 0;
			if (groundContactCount > 1) {
				contactNormal.Normalize();
			}
		}
		else {
			contactNormal = Vector3.up;
		}
	}

	void Jump() {
		if (OnGround || jumpPhase < maxAirJumps) {
			stepsSinceLastJump = 0;
			jumpPhase += 1;
			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
			float alignedSpeed = Vector3.Dot(velocity, contactNormal);
			if (alignedSpeed > 0f) {
				jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
			}
			velocity += contactNormal * jumpSpeed;
		}
	}

	void OnCollisionEnter(Collision collision) {
		EvaluateCollision(collision);
	}

	void OnCollisionStay(Collision collision) {
		EvaluateCollision(collision);
	}

	void EvaluateCollision(Collision collision) {
		float minDot = GetMinDot(collision.gameObject.layer);
		for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
			if (normal.y >= minDot) {
				groundContactCount += 1;
				contactNormal += normal;
			}
			else if (normal.y > -0.01f) {
				steepContactCount += 1;
				steepNormal += normal;
			}
		}
	}

	Vector3 ProjectOnContactPlane(Vector3 vector) {
		return vector - contactNormal * Vector3.Dot(vector, contactNormal);
	}

	void AdjustVelocity() {
		Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

		float currentX = Vector3.Dot(velocity.normalized, xAxis);
		float currentZ = Vector3.Dot(velocity.normalized, zAxis);

		float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX =
			Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		float newZ =
			Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
	}

	bool SnapToGround() {
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
			return false;
		}
		float speed = velocity.magnitude;
		if (speed > maxSnapSpeed) {
			return false;
		}
		if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit,
			probeDistance, probeMask)) {
			return false;
		}
		if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer)) {
			return false;
		}

		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);

		if (dot > 0f) {
			velocity = (velocity - hit.normal * dot).normalized * speed;
		}
		return true;
	}

	float GetMinDot(int layer) {
		return (stairsMask & (1 << layer)) == 0 ?
			minGroundDotProduct : minStairsDotProduct;
	}

	bool CheckSteepContacts() {
		if (steepContactCount > 1) {
			steepNormal.Normalize();
			if (steepNormal.y >= minGroundDotProduct) {
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

}