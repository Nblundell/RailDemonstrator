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
    {   // checks if the train is able to interact with stations
        if (this.transform.tag != "Broken")
        {   //checks if the collided object is a station
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
            {  this.transform.tag = "Broken";
                //stuck set to true to prevent any train movement
                stuck = true;
                //rotates the train so it looks crashed
                this.transform.Rotate(0, 0, 30);
                //sets the track to broken which 
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
        //check the signals of the switch
        foreach (GameObject signal in signals)
        {

            if (found)
            {
                //exit the loop
                break;
            }
            GameObject track = signal.GetComponent<Signals>().getTrack();
            GameObject[] Links = track.GetComponent<Track>().getLinks();
            //loop through the links of the tracks
            foreach (GameObject link in Links)
            {

                if (signal.GetComponent<Signals>().getColour() == "Green" && link.transform.name == route[nextLoc].transform.name)
                {
                    //switch check
                    if (aSwitch.GetComponent<Switch>().switchCheck(currentLoc, track, this.gameObject))
                    {
                        //correct track is found
                        //work out next loc
                        calNextPos(route[nextLoc], track);
                        TrackChange(track);
                        found = true;
                        break;
                    }
                    
                }
                //checks the link if it is a switch but not if it is the current switch
                else if (link.transform.parent.name == "Switches" && link != aSwitch)
                {
                    
                    foreach (GameObject loc in Links)
                    {
                        if (loc != link)
                        {
                            GameObject[] switchLinks = link.GetComponent<Switch>().getLinks();
                            foreach (GameObject Station in switchLinks)
                            {
                                //checks if the signal is green and is going in the correct direction
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
            //not route was found
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
        //cals the differnce of next loc and current loc
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
                    //check the track at the start
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
            //looks at the signals
            foreach (GameObject signal in signals)
            {

                if (found)
                {
                    //looks for the link of each track
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
                //gets the track for the signal
                track = signal.GetComponent<Signals>().getTrack();
                //gets the linnks for the tracks
                Links = track.GetComponent<Track>().getLinks();

                foreach (GameObject link in Links)
                {

                    //check all conditions for the movement of the track
                    if (signal.GetComponent<Signals>().getColour() == "Green" && link.transform.name == route[nextLoc].transform.name)
                    {
                        calNextPos(route[nextLoc], track);
                        leaveStation(track);
                        signal.GetComponent<Signals>().joinedTrack();
                        found = true;
                        break;
                    }
                    //differnt check if on a switch or link is a switch
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
        //cals the next location the train is traveling to
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
        //as long as the train is broke it will wait
        if (transform.tag != "Broken")
        {
            yield return new WaitForSeconds(2);
        }
        //as long as the train isnt stuck the passengers board
        if (!stuck)
        {

            int count = 0;
            GameObject[] newPassangers = new GameObject[Capacity];
            foreach (GameObject passenger in passengers)
            {
                if (passenger != null)
                {
                    //checks for passengers leaving the trian
                    if (!passenger.GetComponent<People>().checkEndJourney(currentLoc))
                    {
                        newPassangers[count] = passenger;
                        count += 1;
                    }
                    else
                    {
                        //reduces passengers for each one leaving
                        numOfPassengers -= 1;
                    }
                }
            }
            passengers = newPassangers;
            if (numOfPassengers != Capacity)
            {
               //adds new passenger (max) up to capacity
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
            //looks for the track to the next loc
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
            //train is moving
            stuck = false;
            //removes the train from the track
            currentLoc.GetComponent<Track>().TrainOff(this.gameObject);
            //updates loc
            currentLoc = station;
            //starts in station wait
            StartCoroutine(InStation());
        }
        else
        {
            //train is stuck
            stuck = true;
        }
    }
    void TrackChange (GameObject newTrack)
    {
        //removes teh train for the track
        currentLoc.GetComponent<Track>().TrainOff(this.gameObject);
        //updates current loc
        currentLoc = newTrack;
        //updates speed
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
        //tells the station the train has left
        currentLoc.GetComponent<Station>().trainLeaving(this.gameObject);
        //updates  the location
        currentLoc = newTrack;
        //updates the trains speed based on the track
        speed = currentLoc.GetComponent<Track>().TrainOn(this.gameObject);
        stuck = false;
    }
  
    void Move()
    {
        //moves the train
        transform.position = Vector2.MoveTowards(transform.position, nextPostion, speed * Time.deltaTime);
    }

    void Lost()
    {

        //looks for a route for the train
        if (!stuck)
        {
            //cancels this invoke if train isnt stuck
            CancelInvoke();
            firstCall = true;
        }
        else
        {
            for (int i = 0; i < numOfPassengers; i++)
            {
                //adds to passenger delay
                passengers[i].GetComponent<People>().Delayed(1);
            }
            if (currentLoc.transform.parent.name == "Stations")
            {
                //looks for a track if at a station
                LookForTrack(currentLoc.GetComponent<Station>().getSignals());
            }
            else if (currentLoc.transform.parent.name == "Tracks")
            {
               
                if(waitingForSwitch != null)
                {
                    //waits to use a switch if stuck at a switch
                    onSwitch(waitingForSwitch);
                }
                else
                {
                    //attempts to enter a station if waiting for a station to empty
                    enterStation(route[nextLoc]);
                } 
            }
        }
        
    }
    void Update()
    {
        //checks each frame and only moves the train if it is not stuck
        if (!stuck && this.transform.tag != "Broken")
        {
            Move();
            CancelInvoke();
            firstCall = true;
        }
        //the train is stuck will start an invoke searching for a route
        if(stuck && moved != 0 && firstCall && this.transform.tag != "Broken")
        {
            InvokeRepeating("Lost", 2.0f, 1);
            firstCall = false;
        }
        //prevents train braking at the start  of the demo
        if (stuck && moved == 0 && this.transform.tag != "Broken")
        {
            startUp(currentLoc.GetComponent<Station>().getSignals());
        }
    }
    public GameObject getNextStation()
    {
        //retunrs the next station
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
            //checks the station is in the correct direction
            //then adds it to the temp list
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
        //formats the list and returns it
        GameObject[] statsLeft = new GameObject[count + 1] ;
        for(int i = 0; i < count + 1;  i ++)
        {
            statsLeft[i] = tempList[i];
        }
        return statsLeft;
    }
    public GameObject getPreviousStation()
    {
        //cals the previous station based on direction and route
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
        //returns current loc
        return currentLoc; 
    }

}
