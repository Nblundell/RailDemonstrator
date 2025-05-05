using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Signals : MonoBehaviour
{
    [SerializeField] string colour = "Green";
    [SerializeField] GameObject track;
    [SerializeField] Sprite[] spriteArray;
    private Sprite currentSprite;
    [SerializeField] string signalDirection;
    [SerializeField] bool changeable = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        setColour("Green");
    }

    // Update is called once per frame
    public string getColour()
    {
        return colour;
    }
    public void setColour(string newColour)
    {
        if(changeable)
        {
            colour = newColour;
            //changes sprite base on colour
            switch (colour)
            {
                case "Red": currentSprite = spriteArray[0]; break;
                case "Green": currentSprite = spriteArray[1]; break;
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = currentSprite;
        }
           
    }
    public GameObject getTrack()
    {
        return track;
    }
    public void setDirection(string direction)
    {
        signalDirection = direction;
    }
    public void joinedTrack()
    {
        //sets signals to red as a train joins
        if(changeable)
        {
            currentSprite = spriteArray[0];
            colour = "Red";
            gameObject.GetComponent<SpriteRenderer>().sprite = currentSprite;
            changeable = false;
        }
        StartCoroutine(Wait());
    }
    public void breakSignal()
    {
        //makes the signal unable to be changes
        this.transform.tag = "Broken";
        changeable = false;
    }
    public void changeState()
    {
        //changes the state of the signal based on its current state
        if(this.transform.tag != "Broken")
        {
            this.transform.tag = "Broken";
            changeable = false;
        }
        else
        {
            this.transform.tag = "Empty";
            changeable = true;
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        if (this.transform.tag != "Broken")
        {
            changeable = true;
        }
    }
    public void updateColour()
    {
        GameObject controller = this.gameObject.transform.parent.gameObject;
        controller.gameObject.GetComponent<SignalController>().signalStatus(this.gameObject);
    }
    public string getDirection()
    {

        return signalDirection;
    }
}
