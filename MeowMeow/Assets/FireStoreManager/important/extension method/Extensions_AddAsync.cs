using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions_AddAsync 
{
    public static async Task AddAsync(this FirestoreRequestContext context, object data)
    {
      var collectionRef = context.TargetStore.GetCollection();
      await collectionRef.AddAsync(data);
    }

     
}
/*
 *
*/
