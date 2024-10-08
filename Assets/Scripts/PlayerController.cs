using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

	public float speed;
	public Text countText;
	public Text winText;

	private Rigidbody rb;
	private int count;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		count = 0;
		SetCountText();
		winText.text = "";

		GetComponent<ChuckSubInstance>().RunCode(@"
		SinOsc foo => dac;
		while( true )
		{
			Math.random2f( 300, 1000 ) => foo.freq;
			100::ms => now;
		}
	");
	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

		rb.AddForce(movement * speed);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Pick Up"))
		{
			other.gameObject.SetActive(false);
			count = count + 1;
			SetCountText();

			GetComponent<ChuckSubInstance>().RunCode(string.Format(@"
			SinOsc foo => dac;
			repeat( {0} )
			{{
				Math.random2f( 300, 1000 ) => foo.freq;
				100::ms => now;
			}}
		", count));
		}

	}

	void SetCountText()
	{
		countText.text = "Count: " + count.ToString();
		if (count >= 12)
		{
			winText.text = "You Win!";
		}
	}
	void OnCollisionEnter(Collision collision)
	{
        // map and clamp from [0, 16] to [0, 1]
        float intensity = Mathf.Clamp01(collision.relativeVelocity.magnitude / 16);

        // square it to make the ramp upward more dramatic
        intensity = intensity * intensity;

        GetComponent<ChuckSubInstance>().RunCode(string.Format(@"
		SndBuf impactBuf => dac;
		me.dir() + ""impact.wav"" => impactBuf.read;

		// start at the beginning of the clip
		0 => impactBuf.pos;
	
		// set rate: least intense is fast, most intense is slow; range 0.4 to 1.6
		1.5 - {0} + Math.random2f( -0.1, 0.1 ) => impactBuf.rate;

		// set gain: least intense is quiet, most intense is loud; range 0.05 to 1
		0.05 + 0.95 * {0} => impactBuf.gain;

		// pass time so that the file plays
		impactBuf.length() / impactBuf.rate() => now;

	", intensity));
    }
}