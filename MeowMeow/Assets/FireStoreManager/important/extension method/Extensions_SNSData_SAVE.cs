using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions_SNSData_SAVE  
{
    public static Task SaveSNSPostData(this FirestoreRequestContext context,FirestoreSNSPostDoc PostData)
    {
        var db = context.TargetStore.DB;
        var enumType= context.TargetStore.EnumType;
        if (enumType != DataType.Posts)
        {
            Debug.LogError("해당 기능은 DataType.Posts만 저장할 수 있습니다. ");
            return Task.CompletedTask;          
        }   

        return db.Collection("SNSPostGroups").AddAsync(PostData);
    }
   
}
