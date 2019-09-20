using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingFall : MonoBehaviour
{
    public LayerMask playerMask;

    Rigidbody2D rb;

	bool falling = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 20f, playerMask);

        if (hit)
        {
			//  rb.isKinematic = false;
			falling = true;
        }

		if (falling)
		{
			transform.Translate(Vector2.up * 0.06f);
		}
    }
}
