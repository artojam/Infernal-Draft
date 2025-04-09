using System;
using UnityEngine;

[Serializable]
public struct Node : IComparable<Node>
{
    public int G; // ����� ���� �� ������
    public int H; // ������ ���������� �� ����
    public int F => G + H; // ������ ���������
    public Vector2Int pos; // ������� � �����
    public int indexParent; // ������ ����������� ����


    public Node(int g, Vector2Int position, Vector2Int target, int index)
    {
        G = g;
        pos = position;
        indexParent = index;

        H = Mathf.Abs(position.x - target.x) + Mathf.Abs(position.y - target.y); // ������������� ����������
    }

    // ��������� ��������� ����� �� F (��� ������, ��� �����)
    public int CompareTo(Node other)
    {
        int fComparison = F.CompareTo(other.F);
        if (fComparison == 0)
            return G.CompareTo(other.G); // ��� ������ F �������� ���� � ������� G
        return fComparison;
    }
}
