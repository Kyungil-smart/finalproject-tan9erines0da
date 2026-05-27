using UnityEngine;

public interface ISheetParsable
{
    //구글 시트에서 가져온 데이터를 행 단위로 추출 후 split해서 string []에 저장한다.
    //저장된 데이터는 함수 매개변수로 넘어가서 규격에 

    public void ApplyRowData(string [] Data);

}