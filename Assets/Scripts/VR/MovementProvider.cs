﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementProvider : LocomotionProvider
{
	public float speed;
	public float normalSpeed = 2.5f;
	public float fastSpeed = 3f;
	public float gravityMultiplier = 1.0f;
	public List<XRController> controllers = null;

	private CharacterController characterController = null;
	private GameObject head = null;

	public void FastSpeedMode(bool fast) {
		if (fast) speed=fastSpeed;
		else speed=normalSpeed;
	}

    protected override void Awake()
    {
		characterController=GetComponent<CharacterController>();
		head=GetComponent<XRRig>().cameraGameObject;
    }

    private void Start()
    {
		Debug.Log("Device Name: " + XRSettings.loadedDeviceName);
		speed=normalSpeed;
		PositionController();
    }

    private void Update()
    {
		PositionController();
		CheckForInput();
		ApplyGravity();
    }

    private void PositionController()
    {
		// Get the head in local, playspace ground
		float headHeight = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
		characterController.height=headHeight;

		// Cut in half, add skin
		Vector3 newCenter = Vector3.zero;
		newCenter.y=characterController.height/2;
		newCenter.y+=characterController.skinWidth;


		// Let's move the capsule in local space as well
		newCenter.x=head.transform.localPosition.x;
		newCenter.z=head.transform.localPosition.z;
		// Apply
		characterController.center=newCenter;
    }

    private void CheckForInput()
    {
		//Debug.Log("Checking for input");
		foreach (XRController controller in controllers) {
			if (controller.enableInputActions)
				CheckForMovement(controller.inputDevice);
		}
		if (XRSettings.loadedDeviceName == "MockHMD")
        {
			//Debug.Log("Mock HMD");
			if (Input.GetKey(KeyCode.W))
            {
				GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
				StartMove(new Vector2(2f, 0.0f));
				//StartMove(new Vector2(mainCamera.transform.forward.x, mainCamera.transform.forward.z));
			}
			if (Input.GetKey(KeyCode.S))
            {
				GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
				StartMove(new Vector2(-2f, 0.0f));
				//StartMove(new Vector2(-mainCamera.transform.forward.x, -mainCamera.transform.forward.z));
			}
			if (Input.GetKey(KeyCode.D))
			{
				GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
				StartMove(new Vector2(0.0f, 2f));
			}
			if (Input.GetKey(KeyCode.A))
			{
				GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
				StartMove(new Vector2(0.0f, -2f));
			}
		}
    }

    private void CheckForMovement(InputDevice device)
    {
		if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
			StartMove(position);
    }

    private void StartMove(Vector2 position)
    {
		// Apply the touch position to the head's forward Vector
		Vector3 direction = new Vector3(position.x, 0, position.y);
		Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);
		// Rotate the input direction by the horizontal head rotation
		direction=Quaternion.Euler(headRotation)*direction;
		// Apply speed and move
		Vector3 movement = direction*speed;
		characterController.Move(movement*Time.deltaTime);
    }

    private void ApplyGravity()
    {
		Vector3 gravity = new Vector3(0, Physics.gravity.y*gravityMultiplier, 0);
		gravity.y*=Time.deltaTime;

		characterController.Move(gravity*Time.deltaTime);
    }
}
