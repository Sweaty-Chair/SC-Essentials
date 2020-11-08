using UnityEngine;

public abstract class SingletonMonoBehaviour : MonoBehaviour
{
	// name this after the prefab in the Resources folder.
	public const string resourceName = "Singletons";
 
	// this will be the object containing all components which are singletons.
	static GameObject singletonsObject = null;
 
	protected static class Initializer {
 
		public static readonly int singletonCount;
 
		static Initializer()
		{
			singletonCount = 0;
 
			// load in the prefab from resources.
			var singletonsObjectPrefab = (GameObject)Resources.Load(resourceName, typeof(GameObject));
 
			// see if we loaded/got it.
			if (!singletonsObjectPrefab)
			{
				Debug.LogError("There was no singleton prefab : Resources/" + resourceName);
				return;
			}
 
			// instantiate it
			singletonsObject = (GameObject)Instantiate(singletonsObjectPrefab);
 
			// mark it as do not destroy.
			DontDestroyOnLoad(singletonsObject);
 
			// run through each mono behaviour on the object
			foreach (MonoBehaviour behaviour in singletonsObject.GetComponents<MonoBehaviour>())
			{
				// get the type once.
				var type = behaviour.GetType();
 
				// check that the type is a singleton type.
				if ( typeof(SingletonMonoBehaviour).IsAssignableFrom(type) )
				{
					// call the SetSingletonFromLoad static private.
					typeof(SingletonMonoBehaviour<>)
						.MakeGenericType(type)
						.GetMethod("SetSingletonFromLoad", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
						.Invoke(null, new object[]{ behaviour} );
					++singletonCount;
				}
			}
 
			// send messages optionally
 
			// SingletonAwake is where all singletons have been configured and set. but not neccisarily configured.
			singletonsObjectPrefab.SendMessage("SingletonAwake", SendMessageOptions.DontRequireReceiver);
 
			// SingletonStart is where all singletons have been awoke at minimum
			singletonsObjectPrefab.SendMessage("SingletonStart", SendMessageOptions.DontRequireReceiver);
 
			// optionally add more messages if you need more layers of initialization
		}
 
		// Never call this in a static initializer of a MonoScript or ScriptableObject.
		public static void Bind() {
			// does nothing but ensures the static Initializer is called.
			// should be very low-performance use.
		}
	}
 
	public static GameObject singletonGameObject { get { Initializer.Bind(); return singletonsObject; } }
 
	public static T GetSingleton<T>() where T : SingletonMonoBehaviour<T>
	{
		return SingletonMonoBehaviour<T>.singleton;
	}
 
	// private: Takes a component with given type sees if the type is assinable to SingletonMonoBehaviour return it if it is otherwise null.
	// checks for null
	static SingletonMonoBehaviour SingletonCast(Component component, System.Type type)
	{
		if (typeof(SingletonMonoBehaviour).IsAssignableFrom(type) && component)
		{
			return (SingletonMonoBehaviour)component;
		}
		else
		{
			return null;
		}
	}
 
	// private: Takes a component with unknown type, sees if the typ eis assinable to SingletonMonoBehaviour return it if it is otherwise null.
	// checks for null
	static SingletonMonoBehaviour SingletonCast(Component component)
	{
		if (component && typeof(SingletonMonoBehaviour).IsAssignableFrom(component.GetType()))
			return (SingletonMonoBehaviour)component;
		else
			return null;
	}
 
	// Only for singleton types. Use singletonGameObject.GetComponent for non singleton types
	public static SingletonMonoBehaviour GetSingleton(System.Type type)
	{
		Initializer.Bind();
 
		return singletonsObject ? SingletonCast( singletonsObject.GetComponent(type), type) : null;
	}
 
	// Only for singleton types. Use singletonGameObject.GetComponent for non singleton types
	public static SingletonMonoBehaviour GetSingleton(string type)
	{
		Initializer.Bind();
 
		return singletonsObject ? SingletonCast( singletonsObject.GetComponent(type) ) : null;
	}
 
	public static bool TryGetSingleton<T>(out T singleton) where T : SingletonMonoBehaviour<T>
	{
		singleton = GetSingleton<T>();
		return singleton != null;
	}
 
	public static int singletonCount { get { return Initializer.singletonCount; } }
 
	public static void Ensure() { Initializer.Bind(); }
}
 
// to use this instead of inheriting MonoBehaviour, instead inherit SingletonMonoBehaviour<> with the type your making
public abstract class SingletonMonoBehaviour<T> : SingletonMonoBehaviour where T : SingletonMonoBehaviour<T>
{
	static T _singleton = null;
 
	private static void SetSingletonFromLoad(T singleton)
	{
		if (_singleton)
		{
			Debug.LogWarning("Setting singleton, but already had one:", _singleton);
			Debug.LogWarning(" --> Incoming", singleton);
		}
		_singleton = singleton;
	}
 
	// access the singleton for this type ( readonly )
	public static T singleton { get { Initializer.Bind(); return _singleton; } }
	// optionally one could change this to protected and expose the singleton in the class itself how they would like or not at all.
}
 
public class SingletonEnsure : MonoBehaviour {
	void Awake() { SingletonMonoBehaviour.Ensure(); }
}