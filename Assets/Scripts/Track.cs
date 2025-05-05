using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] GameObject[] Links;
    [SerializeField] private int speedLimit;
    [SerializeField] GameObject[] Trains = new GameObject[5];
    [SerializeField] string trainDirection;
    [SerializeField] int numberOfTrains;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.tag = "Empty";
        //InvokeRepeating("updateTrack", 0.3f, 0.3f);
    }
    public int TrainOn(GameObject train)
    {
        if (transform.tag != "Broken")
        {
            transform.tag = "Busy";
        }
        
        Trains[numberOfTrains] = train;
        numberOfTrains += 1;
        return speedLimit;
    }
    public void TrainOff(GameObject leavingTrain)
    {

        if (Trains.Length == 0)
        {
            Trains = new GameObject[4];
        }
        int count = 0;
        GameObject[] temp = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            if (Trains[i] == leavingTrain)
            {
                Trains[i] = null;
                numberOfTrains -= 1;
            }
            else if (Trains[i] != null)
            {
                temp[count] = Trains[i];
                count += 1;
            }
        }
        Trains = temp;
    }
    public GameObject[] getLinks()
    {
        return Links;
    }
    // Update is called once per frame
    public void setDirection(string direction)
    {
        trainDirection = direction;
    }
    public string getDirection()
    {
      return trainDirection;
    }
    public void changeState()
    {
        if (transform.tag != "Broken")
        {
            transform.tag = "Broken";
        }
        else
        {
            transform.tag = "Empty";
        }
    }
    public void breakTrack()
    {
        transform.tag = "Broken";
    }
    void Update()
    {
       
        if (numberOfTrains == 0 && transform.tag != "Broken")
        {
            transform.tag = "Empty";
        }
        if (numberOfTrains != 0 && transform.tag != "Broken")
        {
            transform.tag = "Busy";
        }
    }
}
