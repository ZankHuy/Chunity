using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAudio : MonoBehaviour
{
    void Start()
    {
        GetComponent<ChuckSubInstance>().RunCode(@"
			TriOsc spatialOsc => dac;
			while( true )
			{
				Math.random2f( 300, 1000 ) => spatialOsc.freq;
				50::ms => now;
			}
		");
    }
}
