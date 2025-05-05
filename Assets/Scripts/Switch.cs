using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] GameObject[] connection = new GameObject[2];
    [SerializeField] GameObject[] Tracks;
    [SerializeField] string[] directions;
    [SerializeField] GameObject[] signals;
    [SerializeField] GameObject[] linksStations;
    [SerializeField] string switchType;
    private bool broken = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int count = 0;
        if (switchType == "dual")
        {
            directions = new string[linksStations.Length];
            foreach (GameObject station in linksStations)
            {
                directions[count] = directionCal(this.gameObject, station);
                count += 1;
            }
        }
        else if (switchType == "single")
        {
            directions = new string[Tracks.Length];
            foreach (GameObject track in Tracks)
            {
                directions[count] = directionCal(this.gameObject, track);
                count += 1;
            }
        }
    }
    public GameObject[] getSignals ()
    {
        return signals;
    }
    public bool switchCheck(GameObject currentLoc, GameObject nextTrack, GameObject Train)
     {
        if (switchType == "dual")
        {
            if(Train.GetComponent<Movement>().getNextStation() == connection[0] || Train.GetComponent<Movement>().getNextStation() == connection[1] && Train.GetComponent<Movement>().getPreviousStation() == connection[0] || Train.GetComponent<Movement>().getPreviousStation() == connection[1])
            {
                return true;
            }
            {
                return setConnection(Train.GetComponent<Movement>().getPreviousStation(), Train.GetComponent<Movement>().getNextStation());
            }
        }
        else
        {
            if (currentLoc == connection[0] || currentLoc == connection[1] && nextTrack == connection[0] || nextTrack == connection[1])
            {
                return true;
            }
            else
            {
                return setConnection(currentLoc, nextTrack);
            }
        }
     }

    private bool setConnection(GameObject connectionItem, GameObject dest)
    { 
        if(!broken)
        {
            connection[0] = connectionItem;
            connection[1] = dest;
            return true;
        }
        else
        {
            return false;
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
    public GameObject[] getLinks()
    {
        return linksStations;
    }
    // Update is called once per frame
 
}

