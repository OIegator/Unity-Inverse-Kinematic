using System.Drawing;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Robot : MonoBehaviour
{
    public RobotJoint[] Joints;
    public float SamplingDistance = 0.1f;
    public float LearningRate = 20.0f;
    public float DistanceThreshold = 0.15f;
    public GameObject Target;

    public float[] angles;
	private void FixedUpdate()
    {
        // Calculate target position based on the target GameObject
		Vector3 targetPosition = Target.transform.position;

        // Perform inverse kinematics to adjust joint angles
        InverseKinematics(targetPosition, angles);

        // Update the joint rotations based on the calculated angles
        for (int i = 0; i < Joints.Length; i++)
        {
            Joints[i].transform.localRotation = Quaternion.AngleAxis(angles[i], Joints[i].axis) * Joints[i].StartRotation;
		}
    }
    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 prevPoint = Joints[0].transform.position;
        Quaternion rotation = Joints[0].transform.rotation;
        for (int i = 1; i < Joints.Length; i++)
        {
            // Выполняет поворот вокруг новой оси
            rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].axis) * Joints[i - 1].StartRotation;
            Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }
        return prevPoint;
    }


    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
		Debug.Log("Target - " + target + " Point - " + point + " Distance - " + Vector3.Distance(point, target)
        + " Point glob - " + Joints[^1].transform.position);
		return Vector3.Distance(point, target);
    }

    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        // Сохраняет угол,
        // который будет восстановлен позже
        float angle = angles[i];

        // Градиент: [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);

        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Восстановление
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

            // Ограничение
            angles[i] = Mathf.Clamp(angles[i], Joints[i].MinAngle, Joints[i].MaxAngle);

            // Преждевременное завершение
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;
        }
    }

}