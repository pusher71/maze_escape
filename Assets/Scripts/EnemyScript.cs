using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Скрипт врага
public class EnemyScript : MonoBehaviour
{
    [SerializeField] private RayScan scanner; //сканер для обнаружения игрока
    [SerializeField] private float speed; //скорость
    [SerializeField] private float angularSpeed; //угловая скорость
    private Rigidbody rb; //физическое тело
    private Animator animatorNoise; //аниматор для реакций на шум игрока
    private PlayerScript playerScript; //скрипт игрока

    public NavigationMesh nav; //навигационная сетка
    public Queue<Vector3> path; //маршрут патрулирования

    private bool canFollow = true; //преследование возможно
    private bool follow = false; //преследование по сканеру активно
    private bool followLast = false; //последнее значение преследования

    [SerializeField] private float noiseUpperThreshold; //величина шума, при которой враг устремляется к игроку
    private bool noiseExceeded = false; //уровень шума превышен

    private Vector3 target; //целевая позиция в данный момент
    private Quaternion targetQuaternion; //целевое направление взгляда в данный момент

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animatorNoise = GetComponent<Animator>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        playerScript.OnDestroyed += disableCanFollow; //гнаться будет не за кем

        target = transform.position; //пока цели нет
        call(roundVector(transform.position));
        Invoke("changeDirection", 0.5f); //ждём, когда установится nav
    }

    // Update is called once per frame
    void Update()
    {
        //повернуть врага по направлению движения
        targetQuaternion = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, angularSpeed * Time.deltaTime);

        //получить видимость игрока врагом с помощью сканнера
        Vector3 targetPos = Vector3.zero; //возможная позиция цели
        follow = canFollow && scanner.rayToScan(out targetPos);

        //если преследование активно
        if (follow)
        {
            //бросить маршрут
            CancelInvoke("changeDirection");

            //устремиться к цели
            target = targetPos;
            setVelocity();
        }

        //восстановить движение по маршруту, если игрок потерялся
        if (!follow && followLast)
            Invoke("changeDirection", 0.5f); //восстановить нормальное передвижение

        //перезаписать последнее значение активности преследования
        followLast = follow;

        //проверить уровень шума игрока
        noiseExceeded = playerScript.noise >= noiseUpperThreshold;

        //проиграть ответную анимацию
        animatorNoise.SetBool("NoiseExceeded", noiseExceeded);
    }

    //вызвать врага на позицию
    public void call(Vector3 destination)
    {
        nav.resetCallDirections();
        nav.setCallDirections(destination);
    }

    //установить линейную скорость
    private void setVelocity()
    {
        rb.velocity = (target - transform.position).normalized * speed;
    }

    //сменить направление движения
    private void changeDirection()
    {
        if (!follow) //оборвать таймер перемещения в случае погони
        {
            Vector3 pos = transform.position; //текущая позиция
            Vector3 posRounded = roundVector(pos); //округлённая текущая позиция

            DirectionSet dirSet = nav.getDirectionSet(posRounded); //набор направлений в данной позиции

            //произвести вызов на игрока, если его уровень шума превышен
            if (noiseExceeded)
                call(roundVector(playerScript.transform.position));

            //если достигли целевой ячейки в маршруте патрулирования
            if (dirSet.isCallDestination)
            {
                //изменить целевую ячейку
                Vector3 targetCell = path.Dequeue();
                path.Enqueue(targetCell);
                call(targetCell);
            }

            //задать целевую позицию
            target = posRounded + nav.getDirectionSet(posRounded).getCallDirection();
            setVelocity();
            Invoke("changeDirection", (target - pos).magnitude / speed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //если столкновение с игроком
        if (collision.gameObject == scanner.target)
            collision.gameObject.GetComponent<PlayerScript>().kill(); //убить попавшегося игрока
    }

    //отключить возможность преследования
    private void disableCanFollow()
    {
        canFollow = false;
        follow = false;
        followLast = false;
        CancelInvoke("changeDirection");
        rb.velocity = Vector3.zero;
    }

    //округлить координаты вектора до ближайшего целого числа
    private static Vector3 roundVector(Vector3 vector)
    {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }
}
