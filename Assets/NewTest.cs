using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;

public class NewTest : MonoBehaviour
{
	public float SamplingDistance = 0.1f;
	public float LearningRate = 0.5f;
	public float DistanceThreshold = 0.15f;
	[FormerlySerializedAs("joints")] public RobotJoint[] Joints;
	public float[] angles;
	public GameObject target;

	private void Update()
	{
		Vector3 targetPosition = target.transform.position;

		InverseKinematics(targetPosition, angles);

		Quaternion jointRotation;
		Quaternion rotationQuaternion;
		for (int i = 0; i < Joints.Length; i++)
		{
			jointRotation = Joints[i].transform.localRotation;

			rotationQuaternion = Quaternion.AngleAxis(angles[i], Joints[i].axis);

			jointRotation = rotationQuaternion * jointRotation;

			Joints[i].transform.localRotation = jointRotation;
		}
	}

	Vector3 ForwardKinematics(float[] angles)
	{
		Quaternion[] initialRotations = new Quaternion[Joints.Length];
		initialRotations[0] = Joints[0].transform.rotation;
		for (int i = 1; i < Joints.Length; i++)
		{
			initialRotations[i] = Joints[i].transform.localRotation;
		}

		Vector3 endEffectorPosition = Vector3.zero;

		for (int i = 0; i < Mathf.Min(Joints.Length, angles.Length); i++)
		{
			Quaternion rotationQuaternion = Quaternion.AngleAxis(angles[i], Joints[i].axis);
			Joints[i].transform.localRotation = rotationQuaternion * initialRotations[i];
		}

		endEffectorPosition = Joints[Joints.Length - 1].transform.position;

		for (int i = 0; i < Joints.Length; i++)
		{
			Joints[i].transform.localRotation = initialRotations[i];
		}

		return endEffectorPosition;
	}

	public float DistanceFromTarget(Vector3 target, float[] angles)
	{
		Vector3 point = ForwardKinematics(angles);
		return Vector3.Distance(point, target);
	}

	public float PartialGradient(Vector3 target, float[] angles, int i)
	{
		float angle = angles[i];
		float f_x = DistanceFromTarget(target, angles);

		angles[i] += SamplingDistance;

		float f_x_plus_d = DistanceFromTarget(target, angles);

		float gradient = (f_x_plus_d - f_x) / SamplingDistance;

		angles[i] = angle;

		return gradient;
	}


	public void InverseKinematics(Vector3 target, float[] angles)
	{
		if (DistanceFromTarget(target, angles) < DistanceThreshold)
			return;

		for (int i = Joints.Length - 1; i >= 0; i--)
		{
			// Градиентный спуск
			// Обновление : Solution -= LearningRate * Gradient
			float gradient = PartialGradient(target, angles, i);
			angles[i] -= LearningRate * gradient;

			//angles[i] = Mathf.Clamp(angles[i], joints[i].MinAngle, joints[i].MaxAngle);

			if (DistanceFromTarget(target, angles) < DistanceThreshold)
				return;
		}
	}

}