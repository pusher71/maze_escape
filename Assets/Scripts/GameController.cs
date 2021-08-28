using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Игровой контроллер
public class GameController : MonoBehaviour
{
    //меню
    [SerializeField] private Canvas canvasMain; //окно с главным меню
    [SerializeField] private Canvas canvasGameOver; //окно проигрыша
    [SerializeField] private Canvas canvasWin; //окно выигрыша

    //игровые объекты
    [SerializeField] private GameObject playerPrefab; //заготовка игрока
    [SerializeField] private GameObject enemyPrefab; //заготовка врага
    [SerializeField] private int enemyCount = 2; //количество врагов
    [SerializeField] private int enemyPathVerticesCount = 8; //коичество вершин на маршрутах патрулирования у врагов
    [SerializeField] private MazeScript maze; //генерируемый лабиринт

    //начать игру
    public void startGame()
    {
        //скрыть все окна
        canvasMain.enabled = false;
        canvasGameOver.enabled = false;
        canvasWin.enabled = false;

        clearScene();

        //сгенерировать и отстроить новый лабиринт
        maze.clear();
        maze.generate();
        maze.build();

        //заспавнить игрока
        GameObject player = Instantiate(playerPrefab, transform);
        player.transform.position = Vector3.zero;

        //добавить обработку событий игрока
        player.GetComponent<PlayerScript>().Killed += showGameOver;
        player.GetComponent<PlayerScript>().OnExit += showWin;

        //заспавнить врагов
        NavigationMesh nav = new NavigationMesh(maze);
        for (int i = 0; i < enemyCount; i++)
        {
            //построить маршрут патрулирования
            Queue<Vector3> path = new Queue<Vector3>();
            for (int j = 0; j < enemyPathVerticesCount; j++)
                path.Enqueue(i == 0 //враги спавнятся на случайных позициях из разных диапазонов
                    ? maze.getRandomEmptyPosition(0, 10, 7, 10)
                    : maze.getRandomEmptyPosition(7, 10, 0, 10));

            //настроить врага
            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.transform.position = path.Peek();
            enemy.GetComponent<EnemyScript>().nav = nav.Clone();
            enemy.GetComponent<EnemyScript>().path = path;
        }
    }

    //очистить сцену от сущностей
    public void clearScene()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    //показать окно проигрыша
    public void showGameOver()
    {
        canvasGameOver.enabled = true;
    }

    //показать окно выигрыша
    public void showWin()
    {
        canvasWin.enabled = true;
    }

    //выйти из игры
    public void quitGame()
    {
        Application.Quit();
    }
}
