using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
	Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}
	// Update is called once per frame
	void Update()
    {

		if (Charachter.input.x == -1)
		{
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else
		{
			transform.localScale = new Vector3(1, 1, 1);
		}

		Animations(Charachter.input.x);
	}

	

	private void Animations(float directionalInput)
	{
		animator.SetFloat("Blend", Mathf.Abs(directionalInput));
	}
}
