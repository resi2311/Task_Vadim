using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BallMover : MonoBehaviour
{
	Vector3 direction;          // направление движения по траектории
	Vector3[] path_phys;        // Траектория в виде массива точек в R^3 полученная из физ. движка
	public float speed;         // Скорость передвижения по траектории
	private float last_click;   // Время последнего нажатия на объект
	private int targetIdx;      // Индекс точки к которой движемся
	private bool IsOnPath;      // Состояния движения
	private TrailRenderer tr;   
	float timer;                // Время начала движения

	// Use this for initialization
	void Start ()
	{
		IsOnPath = false;
		speed = 15;
		tr = GetComponent<TrailRenderer>();  // Для отрисовки траектории используем TrailRenderer
		tr.material = new Material (Shader.Find ("Sprites/Default"));
		timer = 10;
		tr.time = 10;
		LoadPath ();    // Загрузим траекторию
		GoToTheStart (); // Установим объект в начало пути
	}



	[System.Serializable]
	public class Trajectory     // Сериализуемый класс
	{
		public float[] x;
		public float[] y;
		public float[] z;
	}

	void LoadPath ()  // Загрузка траектории из ball_path.json 
	{
		string JsonRep = File.ReadAllText( Application.streamingAssetsPath + "/ball_path.json");
		Trajectory trajectory  = JsonUtility.FromJson<Trajectory> (JsonRep);    // Используется стандартная JsonUtility
		path_phys = new Vector3[trajectory.x.Length];
		for (int i = 0; i < trajectory.x.Length; i++) 
			path_phys[i] = new Vector3 (trajectory.x [i], trajectory.y [i],trajectory.z [i]);
	}

	// Update is called once per frame
	void Update () 
	{
		if (IsOnPath) // Проверка состояния объекта
		{
			if (InEpsilon(0.01f) | OutEpsilon())  // Если при перемещении попали в эпсилон окрестность следующей по траектории точки или перелетели за точку
			{
				MoveInEpsilonArea ();                   // Передвигаемся в точку в чью окрестность мы попали и обновляем направление движения
				tr.time = Time.time - timer + 10;       // Для того чтобы путь не исчезал (1)
				return;
			} 

			else 
			{
				transform.position += direction * speed * Time.deltaTime;   // Передвигамся в следующую промежуточную точку на траектории
			}
		}
		tr.time = Time.time - timer + 10; // См.(1)
	}
		

	void MoveInEpsilonArea()    // Отвечает за перемещение в следующую точку траектории
	{
		if (targetIdx == path_phys.Length - 1) // В случае если приблизились к последней точке маршрута
		{
			transform.position = path_phys [targetIdx];
			IsOnPath = false;
			return;
		}
		direction = (path_phys [targetIdx + 1] - path_phys [targetIdx]).normalized;
		targetIdx++;
		transform.position = path_phys [targetIdx - 1];
		return;
	}

	bool OutEpsilon()   // Проверка на перелёт епсилон области
	{
		return (transform.position - path_phys [targetIdx]).magnitude < (direction * speed * Time.deltaTime).magnitude;
	}

	bool InEpsilon(float eps) // Проверка на попадание в епсилон шар
	{
		return (path_phys [targetIdx] - transform.position).magnitude <= eps;
	}

	public void AdjustSpeed(float NewSpeed)  // Изменение скорости с помощью слайдера
	{
		speed = NewSpeed;
		return;
	}

	void OnMouseDown()
	{
		if (!IsOnPath) 
		{
			GoToTheStart ();
			IsOnPath = true;
			timer = Time.time;  // Установка таймера для слежения времени существования траектории
		}

		if (Time.time - last_click < 0.4f)   // Проверка на двойное нажатие мыши
		{
			GoToTheStart ();
			IsOnPath = false;
		}

		last_click = Time.time;
	}

	void GoToTheStart() // Установка в начало траектории
	{
		tr.Clear ();
		transform.position = path_phys [0];
		targetIdx = 0;
	}
}
	
