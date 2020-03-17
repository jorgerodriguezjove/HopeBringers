//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//public class Heap<T> where T : IHeapItem<T>
//{
//    T[] items;
//    int currentItemCount;

//    public Heap(int maxHeapSize)
//    {
//        items = new T[maxHeapSize];
//    }

//    //Añado el elemento a la lista de items. Sin embargo ahora esta añadido al final así que tengo que moverlo comparando valores.
//    public void Add(T item)
//    {
//        item.HeapIndex = currentItemCount;
//        items[currentItemCount] = item;
//        SortUp(item);
//        currentItemCount++;
//    }

//    public T RemoveFirst()
//    {
//        T firstItem = items[0];
//        currentItemCount--;
//        items[0] = items[currentItemCount];
//        items[0].HeapIndex = 0;
//        SortDown(items[0]);
//        return firstItem;
//    }

//    public void UpdateItem(T item)
//    {
//        SortUp(item);
//    }

//    public int Count { get { return currentItemCount; } }

//    public bool Contains(T item)
//    {
//        return Equals(items[item.HeapIndex], item);
//    }

//    void SortDown(T item)
//    {
//        while (true)
//        {
//            int childIndexLeft = item.HeapIndex * 2 + 1;
//            int childIndexRight = item.HeapIndex * 2 + 2;
//            int swapIndex = 0;

//            if (childIndexLeft < currentItemCount)
//            {
//                swapIndex = childIndexLeft;

//                if (childIndexRight < currentItemCount)
//                {
//                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
//                    {
//                        swapIndex = childIndexRight;
//                    }
//                }

//                if (item.CompareTo(items[swapIndex]) < 0)
//                {
//                    Swap(item, items[swapIndex]);
//                }
//                else
//                {
//                    return;
//                }
//            }
//            else
//            {
//                return;
//            }
//        }
//    }

//    void SortUp(T item)
//    {
//        //Por como está orenado el Heap con esta formula puedo obtener el padre de este nodo
//        int parentIndex = (item.HeapIndex - 1) / 2;

//        while (true)
//        {
//            T parentItem = items[parentIndex];
//            if (item.CompareTo(parentItem) > 0)
//            {
//                Swap(item, parentItem);
//            }
//            else
//            {
//                break;
//            }
//        }
//    }

//    //Intercambio los dos objetos en el array y también les cambio el index
//    void Swap(T itemA, T itemB)
//    {
//        items[itemA.HeapIndex] = itemB;
//        items[itemB.HeapIndex] = itemA;

//        int tempItemAIndex = itemA.HeapIndex;
//        itemA.HeapIndex = itemB.HeapIndex;
//        itemB.HeapIndex = tempItemAIndex;
//    }

//}

////Al ser T un elemento genérico no se si va a poder compararse asi que uso una interfaz para asegurarme de que tiene un index.
//public interface IHeapItem<T> : IComparable<T>
//{
//    int HeapIndex { get; set; }
//}

//using UnityEngine;
//using System.Collections;
//using System;

//public class Heap<T> where T : IHeapItem<T>
//{

//    T[] items;
//    int currentItemCount;

//    public Heap(int maxHeapSize)
//    {
//        items = new T[maxHeapSize];
//    }

//    public void Add(T item)
//    {
//        item.HeapIndex = currentItemCount;
//        items[currentItemCount] = item;
//        SortUp(item);
//        currentItemCount++;
//    }

//    public T RemoveFirst()
//    {
//        T firstItem = items[0];
//        currentItemCount--;
//        items[0] = items[currentItemCount];
//        items[0].HeapIndex = 0;
//        SortDown(items[0]);
//        return firstItem;
//    }

//    public void UpdateItem(T item)
//    {
//        SortUp(item);
//    }

//    public int Count
//    {
//        get
//        {
//            return currentItemCount;
//        }
//    }

//    public bool Contains(T item)
//    {
//        return Equals(items[item.HeapIndex], item);
//    }

//    void SortDown(T item)
//    {
//        while (true)
//        {
//            int childIndexLeft = item.HeapIndex * 2 + 1;
//            int childIndexRight = item.HeapIndex * 2 + 2;
//            int swapIndex = 0;

//            if (childIndexLeft < currentItemCount)
//            {
//                swapIndex = childIndexLeft;

//                if (childIndexRight < currentItemCount)
//                {
//                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
//                    {
//                        swapIndex = childIndexRight;
//                    }
//                }

//                if (item.CompareTo(items[swapIndex]) < 0)
//                {
//                    Swap(item, items[swapIndex]);
//                }
//                else
//                {
//                    return;
//                }

//            }
//            else
//            {
//                return;
//            }

//        }
//    }

//    void SortUp(T item)
//    {
//        int parentIndex = (item.HeapIndex - 1) / 2;

//        while (true)
//        {
//            T parentItem = items[parentIndex];
//            if (item.CompareTo(parentItem) > 0)
//            {
//                Swap(item, parentItem);
//            }
//            else
//            {
//                break;
//            }

//            parentIndex = (item.HeapIndex - 1) / 2;
//        }
//    }

//    void Swap(T itemA, T itemB)
//    {
//        items[itemA.HeapIndex] = itemB;
//        items[itemB.HeapIndex] = itemA;
//        int itemAIndex = itemA.HeapIndex;
//        itemA.HeapIndex = itemB.HeapIndex;
//        itemB.HeapIndex = itemAIndex;
//    }
//}

//public interface IHeapItem<T> : IComparable<T>
//{
//    int HeapIndex
//    {
//        get;
//        set;
//    }
//}

using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{

    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }

        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
