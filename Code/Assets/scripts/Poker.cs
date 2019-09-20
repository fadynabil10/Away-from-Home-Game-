using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker : MonoBehaviour
{
	Transform home;
	bool move = true;
	bool teleport = true;

	public GameObject newHome;
    // Start is called before the first frame updates

    // Update is called once per frame
    void Update()
    {
		home = GameObject.FindGameObjectWithTag("Home").transform;
		if(home != null && teleport == true)
		{
			if(transform.position.x < home.transform.position.x - 30)
			{
				transform.position = home.transform.position + Vector3.left * 30;
			}
		}
		else
		{
			teleport = true;
		}

		if (move)
		{
			transform.Translate(Vector2.right * .03f);
		}
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Home"))
		{
			move = false;
			collision.GetComponent<Home>().working = false;
		}

		if (collision.CompareTag("Player"))
		{
			collision.GetComponent<Charachter>().Respawn();
		}
	}
}
