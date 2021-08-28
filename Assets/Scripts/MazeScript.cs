using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2D лабиринт (не включает внешние стены)
public class MazeScript : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public int[,] array; //массив лабиринта (0 - пусто, 1 - стена)

    //параметры генерации
    [SerializeField] private int blockCount = 16; //количество расставляемых блоков

    [SerializeField] private GameObject blockPrefab; //заготовка блока для отстраивания

    //сгенерировать лабиринт
    public void generate()
    {
        array = new int[width, height];

        //проставить стены в массив лабиринта
        for (int i = 0; i < blockCount; i++)
        {
            //получить случайную позицию очередной стены
            Vector3 pos = getRandomEmptyPosition(1, width - 1, 1, height - 1);

            //поставить стену
            array[(int)pos.x, (int)pos.z] = 1;
        }
    }

    //получить случайную пустую позицию в заданном диапазоне
    public Vector3 getRandomEmptyPosition(int minX, int maxX, int minY, int maxY)
    {
        if (minX < 0 || maxX > width || minY < 0 || maxY > height)
            throw new System.Exception("Невозможно получить случайную позицию. Выбранный диапазон выходит за границы лабиринта.");

        Vector3 pos;
        do pos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minY, maxY));
        while (array[(int)pos.x, (int)pos.z] != 0);
        return pos;
    }

    //отстроить лабиринт на сцене
    public void build()
    {
        if (array != null)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (array[x, y] == 1)
                    {
                        GameObject block = Instantiate(blockPrefab, transform);
                        block.transform.position = new Vector3(x, 0, y);
                    }
        }
        else
            throw new System.Exception("Невозможно отстроить лабиринт. Метод генерации не был запущен.");
    }

    //очистить лабиринт со сцены
    public void clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
