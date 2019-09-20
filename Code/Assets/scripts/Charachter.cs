using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Controller2D))]
public class Charachter : MonoBehaviour
{

	#region Fields

	// reference to the controller2D & RaycastControl
	private Controller2D controller;

	// input support
	public static Vector2 input { get; private set; }

	// movment support
	private Vector3 velocity;
	[Header("Movment")]
	[SerializeField] private float walkSpeed = 6;
	private float moveSpeed = 6;

	// smoothing the movement support
	private float velocityXSmoothing;
	private float accelerationTimeAirborne = 0.2f;
	private float accelerationTimeGrounded = 0.1f;

	// gravity and ground jumping support
	public static float gravity { get; private set; }
	private float jumpVelocity;
	[Header("Ground Jumping")]
	[SerializeField] private float jumpHeight = 4;
	[SerializeField] private float timeToJumpApex = 0.4f;
	[SerializeField] private int numberOfJumps = 1;
	private int jumpCount;

	// home stuff
	public float startingHealth = 100;
	public float health;
	[SerializeField] private Image healthBar;
	[SerializeField] private Transform spawnPoint;

	SpriteRenderer renderer;
	public bool godMode = false;

	#endregion

	private void Start()
	{
		renderer = GetComponent<SpriteRenderer>();
		// cashe for efficiency
		controller = GetComponent<Controller2D>();

		// gravity calculations
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		// speed control
		moveSpeed = walkSpeed;

		// home
		health = startingHealth;
	}

	private void Update()
	{
		// reset jumping
		if (controller.collisions.below && velocity.y == 0)
		{
			jumpCount = numberOfJumps;
		}

		moveSpeed = walkSpeed;

		if(health <= 0)
		{
			Respawn();
		}

		if (godMode)
		{
			health = startingHealth;
		}
		healthBar.fillAmount = health / startingHealth;

	}

	private void FixedUpdate()
	{
		// calclate the final movment in the current update frame
		CalculateVelocity(input);

		// apply the movment
		controller.Move(velocity * Time.deltaTime, input);

		// prevent gravity accumilation
		if (controller.collisions.below || controller.collisions.above)
		{
			velocity.y = 0;
		}
	}

	#region Private Functions

	/// <summary>
	/// Calculates the next move of the player
	/// </summary>
	private void CalculateVelocity(Vector2 input)
	{
		// smooth xAxis velocity
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

		// apply gravity on the yAxis
		velocity.y += gravity * Time.fixedDeltaTime;
	}

	#endregion

	#region Public Funtions

	/// <summary>
	/// Recieves the input from the manager
	/// </summary>
	/// <param name="input"></param>
	public void SetDirectionalInput(Vector2 input)
	{
		Charachter.input = input;
	}

	/// <summary>
	/// Perform a jump by the player
	/// </summary>
	public void Jump()
	{
		if (controller.collisions.below)
		{
			// normal ground jump
			velocity.y = jumpVelocity;
			jumpCount--;
			StartCoroutine(sound());
			AudioManager.Play(AudioClipName.Jump);
		}
		else if (jumpCount > 0)
		{
			// double air jump
			velocity.y = jumpVelocity;
			jumpCount--;
		}
	}

	public void Respawn()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name);
	//spawnPoint = GameObject.FindGameObjectWithTag("Home").transform;
	//if(spawnPoint!= null)
	//{
	//	transform.position = spawnPoint.position;
	//}
	}

	IEnumerator sound()
	{
		GameAudioSource.audioSource.volume /= 5;
		yield return new WaitForSeconds(0.2f);
		GameAudioSource.audioSource.volume *= 5;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("End"))
		{
			godMode = true;
		}
	}

	#endregion

}
