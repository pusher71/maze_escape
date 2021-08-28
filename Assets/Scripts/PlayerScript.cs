using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Скрипт игрока
public class PlayerScript : MonoBehaviour
{
    private Rigidbody rb;

    private bool isWalking = false; //игрок передвигается
    public float noise { get; private set; } //текущий уровень шума
    [SerializeField] private float noiseIncSpeed; //величина повышения шума за секунду при движении
    [SerializeField] private float noiseDecSpeed; //величина понижения шума за секунду при покое
    [SerializeField] private float noiseUpperThreshold; //величина шума, при которой враг устремляется к игроку
    [SerializeField] private Image barNoise; //бар для отображения уровня шума
    [SerializeField] private GameObject createOnDestroy; //создаваемые частицы при уничтожении игрока

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        noise = 0;

        //привязать главную камеру к игроку
        Camera.main.transform.parent = transform;
        Camera.main.transform.localPosition = new Vector3(0, 16, -6);
    }

    // Update is called once per frame
    void Update()
    {
        //движение игрока
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(x, 0, y);
        rb.velocity = move;

        isWalking = move.magnitude > 0;
    }

    void FixedUpdate()
    {
        //изменить уровень шума
        if (isWalking)
            noise += noiseIncSpeed * Time.fixedDeltaTime;
        else
            noise -= noiseDecSpeed * Time.fixedDeltaTime;

        //ограничить снизу
        if (noise < 0)
            noise = 0;

        //отобразить шум в баре
        barNoise.fillAmount = Mathf.Clamp01(noise / noiseUpperThreshold);
        barNoise.color = barNoise.fillAmount == 1 ? Color.red : Color.yellow;
    }

    //уничтожить игрока
    public void destroy()
    {
        Camera.main.transform.parent = null; //отвязать главную камеру от игрока
        Instantiate(createOnDestroy, transform.position, Quaternion.identity); //пустить частицы

        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }

    //игрок пропал (от врага или выхода)
    public delegate void _onDestroyed();
    public event _onDestroyed OnDestroyed;

    //убить
    public void kill()
    {
        Killed?.Invoke();
        destroy();
    }

    //игрок убит
    public delegate void _killed();
    public event _killed Killed;

    private void OnCollisionEnter(Collision collision)
    {
        //игрок добавлся до выхода
        if (collision.gameObject.CompareTag("Exit"))
        {
            OnExit?.Invoke();
            destroy();
        }
    }

    //игрок на выходе
    public delegate void _onExit();
    public event _onExit OnExit;
}
