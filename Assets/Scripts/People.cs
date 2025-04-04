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
    {
        // find station to travel to
        int station = UnityEngine.Random.Range(0, StationsLeft.Length );
      
        if(station == StationsLeft.Length)
        {
            station -= 1;
        }
        Dest = StationsLeft[station];

        int temp = 5 + (station + 1) * 4;
        Fee = UnityEngine.Random.Range(5 , temp);
    }
    public void Delayed(int time)
    {
        delayTime = delayTime + time;
    }
    public bool checkEndJourney(GameObject currentLoc)
    {
        if (Dest == currentLoc)
        {


            if (delayTime < 3)
            {

                UItext.GetComponent<TextGUI>().addLostFareMoney(0);
            }
            else if (delayTime < 6)
            {
                double temp = (0.5 * Fee);  
                UItext.GetComponent<TextGUI>().addLostFareMoney(temp);
            }
            else
            {
                UItext.GetComponent<TextGUI>().addLostFareMoney(Fee);
                
            }
            departed = true;
            return true;
        }
        else
        {
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
