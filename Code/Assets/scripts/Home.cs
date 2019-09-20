using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
	// 
	Collider2D colliderPlayer;
	private float radius = 5;
	public LayerMask playerMask;
	SpriteRenderer renderer;
	public Sprite off;
	public Sprite on;

	// player stuff
	float standardDmg = 0.15f;
	float currentDamage;

	public bool working = true;

	// Start is called before the first frame update
	void Start()
	{
		currentDamage = standardDmg;
		renderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		colliderPlayer = Physics2D.OverlapCircle(transform.position, radius, playerMask);
		if(colliderPlayer != null && working)
		{
			renderer.sprite = on;
			Charachter player = colliderPlayer.GetComponent<Charachter>();
			player.health = player.startingHealth;
		}
		else
		{
			renderer.sprite = off;
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<Charachter>().health -= currentDamage;
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, 5);
	}
}
