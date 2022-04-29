using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnDatabaseStuff : MonoBehaviour
{
    public int id;
    public Text nameText;
    public Data myData;

    public void Inicialization(int nexId, Data newData){
        myData = newData;
        id = nexId;
        nameText.text = myData.databaseOfStuff[id].name;
    }

    public void ButtonClick(){
        myData.ButtonOnDatabaseClick(id);
    }
}
