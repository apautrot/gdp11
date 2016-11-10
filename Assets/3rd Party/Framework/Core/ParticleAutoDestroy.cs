using UnityEngine;
using System.Collections;

public class ParticleAutoDestroy : MonoBehaviour
{
	public float StopEmissionAfter = -1;
	private bool hadParticles;

	void FixedUpdate ()
	{
		if ( StopEmissionAfter > 0 )
		{
			StopEmissionAfter -= Time.fixedDeltaTime;
			if ( StopEmissionAfter <= 0 )
				GetComponent<ParticleSystem>().Stop ();
		}

		hadParticles |= GetComponent<ParticleSystem>().particleCount > 0;
		if ( hadParticles && ( GetComponent<ParticleSystem>().particleCount == 0 ) )
			gameObject.DestroySelf ();
	}
}
