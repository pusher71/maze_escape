using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Сканер для обнаружения игрока
public class RayScan : MonoBehaviour
{
	private LineRenderer lineRenderer; //визуализатор области сканирования

	//параметры сканирования
	[SerializeField] private int rayCount = 24; //количество лучей
	[SerializeField] private int distance = 4; //длина лучей
	[SerializeField] private float angle = 30; //угол обзора

	public GameObject target { get; set; } //цель обнаружения

	// Start is called before the first frame update
	void Start()
    {
		lineRenderer = GetComponent<LineRenderer>();
		target = GameObject.FindGameObjectWithTag("Player");
    }

	//проверить, находится ли цель в сканируемой области
	public bool rayToScan(out Vector3 targetPos)
	{
		targetPos = Vector3.zero;
		bool result = false;
		float j = -angle * Mathf.Deg2Rad; //текущий угол

		Vector3[] scanOutline = new Vector3[rayCount + 1]; //контур полученной области сканирования

		//для каждого луча
		for (int i = 0; i < rayCount; i++)
		{
			//определить координаты направления луча по углу j
			var x = Mathf.Sin(j);
			var y = Mathf.Cos(j);

			//увеличить текущий угол
			j += angle * Mathf.Deg2Rad * 2 / rayCount;

			//проверить наличие целевого объекта на луче
			Vector3 dir = transform.TransformDirection(new Vector3(x, 0, y));
			bool targetOnRay = getRaycast(dir, out Vector3 endPoint);
			if (targetOnRay)
			{
				//целевой объект обнаружен
				result = true;
				targetPos = endPoint;
			}

			scanOutline[i] = endPoint;
		}

		//замкнуть область сканирования
		scanOutline[rayCount] = transform.position;

		//отобразить сканируемую область
		lineRenderer.positionCount = scanOutline.Length;
		lineRenderer.SetPositions(scanOutline);

		return result;
	}

	//проверить, проходит ли луч по направлению через цель
	private bool getRaycast(Vector3 dir, out Vector3 endPoint)
	{
		bool result = false;

		if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance))
		{
			endPoint = hit.point;
			if (hit.transform == target.transform)
				result = true;
		}
		else
			endPoint = transform.position + dir * distance;

		return result;
	}
}
