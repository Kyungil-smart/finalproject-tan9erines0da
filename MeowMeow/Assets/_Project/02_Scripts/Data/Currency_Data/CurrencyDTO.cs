using Firebase.Firestore;
using UnityEngine;

[System.Serializable]
[FirestoreData]
public struct CurrencyDTO
{
    [field: SerializeField][FirestoreProperty] public int NyangNyangStone { get; set; }
}
