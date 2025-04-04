using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField] public bool working = true;
    [SerializeField] public int numOfPlatforms;
    [SerializeField] GameObject[] Trains ;
    [SerializeField] GameObject[] Tracks;
    [SerializeField] GameObject[] Signals;
    [SerializeField] int numberOfTrains;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numberOfTrains = 0;
        numOfPlatforms =  4;
        Trains = new GameObject[4];
    }
    public GameObject[] getTracks()
    {
        return Tracks;
    }
    public GameObject[] getSignals()
    {
        return Signals;
    }
    public bool trainArriving(GameObject train)
    {
       if (numOfPlatforms > numberOfTrains)
       {
            Trains[numberOfTrains] = train;
            numberOfTrains += 1;
            return true;
       }
        return false;
    }
    public void trainLeaving(GameObject leavingTrain)
    {
        
        if (Trains.Length == 0)
        {
            Trains = new GameObject[4];
        }
        int count = 0;
        GameObject[] temp = new GameObject[numOfPlatforms];
        for (int i = 0; i < 4; i++)
        {
            if (Trains[i] == leavingTrain)
            {
                Trains[i] = null;
                numberOfTrains -= 1;
            }
            else if(Trains[i] != null)
            {
                temp[count] = Trains[i];
                count += 1;
            }
        }
        Trains = temp;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public GameObject[] getTrains()
    {
        return Trains;
    }
}
