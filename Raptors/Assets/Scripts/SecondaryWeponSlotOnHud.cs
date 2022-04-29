using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryWeponSlotOnHud : MonoBehaviour
{
    public int value=0;
    public Text myText;
    public Image myImage;
    public Sprite[] variants;

    public void MakeChange(int option){
        if(value != option){
            
            value = option;
            
                myImage.sprite = variants[option];
                if(option == 0) {myText.text = "";}
                if(option == 1) {myText.text = "S";}
                if(option == 2) {myText.text = "C";}
                if(option == 3) {myText.text = "T";}
                if(option == 4) {myText.text = "X";}

        }        
    }

    private void Start() {
        myImage.sprite = variants[0];
        myText.text = "";
    }
}
