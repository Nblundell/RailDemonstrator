using UnityEngine;

public class People : MonoBehaviour
{
    [SerializeField] double Fee;
    [SerializeField] GameObject Dest;
    [SerializeField] int delayTime;
    [SerializeField] GameObject UItext;
    [SerializeField] bool departed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void newPass (GameObject [] StationsLeft)
    {    // find station to travel to
        int station = UnityEngine.Random.Range(0, StationsLeft.Length - 1);
        // sets the station to the destination to the randomly assigned number
        Dest = StationsLeft[station];
        //sets the fee based on distance of the station 
        int temp = 5 + (station + 1) * 4;
        //randomises the fee to add more realisitic fares
        Fee = UnityEngine.Random.Range(5 + station , temp);
    }
    public void Delayed(int time)
    {   //adds to the persons delay time
        delayTime = delayTime + time;
    }
    public bool checkEndJourney(GameObject currentLoc)
    {

        if (Dest == currentLoc)
        {//if the delay is less than 3 secons no money lost
            if (delayTime < 3)
            {  UItext.GetComponent<TextGUI>().addLostFareMoney(0);}
            // sets the fare loss to 50% of original cost if delay is 3-6 seconds
            else if (delayTime < 6)
            {
                double temp = (0.5 * Fee);  
                UItext.GetComponent<TextGUI>().addLostFareMoney(temp);
            }
           //sets lost money to 100% of cost of fare if 6 seconds or over in delay
            else
            {UItext.GetComponent<TextGUI>().addLostFareMoney(Fee);}
            departed = true;
            // lets the train know the passeneger has disembared
            return true;
        }
        else
        {   //returns false to let train know this passeneger hasn't dissembared
            return false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(departed)
        {
            Destroy(this.gameObject);
        }
    }
}
