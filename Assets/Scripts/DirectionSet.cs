using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Набор возможных направлений в ячейке помещения
public class DirectionSet
{
    public List<Vector3> dirs = new List<Vector3>(); //список направлений
    private int dirCall = -1; //индекс направления на вызов (направление выбирается из dirs по индексу)
    public bool isCallDestination = false; //данная позиция - цель вызова

    public DirectionSet Clone()
    {
        return new DirectionSet
        {
            dirs = this.dirs,
            dirCall = this.dirCall,
            isCallDestination = this.isCallDestination
        };
    }

    //установить индекс направления на вызов
    public void setCallDirection(int index)
    {
        dirCall = index;
    }

    //указан ли индекс направления на вызов
    public bool isCallDirectionSpecified()
    {
        return dirCall != -1;
    }

    //получить направление на вызов
    public Vector3 getCallDirection()
    {
        return dirs[dirCall];
    }
}
