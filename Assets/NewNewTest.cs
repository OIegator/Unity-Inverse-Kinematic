using UnityEngine;


public class NewNewTest : MonoBehaviour
{ 
	public RobotJoint[] joints;
	public float[] angles;
	public GameObject target;
	
	// Функция которая один раз применит к ноге заданные в инспекторе трансформации. Тестил на ноге С
	private void Start()
	{
		for (int i = 0; i < joints.Length - 1; i++)
		{
			Vector3 direction = joints[i + 1].transform.localPosition - joints[i].transform.localPosition; // Вектор от одного узла к другому
			Vector3 newAxis = Vector3.Cross(direction.normalized, joints[i].transform.InverseTransformDirection(joints[i].axis)); // Перпендикуляр к плоскости образованной направлением и осью из инспектора
			Quaternion rotation = Quaternion.AngleAxis(angles[i], newAxis); // Задаем вращение по этому перпендикуляру 
			joints[i].transform.localRotation *= rotation; // Применяем к локальному вращению узла 
			
			// Дальше отладка 
			Vector3 glob = joints[i].transform.TransformDirection(direction.normalized);
			Debug.DrawRay(joints[i].transform.position, glob, Color.red, 10000);
			Debug.DrawRay(joints[i].transform.position, joints[i].transform.InverseTransformDirection(joints[i].axis), Color.blue, 10000);
			Debug.DrawRay(joints[i].transform.position, joints[i].transform.InverseTransformDirection(newAxis), Color.green, 10000);
		}
		
		// Проблема в самом первом узле, для него некорректно работают вектора направлений и почему так происходит не ясно.
		// Пробовал первый узел считать в глобальных координатах, направения тогда чинятся, по крайней мере визуально, но
		// при трансформации кватарионом оси опять ломаются 
	}
	
}