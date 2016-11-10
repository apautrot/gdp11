using UnityEngine;



[System.Serializable]
public class AudioClipEx
{
	public AudioClip Sound;
	public float Volume = 1;
	public float Pitch = 1;
	public bool IsLooping;
	public float Delay = 0;

	public AudioClipEx ()
	{
		Volume = 1;
		Pitch = 1;
	}
}



public class AudioClipExAttribute : PropertyAttribute
{
	public AudioClipExAttribute ()
	{
	}
}