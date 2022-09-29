using UnityEngine;
using System.Collections;

public class AimingDectector : MonoBehaviour 
{
    //Speed at which lerp should occur

    private float speed = 5f;

    public float aiming;
  
    //Lerp Direction
    bool LerpedUp = false;

    //Lerp time value
    float LerpTime = 1.0f;
  
    void Update()   
    { 
        //If HoldOn Right Mousebutton,Move from 0 to 1  
        if(Input.GetMouseButton(1)) 
        {
          if(!LerpedUp)
          {
            //Reset LerpTime
            LerpTime=0.0f;
            //State Lerping Up(0 to 1)
            LerpedUp=true;
          }         
          else if(LerpTime < 1.0f)
          {         
            aiming = Mathf.Lerp(0 , 1, LerpTime);          
            LerpTime += Time.deltaTime*speed;
          }
        }  
        else //If released Right Mousebutton,Move from Point 1 to 0
        {
          if(LerpedUp)
          {
            //Reset LerpTime
            LerpTime=0.0f;
            //State Lerping Down(1 to 0)
            LerpedUp=false;
          }         
          else if(LerpTime < 1.0f)
          {
            aiming = Mathf.Lerp(1 , 0, LerpTime);          
            LerpTime += Time.deltaTime*speed;
          }
        } 
    } 
}
  

