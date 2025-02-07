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
				instance = FindFirstObjectByType<T>();
				if (instance == null)
					Debug.LogWarning("Cannot find " + typeof(T).Name);
			}
			return instance;
		}
	}
}