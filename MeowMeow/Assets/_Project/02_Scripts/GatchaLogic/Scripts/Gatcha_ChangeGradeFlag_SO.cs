using System.Collections;
 
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "09_Gatcha_ChangeGradeFlag_SO", menuName = "GatchaSO/09_Gatcha_IsCompensation_SO")]
public class Gatcha_ChangeGradeFlag_SO : BaseGatcha
{
    public override void Init(GatchaDataManager manager)
    {
        this.Owner = manager;
    }
    public override void Excute(int value)
    {
        if (value == 1)
            Owner.GatchaData.Grade_1 = true;
        else if (value == 2)
            Owner.GatchaData.Grade_2 = true;
        else if (value == 3)
            Owner.GatchaData.Grade_3 = true;
    }
}
