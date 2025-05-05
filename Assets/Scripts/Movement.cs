using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


public class Movement : MonoBehaviour
{
    //the stations the train whats to get to
    [SerializeField] GameObject[] route;
    //speed of the train
    [SerializeField] public int speed;
    //incrmentation of the route list
    [SerializeField] int nextLoc;
    //the direction of the route the train is traving 1 = to -1 = from
    private int direction = 1;
    //if the train was unable to find a track
    private bool stuck = false;
    //if the train has moved
    private int moved = 0;
    //the current infustruture the train is using
    public GameObject currentLoc;
    //the location that the sprite is pathing towards
    [SerializeField] Vector2 nextPostion;
    private GameObject waitingForSwitch = null;
    [SerializeField] GameObject pass;
    [SerializeField] GameObject[] passengers;
    private int Capacity = 200;
    [SerializeField] int numOfPassengers = 0;
    [SerializeField] GameObject UserInterface;
    private bool firstCall = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //This determines the delay on the trains
        float delay = UnityEngine.Random.Range(1 , 10);
        System.Random rand = new System.Random();
        float ranFloat = (float)rand.NextDouble();

        //sets trains tag to broken so it doesnt interact with stations until it starts moving
        this.transform.tag = "Broken";
        //sets next loc to 0 as the is where the train will start its route
        nextLoc = 0;
        //sets current loc to start location
        currentLoc = route[nextLoc].gameObject;
        //moves the train to its start location
        transform.Translate(route[nextLoc].transform.position.x, route[nextLoc].transform.position.y, 0);
        //sets the size of the passenger array based on the trains capacity
        passengers = new GameObject[Capacity];
        //calls the function after the delay begin funtion that starts the trains journey
        InvokeRepeating("begin", delay, 0f);
    }
    void begin()
    {
        // starts the train in the station it located at
        //this is so it picks up passangers at journey begining
        enterStation(currentLoc);
        //sets tag to empty so it can interact with what it colides with
        this.transform.tag = "Empty";
        //stops this funtion after it has run once
        CancelInvoke();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        // checks if the train is able to interact with stations
        if (this.transform.tag != "Broken")
        {
           //checks if the collided object is a station
            if (col.transform.parent.name == "Stations")
            {
                GameObject temp = col.gameObject;
                enterStation(temp);
            }
            //checks if the collided object is a switch
            else if (col.transform.parent.name == "Switches")
            {
                GameObject temp = col.gameObject;
                onSwitch(temp);
            }
            //checks if the train has crash with another train
            else if (this.transform.tag != "Broken"&& col.transform.parent.name == "Trains" && currentLoc.transform.parent.name == "Tracks" && (col.GetComponent<Movement>().getCurrentloc()).gameObject.transform.parent.name == "Tracks")
            {
                
                Debug.Log(message: $"Crashed: {transform.name}");
                this.transform.tag = "Broken";
                stuck = true;
                //rotates the train so it looks crashed
                this.transform.Rotate(0, 0, 30);
                currentLoc.GetComponent<Track>().breakTrack();
                Debug.Log(numOfPassengers);
                UserInterface.GetComponent<TextGUI>().crash(numOfPassengers);
                nextPostion = new Vector2(transform.position.x, transform.position.y);
                
                
            }
        }
        else
        {
             moved++;
        }
    }
    void onSwitch(GameObject aSwitch)
    {
        bool found = false;
        GameObject[] signals = aSwitch.GetComponent<Switch>().getSignals();
        foreach (GameObject signal in signals)
        {

            if (found)
            {
                break;
            }
            GameObject track = signal.GetComponent<Signals>().getTrack();
            GameObject[] Links = track.GetComponent<Track>().getLinks();
            foreach (GameObject link in Links)
            {

                if (signal.GetComponent<Signals>().getColour() == "Green" && link.transform.name == route[nextLoc].transform.name)
                {
                    //switch check
                    if (aSwitch.GetComponent<Switch>().switchCheck(currentLoc, track, this.gameObject))
                    {
                        
                        calNextPos(route[nextLoc], track);
                        TrackChange(track);
                        found = true;
                        
                        break;
                    }
                    
                }
                else if (link.transform.parent.name == "Switches" && link != aSwitch)
                {
                    
                    foreach (GameObject loc in Links)
                    {
                        if (loc != link)
                        {
                            GameObject[] switchLinks = link.GetComponent<Switch>().getLinks();
                            foreach (GameObject Station in switchLinks)
                            {

                                if (signal.GetComponent<Signals>().getColour() == "Green" && Station.transform.name == route[nextLoc].transform.name && loc != this.getPreviousStation())
                                {
                                   
                                    calNextPos(loc, track);

                                    TrackChange(track);
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                       
                    break;
                }

            }
        }
        if (found == false)
        {
            waitingForSwitch = aSwitch;
            stuck = true;
        }
        else
        {
            waitingForSwitch = null;
        }
    }

    void calNextPos(GameObject newPos, GameObject track)
    {
        
        float diffX = newPos.transform.position.x - this.transform.position.x;
        float diffY = newPos.transform.position.y- this.transform.position.y;
        if (diffX <= 0)
        {
            diffX *= -1;
        }
        if (diffY <= 0)
        {
            diffY *= -1;
        }
        if (diffX <= diffY)
        {
          
            if(newPos.transform.position.y - this.transform.position.y < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
                track.GetComponent<Track>().setDirection( "South");
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 270);
                track.GetComponent<Track>().setDirection( "North");
            }
            nextPostion = new Vector2(track.transform.position.x, newPos.transform.position.y);
            transform.position = new Vector2(track.transform.position.x, this.transform.position.y);
        }
        else
        //if (diffY <= diffX)
        {
            if(newPos.transform.position.x - this.transform.position.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                track.GetComponent<Track>().setDirection( "West");
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 180);
                track.GetComponent<Track>().setDirection("East");
            }
            nextPostion = new Vector2(newPos.transform.position.x, track.transform.position.y);
           
            transform.position = new Vector2(this.transform.position.x, track.transform.position.y);
            // transform.Translate(0, track.transform.position.y - this.transform.position.y, 0);
        }
        
    }
    //used if there is an error in the trains start
    void startUp(GameObject[] signals)
    {
        bool found = false;
        try
        {
            foreach (GameObject signal in signals)
            {

                if (found)
                {
                    break;
                }
                GameObject track = signal.GetComponent<Signals>().getTrack();
                GameObject[] Links = track.GetComponent<Track>().getLinks();
                foreach (GameObject link in Links)
                {
                    
                    if (signal.GetComponent<Signals>().getColour() == "Green" && link.transform.name == route[nextLoc].transform.name)
                    {
                        calNextPos(route[nextLoc], track);
                        currentLoc = track;
                        speed = currentLoc.GetComponent<Track>().TrainOn(this.gameObject);
                        stuck = false;
                        found = true;
                        break;
                    }
                }
            }
        }
        catch
        {
            stuck = true;
            // train unable to leave station
        }
        if (found == false)
        {
            stuck = true;
        }
    }
    //used to search for a track for the train to use
    private void LookForTrack(GameObject[] signals)
    {
        //local variable for when a suitable
        bool found = false;
        try
        {
            //creates a loacl array for the stations
            GameObject[] Links = null;
            //creates local variable for the track
            GameObject track;
            foreach (GameObject signal in signals)
            {

                if (found)
                {
                    foreach (GameObject link in Links)
                    {
                        // creates a array for the signals in the 
                        GameObject[] sigs;
                        if (link.transform.parent.name == "Switches")
                        {
                            sigs = link.GetComponent<Switch>().getSignals();
                        }
                        else
                        {
                            sigs = link.GetComponent<Station>().getSignals();
                        }
                        foreach (GameObject sig in sigs)
                        {
                            sig.GetComponent<Signals>().updateColour();
                        }
                    }
                    break;
                }
                track = signal.GetComponent<Signals>().getTrack();
                Links = track.GetComponent<Track>().getLinks();

                foreach (GameObject link in Links)
                {


                    if (signal.GetComponent<Signals>().getColour() == "Green" && link.transform.name == route[nextLoc].transform.name)
                    {
                        calNextPos(route[nextLoc], track);
                        leaveStation(track);
                        signal.GetComponent<Signals>().joinedTrack();
                        found = true;
                        break;
                    }
                    else if(link.transform.parent.name == "Switches")
                    {
                        
                        GameObject[] switchLinks = link.GetComponent<Switch>().getLinks();
                        foreach(GameObject Station in switchLinks)
                        {
                            if (signal.GetComponent<Signals>().getColour() == "Green" && Station.transform.name == route[nextLoc].transform.name)
                            {
                                
                                calNextPos(link, track);
                                
                                leaveStation(track);
                               
                                signal.GetComponent<Signals>().joinedTrack();
                                
                                found = true;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
        catch
        {
            
            stuck = true;
            // train unable to leave station
        }
       
        if (found == false)
        {
            
            stuck = true;
        }
    }

    private IEnumerator InStation()
    {
        //adds and remove people
        
        if (direction == 1)
        {
            nextLoc += 1;
        }
        if (direction == -1)
        {
            nextLoc -= 1;
        }
        if (currentLoc.name == route[route.Length - 1].name && nextLoc >= route.Length)
        {
            direction = -1;
            nextLoc = route.Length - 2;
        }
        if (currentLoc.name == route[0].name && nextLoc == -1)
        {
            direction = 1;
            nextLoc = 1;
        }
        if (transform.tag != "Broken")
        {
            yield return new WaitForSeconds(2);
        }
        if (!stuck)
        {
            int count = 0;

            GameObject[] newPassangers = new GameObject[Capacity];
            foreach (GameObject passenger in passengers)
            {
                if (passenger != null)
                {
                    if (!passenger.GetComponent<People>().checkEndJourney(currentLoc))
                    {
                        newPassangers[count] = passenger;
                        count += 1;
                    }
                    else
                    {

                        numOfPassengers -= 1;
                    }
                }
            }
            passengers = newPassangers;
            if (numOfPassengers != Capacity)
            {
               
                GameObject[] nextStations = stationsLeft();
                
                int newPass = UnityEngine.Random.Range(0, Capacity - numOfPassengers);
              
                for (int i = 0; i < newPass; i++)
                {
                   
                    passengers[numOfPassengers + i] = Instantiate(pass);
                    passengers[numOfPassengers + i].GetComponent<People>().newPass(nextStations);
                    
                }
                numOfPassengers = numOfPassengers + newPass;
            }
        }
        try
        {
            LookForTrack(currentLoc.GetComponent<Station>().getSignals());
        }
        catch
        {
           
            stuck = true;
        }
    }

    void enterStation(GameObject station)
    {
        

        if (currentLoc.transform.parent.name == "Stations")
        {
            //currentLoc.gameObject.GetComponent<Station>().trainLeaving(this.gameObject);
            currentLoc = station;
            StartCoroutine(InStation());
        }
        else if (station.GetComponent<Station>().trainArriving(this.gameObject) && currentLoc.transform.parent.name != "Stations")
        {
            stuck = false;
            currentLoc.GetComponent<Track>().TrainOff(this.gameObject);
            currentLoc = station;
            StartCoroutine(InStation());
        }
        else
        {
            stuck = true;
        }
    }
    void TrackChange (GameObject newTrack)
    {
        currentLoc.GetComponent<Track>().TrainOff(this.gameObject);
        currentLoc = newTrack;
        speed = currentLoc.GetComponent<Track>().TrainOn(this.gameObject);
        stuck = false;
    }

    void leaveStation(GameObject newTrack)
    {
        bool stay = true;
        while (stay)
        {
            //request to leave
            if(stay)
            {
                stay = false;
            }
        }
        currentLoc.GetComponent<Station>().trainLeaving(this.gameObject);
        currentLoc = newTrack;
        speed = currentLoc.GetComponent<Track>().TrainOn(this.gameObject);
        stuck = false;
    }
  
    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, nextPostion, speed * Time.deltaTime);
    }

    void Lost()
    {
        if (!stuck)
        {
            CancelInvoke();
            firstCall = true;
        }
        else
        {
            
            for (int i = 0; i < numOfPassengers; i++)
            {
                passengers[i].GetComponent<People>().Delayed(1);
            }
            
            if (currentLoc.transform.parent.name == "Stations")
            {
          
                LookForTrack(currentLoc.GetComponent<Station>().getSignals());
            }
            else if (currentLoc.transform.parent.name == "Tracks")
            {
                if(waitingForSwitch != null)
                {
                    onSwitch(waitingForSwitch);
                }
                else
                {
                    enterStation(route[nextLoc]);
                }
                
            }
            
        }
        
    }

    void Update()
    {
        
        
        if (!stuck && this.transform.tag != "Broken")
        {
            Move();
            CancelInvoke();
            firstCall = true;
        }

        if(stuck && moved != 0 && firstCall && this.transform.tag != "Broken")
        {
            InvokeRepeating("Lost", 2.0f, 1);
            firstCall = false;
        }
        if (stuck && moved == 0 && this.transform.tag != "Broken")
        {
            startUp(currentLoc.GetComponent<Station>().getSignals());
        }
    }

    public GameObject getNextStation()
    {
        return route[nextLoc];
    }
    public GameObject[] stationsLeft()
    {
        int temp = nextLoc;
        bool loop = true;
        int count = 0;
        int tempDirect = direction;
        GameObject[] tempList = new GameObject[route.Length];
        while (loop)
        {
           
            if (temp == 0 && tempDirect == -1)
            {
                if(count == 0)
                {
                    tempDirect = 1;
                }
                else
                {
                    tempList[count] = route[temp];
                    loop = false;
                }
                
            }
            else if (temp == route.Length - 1 && tempDirect == 1)
            {
                if(count == 0)
                {
                    tempDirect = -1;
                }
                else
                {
                    tempList[count] = route[temp];
                    loop = false;
                }
            }
            else
            {
                tempList[count] = route[temp];
                temp = temp + tempDirect;
                count += 1;
            }
        }
    
        
        GameObject[] statsLeft = new GameObject[count + 1] ;
        for(int i = 0; i < count + 1;  i ++)
        {
            statsLeft[i] = tempList[i];
        }
  
        return statsLeft;
    }
    public GameObject getPreviousStation()
    {
        if(nextLoc == 0)
        {
            return route[1];
        }
        else if(nextLoc == route.Length - 1)
        {
            return route[nextLoc - 1];
        }
        else
        {
            return route[nextLoc - direction];
        }
        
    }
    public GameObject getCurrentloc ()
    {
        return currentLoc; 
    }

}
