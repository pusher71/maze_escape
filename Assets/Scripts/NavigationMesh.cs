using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Навигационная сетка для врага
public class NavigationMesh
{
    public DirectionSet[,] dirSets; //массив наборов направлений в ячейках
    private int width;
    private int height;

    public NavigationMesh()
    {

    }

    public NavigationMesh(MazeScript maze)
    {
        width = maze.width;
        height = maze.height;
        dirSets = new DirectionSet[width, height];

        //построить навигационную сетку
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                //присвоить направления на DirectionSet в данной ячейке
                dirSets[x, y] = new DirectionSet();
                if (x > 0 && maze.array[x - 1, y] == 0)
                    dirSets[x, y].dirs.Add(Vector3.left);
                if (y > 0 && maze.array[x, y - 1] == 0)
                    dirSets[x, y].dirs.Add(Vector3.back);
                if (x < width - 1 && maze.array[x + 1, y] == 0)
                    dirSets[x, y].dirs.Add(Vector3.right);
                if (y < height - 1 && maze.array[x, y + 1] == 0)
                    dirSets[x, y].dirs.Add(Vector3.forward);
            }
    }

    public NavigationMesh Clone()
    {
        DirectionSet[,] dirSetsNew = new DirectionSet[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                dirSetsNew[x, y] = dirSets[x, y].Clone();

        return new NavigationMesh
        {
            dirSets = dirSetsNew,
            width = this.width,
            height = this.height
        };
    }

    //получить набор направлений по позиции
    public DirectionSet getDirectionSet(Vector3 position)
    {
        return dirSets[Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z)];
    }

    //установить направления на вызов
    private static Queue<Vector3> positions = new Queue<Vector3>(); //очередь посещаемых позиций
    private static Queue<Vector3> backDirs = new Queue<Vector3>(); //очередь задних направлений
    public void setCallDirections(Vector3 destination)
    {
        positions.Clear();
        backDirs.Clear();
        positions.Enqueue(destination);
        backDirs.Enqueue(Vector3.zero);

        //пока очередь не закончилась, и размах имеется
        while (positions.Count > 0)
        {
            //вынуть из очереди позицию и её ROOT-направление
            Vector3 position = positions.Dequeue();
            Vector3 backDir = backDirs.Dequeue();

            //получить текущий набор направлений
            DirectionSet current = getDirectionSet(position);

            //если направление вызова не задано
            if (!current.isCallDirectionSpecified())
            {
                //для каждого направления
                for (int i = 0; i < current.dirs.Count; i++)
                {
                    if (current.dirs[i] == backDir) //если направление ведёт назад (к цели вызова)
                        current.setCallDirection(i); //пометить его как направление следования

                    //добавить в очередь данные для обработки
                    positions.Enqueue(position + current.dirs[i]);
                    backDirs.Enqueue(-current.dirs[i]);
                }

                if (!current.isCallDirectionSpecified()) //если направление так и не удалось задать
                    current.isCallDestination = true; //пометить как финишную точку
            }
        }
    }

    //сбросить направления на вызов
    public void resetCallDirections()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                dirSets[x, y].setCallDirection(-1);
                dirSets[x, y].isCallDestination = false;
            }
    }
}
