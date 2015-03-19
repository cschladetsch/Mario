using UnityEngine;
using System.Collections;

public abstract class Effect : MonoBehaviour
{
	/// <summary>
	/// If true, particle systems are moved into world space while playing
	/// </summary>
	public bool MoveToWorld;

	public bool GoingUp = true;

	public bool GoingDown = true;

	private bool _active;

	public abstract bool Triggered();

	protected Player _player;

	private void Awake()
	{
		// de-activate all children
		_active = true;
		ActivateChildren(false);
		_player = FindObjectOfType<Player>();
	}

	private void Start()
	{
	}

	public bool ShouldShow()
	{
		var v = _player.rigidbody2D.velocity.y;
		if (v <= 0 && !GoingDown)
			return false;
		if (v > 0 && !GoingUp)
			return false;

		return Triggered();
	}

	public void UpdateEffect()
	{
		var show = ShouldShow();
		if (show && !_active)
			ActivateChildren(true);
		else if (!show && _active)
			ActivateChildren(false);
	}

	protected void DebugLog(string text)
	{
		//Debug.Log(text);
	}

	private void ActivateChildren(bool activate)
	{
		if (activate == _active)
			return;

		foreach (Transform tr in transform)
		{
			Activate(tr.gameObject, activate);
		}

		_active = activate;
	}

	public void Activate(bool activate)
	{
		if (activate == _active)
			return;

		ActivateChildren(activate);
	}

	private void Activate(GameObject child, bool activate)
	{
		if (child == null)
		{
			Debug.LogError("Effect.Activate: null child");
			return;
		}

		DebugLog("Activating " + child.name + " " + activate + " " + name);

		// if the child is to be activated and has a particle system,
		// then play it.
		var ps = child.GetComponent<ParticleSystem>();
		if (ps)
		{
			if (activate)
			{
				DebugLog("Starting PFX " + ps.name);
				StartCoroutine(PlayParticleSystem(ps));
			}
			else
			{
				DebugLog("Stopping PFX " + ps.name + " child.active: " + child.activeSelf);
				if (!child.activeSelf)
					child.SetActive(true);

				ps.Stop(true);
			}

			return;
		}

		SpawnGameobject spawner = null;
		if (activate)
			spawner = child.GetComponent<SpawnGameobject>();

		child.SetActive(activate);

		if (spawner && spawner.Instance)
			Activate(spawner.Instance, true);

		if (activate && child.audio)
		{
			//Debug.Log("Playing audio " + child.audio.name);
			AudioSource source = null;
			foreach (var c in GetComponents<AudioSource>())
			{
				if (!c.isPlaying)
				{
					source = c;
					break;
				}
			}

			if (!source)
				source = gameObject.AddComponent<AudioSource>();
			source.clip = child.audio.clip;
			source.Play();
		}
	}

	private bool _stop;

	private IEnumerator PlayParticleSystem(ParticleSystem ps)
	{
		//Debug.Log("Playing " + ps.name);
		ps.Emit(1);

		if (!MoveToWorld)
			yield break;

		// move to outside player object, so the particle system doesn't move with player
		ps.gameObject.transform.parent = _player.EmittingParticleSystems.transform;
		ps.gameObject.transform.position = _player.transform.position;

		// wait for play to finish, or we are stopped
		float curr = 0;
		while (curr < ps.duration && !_stop)
		{
			curr += Time.deltaTime;
			yield return 0;
		}

		ps.Clear();
		ps.Stop();

		ps.gameObject.transform.parent = transform;
	}

	public void Reset()
	{
		DebugLog("Resetting Activator " + name);
		_stop = true;
		_active = true;
		ActivateChildren(false);
	}
}

public abstract class ScalarDependantEffect : Effect
{
	public float Value; // { get; }
}