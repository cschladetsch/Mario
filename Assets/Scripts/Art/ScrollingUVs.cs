using UnityEngine;

internal class ScrollingUVs : MonoBehaviour
{
	public Vector2 speed = Vector2.zero;

	private void Update()
	{
		Vector2 offset = renderer.material.mainTextureOffset + speed*Time.deltaTime;
		offset.x = offset.x%1.0f;
		offset.y = offset.y%1.0f;
		renderer.material.mainTextureOffset = offset;
	}
}