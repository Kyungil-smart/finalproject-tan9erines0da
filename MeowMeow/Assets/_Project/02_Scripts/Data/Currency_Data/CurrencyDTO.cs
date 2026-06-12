using Firebase.Firestore;
using UnityEngine;

[System.Serializable]
[FirestoreData]
public class CurrencyDTO
{
    [field: SerializeField][FirestoreProperty] public int NyangNyangStone { get; set; }
}
