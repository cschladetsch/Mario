using UnityEngine;

public class HasWorld : MonoBehaviour
{
	protected World World;
	protected Player Player;
	public bool Paused;

	void Awake()
	{
		World = FindObjectOfType<World>();
		Player = FindObjectOfType<Player>();

		Construct();
	}

	void Start()
	{
		Begin();
	}

	protected virtual void Construct()
	{
	}

	protected virtual void Begin()
	{
	}
}


