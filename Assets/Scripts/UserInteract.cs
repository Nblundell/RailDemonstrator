using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInteract : MonoBehaviour
{
    [SerializeField] GameObject[] Tracks;
    [SerializeField] GameObject[] signals;

    //[SerializeField] GameObject[] Tracks;
    //[SerializeField] GameObject[] Tracks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void resetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("s"))
        {
            transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 5, -1);
        }
        else if (Input.GetKey("w"))
        {
            transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 5, -1);
        }
        else if (Input.GetKey("d"))
        {
            transform.position = new Vector3(this.transform.position.x + 5, this.transform.position.y, -1);
        }
        else if (Input.GetKey("a"))
        {

            transform.position = new Vector3(this.transform.position.x - 5, this.transform.position.y, -1);
        }
        else if (Input.GetKey("q"))
        {
            GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 5;
        }
        else if (Input.GetKey("e"))
        {
            GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 5;
        }
        else if (Input.GetKeyDown("space"))
        {
            int rnd = UnityEngine.Random.Range(0, Tracks.Length);

            GameObject selectedObject = Tracks[rnd];
            selectedObject.transform.tag = "Broken";
        }
        else if (Input.GetKeyDown("r"))
        {
            int rnd = UnityEngine.Random.Range(0, signals.Length);

            GameObject selectedObject = signals[rnd];
            selectedObject.GetComponent<Signals>().breakSignal();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.x));
            float closestdistance = Mathf.Infinity;
            GameObject closestObject = null;
            Vector2 camPos = new Vector2(this.transform.position.x, this.transform.position.y);
            foreach (GameObject empty in Tracks)
            {
                float distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)), empty.transform.position);
                if (distance < closestdistance) // or <= ,i will explain 2 cases.
                {
                    closestdistance = distance;
                    closestObject = empty;
                }
            }
            closestObject.GetComponent<Track>().changeState();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.x));
            float closestdistance = Mathf.Infinity;
            GameObject closestObject = null;
            Vector2 camPos = new Vector2(this.transform.position.x, this.transform.position.y);
            foreach (GameObject empty in signals)
            {
                float distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)), empty.transform.position);
                if (distance < closestdistance) // or <= ,i will explain 2 cases.
                {
                    closestdistance = distance;
                    closestObject = empty;
                }
            }
            closestObject.GetComponent<Signals>().changeState();
        }
    } 
}
