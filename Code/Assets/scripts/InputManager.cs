using UnityEngine;

[RequireComponent(typeof(Charachter))]
public class InputManager : MonoBehaviour
{

	#region Fields

	private Charachter charachter;
	private Animator animator;

	#endregion

	private void Start()
	{
		charachter = GetComponent<Charachter>();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		// check for input left and right and process it to the player
		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		charachter.SetDirectionalInput(directionalInput);

		// check for jumping or thrusting when using jetpack
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (charachter.enabled)
			{
				charachter.Jump();
			}
		}

		// animation control
		//Animations(directionalInput);
	}

	private void Animations(Vector2 directionalInput)
	{
		animator.SetFloat("Blend", Mathf.Abs(directionalInput.x));
	}
}
