using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
	private static T instance;
	public static T Instance
	{ 
		get
		{
			if ( instance == null )
			{
				instance = FindObjectOfType<T> ();
				if (instance == null)
					Debug.LogError("Cannot find " + typeof(T).Name);
			}
			return instance;
		}
	}
}