using Firebase.Firestore;
using System;
using UnityEngine;

[System.Serializable]
[FirestoreData]
public class CurrencyDTO
{
    [field: SerializeField][FirestoreProperty] public int NyangNyangStone { get; set; }
    [field: SerializeField][FirestoreProperty] public string LastDate { get; set; }
    [ServerTimestamp]public DateTime ServerTime { get; set; }
}
