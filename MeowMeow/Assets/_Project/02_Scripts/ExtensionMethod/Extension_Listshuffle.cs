using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension_Listshuffle  
{
     public static void Shuffle<T>(this List<T> list)
     {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
     }
}
