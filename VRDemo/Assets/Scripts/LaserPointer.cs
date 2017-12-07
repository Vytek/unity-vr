using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour 
{
	// create objects for laser
	public GameObject laserPrefab;

	private GameObject laser;
	private Vector3 hitPoint;
	private Transform laserTransform;

	// create objects for teleportation
	private GameObject reticle;
	public Transform CameraRigTransform;
	public GameObject teleportReticlePrefab;

	private bool shouldTeleport;
	public LayerMask teleportMask;
	public Transform headTransform;
	public Vector3 teleportReticleOffset;
	private Transform teleportReticleTransform;

	// create objects for controller
	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device Controller 
	{
		get { return SteamVR_Controller.Input ((int)trackedObj.index); }
	}

	// Use this for initialization
	void Start ()
	{
		laser = Instantiate (laserPrefab);
		laserTransform = laser.transform;

		reticle = Instantiate(teleportReticlePrefab);
		teleportReticleTransform = reticle.transform;
	}

	// Use this for initialization
	void Awake () 
	{
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}

	private void ShowLaser(RaycastHit hit)
	{
		laser.SetActive (true);

		laserTransform.LookAt (hitPoint);
		laserTransform.position = Vector3.Lerp (trackedObj.transform.position, 
			hitPoint, .5f);
		laserTransform.localScale = new Vector3 (laserTransform.localScale.x, 
			laserTransform.localScale.y, hit.distance);
	}

	private void Teleport()
	{
		shouldTeleport = false;
		reticle.SetActive (false);
		Vector3 difference = CameraRigTransform.position - headTransform.position;

		difference.y = 0;
		CameraRigTransform.position = hitPoint + difference;
	}

	// Update is called once per frame
	void Update () 
	{
		if (Controller.GetPress (SteamVR_Controller.ButtonMask.Touchpad)) 
		{
			RaycastHit hit;

			if (Physics.Raycast (trackedObj.transform.position, transform.forward, 
				    out hit, 100, teleportMask)) 
			{
				hitPoint = hit.point;
				ShowLaser (hit);

				reticle.SetActive (true);
				teleportReticleTransform.position = hitPoint + teleportReticleOffset;

				shouldTeleport = true;
			}
		} 
		else 
		{
			laser.SetActive (false);
			reticle.SetActive (false);
		}

		if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
			Teleport ();
	}
}
