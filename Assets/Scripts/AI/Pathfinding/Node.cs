using System;
using UnityEngine;

[Serializable]
public struct Node : IComparable<Node>
{
    public int G; // Длина пути от старта
    public int H; // Оценка расстояния до цели
    public int F => G + H; // Полная стоимость
    public Vector2Int pos; // Позиция в сетке
    public int indexParent; // индекс придыдущего узла


    public Node(int g, Vector2Int position, Vector2Int target, int index)
    {
        G = g;
        pos = position;
        indexParent = index;

        H = Mathf.Abs(position.x - target.x) + Mathf.Abs(position.y - target.y); // Манхэттенское расстояние
    }

    // Добавляем сравнение узлов по F (чем меньше, тем лучше)
    public int CompareTo(Node other)
    {
        int fComparison = F.CompareTo(other.F);
        if (fComparison == 0)
            return G.CompareTo(other.G); // При равном F выбираем узел с меньшим G
        return fComparison;
    }
}
