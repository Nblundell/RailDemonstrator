using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignalController : MonoBehaviour
{
    [SerializeField] GameObject[] signals;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        foreach (GameObject signal in signals)
        {
            signal.gameObject.GetComponent<Signals>().setDirection(directionCal(this.gameObject, signal.gameObject));
        }
        InvokeRepeating("updateSignal", 0.3f, 0.3f);
    }
    void updateSignal()
    {
        foreach (GameObject signal in signals)
        {
            signalStatus(signal);
        }
    }
    public void signalStatus(GameObject signal)
    {
        
        GameObject track = signal.gameObject.GetComponent<Signals>().getTrack();
        string direction = track.gameObject.GetComponent<Track>().getDirection();
        if (direction != signal.gameObject.GetComponent<Signals>().getDirection())
        {
            signal.gameObject.GetComponent<Signals>().setColour("Green");
        }
        else
        {
            signal.gameObject.GetComponent<Signals>().setColour("Red");
        }
        if (track.transform.tag == "Empty")
        {
            signal.gameObject.GetComponent<Signals>().setColour("Green");
        }
        if (track.transform.tag == "Broken")
        {
            signal.gameObject.GetComponent<Signals>().setColour("Red");
        }
    }

    public string directionCal(GameObject Refernece, GameObject location)
    {
        string direction;
        float diffX = Refernece.transform.position.x - location.transform.position.x;
        float diffY = Refernece.transform.position.y - location.transform.position.y;
        if (diffX <= 0)
        {
            diffX = diffX * -1;
        }
        if (diffY <= 0)
        {
            diffY = diffY * -1;
        }
        if (diffX <= diffY)
        {

            if (Refernece.transform.position.y - location.transform.position.y < 0)
            {
                direction = "North";
            }
            else
            {
                direction = "South";
            }
        }
        else
        {
            if (Refernece.transform.position.x - location.transform.position.x < 0)
            {
                direction = "East"; ;
            }
            else
            {
                direction = "West";
            }
        }
        return direction;
    }

}
