using UnityEngine;

public class test : MonoBehaviour
{
    public RobotJoint[] Joints;
    public float SamplingDistance = 0.1f;
    public float LearningRate = 20.0f;
    public float DistanceThreshold = 0.15f;
    public GameObject Target;

    private float[] angles;

    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 prevPoint = Joints[0].transform.position;
        Quaternion jointRotation;
        for (int i = 1; i < Joints.Length; i++)
        {
            jointRotation = Joints[i - 1].transform.localRotation;
            // Выполняет поворот вокруг новой оси
            jointRotation = Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].axis) * jointRotation;
            Debug.Log(jointRotation);
            Vector3 nextPoint = prevPoint + jointRotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }
        return prevPoint;
    }
    private void Start()
    {
        angles = new float[Joints.Length];

        for(int i = 0; i < Joints.Length; i++)
        {
            angles[i] = 30f;
        }

        Vector3 prevPoint = Joints[0].transform.localPosition;

        Debug.Log(ForwardKinematics(angles));
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

    private void Update()
    {
        Debug.Log(Joints[3].transform.position);
    }
}