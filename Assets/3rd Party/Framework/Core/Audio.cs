using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Audio : SceneSingleton<Audio>
{
	private AudioSource[] sources;

	new void Awake ()
	{
		sources = gameObject.GetComponents<AudioSource> ();
		if ( sources.Length == 0 )
			Debug.LogWarning ( "There is no audio source on " + gameObject.name + ". No sound will be played", gameObject );
	}

	void Reawake ()
	{
		Awake ();
	}
	
	internal AudioSource PlaySound ( AudioClip clip, float volume = 1.0f, float delay = 0.0f, float pitch = 1 )
	{
		if ( clip != null )
		{
			if ( pitch == 0 )
				Debug.LogWarning ( "Playing a sound with a pitch at zero will make it play for ever !" );

			for ( int i = 0; i < sources.Length; i++ )
			{
				AudioSource source = sources[i];
				if ( !source.isPlaying )
				{
					source.clip = clip;
					source.volume = volume;
					source.loop = false;
					source.pitch = pitch;
					if ( delay > 0 )
						source.PlayDelayed ( delay );
					else
						source.Play ();

					return source;
				}
			}

			string soundList = "";
			for ( int i = 0; i < sources.Length; i++ )
				soundList += "\n" + sources[i].clip.name;
			Debug.LogWarning ( "There is no free audio source on " + gameObject.name + ". No sound will be played.\nCurrent sounds:\n" + soundList );
		}

		return null;
	}

	internal static void Play ( AudioClipEx clip, AudioSource source )
	{
		if ( clip.Sound != null )
		{
			source.clip = clip.Sound;
			source.volume = clip.Volume;
			source.loop = clip.IsLooping;
			source.pitch = clip.Pitch;
			if ( clip.Delay > 0 )
				source.PlayDelayed ( clip.Delay );
			else
				source.Play ();
		}
	}

	internal static void Play ( AudioClipEx clip, ref AudioSource audioSource )
	{
		if ( audioSource != null )
			audioSource.Stop ();

		audioSource = Play ( clip );
	}

	internal static AudioSource Play ( AudioClipEx clip )
	{
		if ( clip.Sound != null )
		{
			AudioSource audioSource = Audio.Instance.PlaySound ( clip.Sound, clip.Volume, clip.Delay, clip.Pitch );
			if ( audioSource != null )
				audioSource.loop = clip.IsLooping;
			return audioSource;
		}
		else
			return null;
	}

	internal static AudioSource Play ( AudioClip clip, float volume = 1.0f, float delay = 0.0f, float pitch = 1 )
	{
		return Audio.Instance.PlaySound ( clip, volume, delay, pitch );
	}

	internal static AudioSource Play ( AudioClip[] clips, float volume = 1.0f, float delay = 0.0f, float pitch = 1 )
	{
		return Audio.Instance.PlaySound ( clips.GetRandomElement(), volume, delay, pitch );
	}
}
