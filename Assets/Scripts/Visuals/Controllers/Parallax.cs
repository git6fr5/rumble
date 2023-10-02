using UnityEngine;
using System.Collections;
using Game = Platformer.Management.GameManager;

// For usage apply the script directly to the element you wish to apply parallaxing
// Based on Brackeys 2D parallaxing script http://brackeys.com/
public class Parallax : MonoBehaviour {
	Transform cam; // Camera reference (of its transform)
	Vector3 previousCamPos;

	[Range(0f, 1f)]
	public float parallaxX = 0f; // Distance of the item (z-index based) 

	[Range(0f, 1f)]
	public float parallaxY = 0f;

	// public float smoothingX = 1f; // Smoothing factor of parrallax effect
	// public float smoothingY = 1f;

	void Awake () {
		// cam = Camera.main.transform;
		cam = Game.Visuals.Camera.transform;
		previousCamPos = cam.position;
	}
	
	void FixedUpdate () {
		Vector3 parallax = cam.position - previousCamPos;
		parallax.x *= parallaxX;
		parallax.y *= parallaxY;
		Vector3 targetPos = new Vector3(transform.position.x + parallax.x, transform.position.y + parallax.y, transform.position.z);
		transform.position = targetPos;
		previousCamPos = cam.position;

	}
}