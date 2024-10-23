using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UIElements;

[RequireComponent(typeof(Camera))]
public class BoardCamera : MonoBehaviour
{
	private new Camera camera;
	public Camera Camera
	{
		get
		{
			if (this.camera == null)
				this.camera = this.GetComponent<Camera>();
			return this.camera;
		}
	}

	#region Movement
	[Header("Movement")]
	[Tooltip("The bounds used to constrain the position of this camera")]
	public GridBounds Bounds;

	[Tooltip("The plane to constraint the position of this camera to")]
	public Tilemap.Orientation Orientation;

	[Tooltip("The vector2 action read to get move deltas")]
	public InputActionReference MoveAction;

	[Tooltip("How quickly this can move")]
	public float MoveSpeed;

	private Vector3 Swizzle(Vector2 value)
	{
		switch (this.Orientation)
		{
			case Tilemap.Orientation.XY:
				return new(value.x, value.y, 0);
			case Tilemap.Orientation.XZ:
				return new(value.x, 0, value.y);
			case Tilemap.Orientation.YZ:
				return new(0, value.x, value.y);
			case Tilemap.Orientation.YX:
				return new(value.y, value.x, 0);
			case Tilemap.Orientation.ZX:
				return new(value.y, 0, value.x);
			case Tilemap.Orientation.ZY:
				return new(0, value.y, value.x);
			default:
				throw new NotImplementedException($"Unsupported orientation: {this.Orientation}");
		}
	}

	private Vector3 Filter(Vector3 base_value, Vector3 filtered_value)
	{
		switch (this.Orientation)
		{
			case Tilemap.Orientation.XY:
			case Tilemap.Orientation.YX:
				return new(filtered_value.x, filtered_value.y, base_value.z);
			case Tilemap.Orientation.XZ:
			case Tilemap.Orientation.ZX:
				return new(filtered_value.x, base_value.y, filtered_value.z);
			case Tilemap.Orientation.YZ:
			case Tilemap.Orientation.ZY:
				return new(base_value.x, filtered_value.y, filtered_value.z);
			default:
				throw new NotImplementedException($"Unsupported orientation: {this.Orientation}");
		}
	}

	public void UpdatePosition(float delta_time)
	{
		Vector2 move_delta = this.MoveAction.action.ReadValue<Vector2>();
		if (move_delta == Vector2.zero)
			return;
		Vector2 displacement = move_delta * this.MoveSpeed * delta_time;
		Vector3 position = this.transform.position;
		Vector3 displaced = position + this.Swizzle(displacement);
		Vector3 clamped = this.Bounds.ClampWorld(displaced);
		Vector3 final = this.Filter(position, clamped);
		this.transform.position = final;
		Debug.Log($"Updated board camera position: {final}", this);
	}
	#endregion

	#region Zoom
	[Header("Zoom")]
	[Tooltip("The action read to get zoom deltas")]
	public InputActionReference ZoomAction;

	[Tooltip("The minimum and maximum amount of zoom")]
	[Min(0)]
	public Vector2 ZoomRange = new(1,5);

	[Tooltip("How quickly this can zoom")]
	public float ZoomSpeed = 1;

	public void UpdateZoom(float delta_time)
	{
		float input = this.ZoomAction.action.ReadValue<float>();
		if (Mathf.Approximately(input, 0))
			return;
		float change = input * this.ZoomSpeed * delta_time;
		float current = this.Camera.orthographicSize;
		float clamped = Mathf.Clamp(current + change, this.ZoomRange.x, this.ZoomRange.y);
		this.Camera.orthographicSize = clamped;
		Debug.Log($"Updated board camera zoom: {clamped}", this);
	}
	#endregion

	#region Unity Messages
	private void Update()
	{
		this.UpdatePosition(Time.deltaTime);
		this.UpdateZoom(Time.deltaTime);
	}
	#endregion
}
