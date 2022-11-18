using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
	public string shape;
	private List<GameObject> people_outside;
	private List<GameObject> people_inside;
	private List<GameObject> windows;
	public int num_good = 0;
	public int num_bad = 0;
	public int total = 0;
	private const int NUM_WINDOWS = 6;
    private AudioSource audioSource;
    public AudioClip chime;
    public AudioClip buzz;
    public GameObject prefabCirclePersonRemnant;
    public GameObject prefabTrianglePersonRemnant;
    private SceneController sceneController;
    // Start is called before the first frame update
    void Start()
    {
        people_outside = new List<GameObject>();
        people_inside = new List<GameObject>();
        windows = new List<GameObject>();
        for(int i = 0; i < NUM_WINDOWS; i++)
        	windows.Add(transform.Find("Window" + i).gameObject);
        audioSource = GetComponent<AudioSource>();
        sceneController = GameObject.Find("Main Camera").GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
    	if(Input.GetKeyDown("space") && people_inside.Count < NUM_WINDOWS && !Input.GetKey(KeyCode.S))
        {
            foreach(GameObject g in people_outside)
            {
            	if(total < NUM_WINDOWS)
            	{
            		people_inside.Add(g);

	                //change to position inside window
                    GameObject personRemnant;
                    if(g.name.Contains("Circle"))
	                   personRemnant = (GameObject)Instantiate(prefabCirclePersonRemnant, g.transform.position, Quaternion.identity);
                    else
                       personRemnant = (GameObject)Instantiate(prefabTrianglePersonRemnant, g.transform.position, Quaternion.identity);

                    Destroy(personRemnant,2f);

                    sceneController.Freeze(0.3f);
                    
	                if(g.name.Contains(shape))
	                {
                        audioSource.Stop();
                        audioSource.clip = chime;
                        audioSource.Play();
	                	num_good++;
	                	ChangeColor(g,true);
	                }
	                else
	                {
                        audioSource.Stop();
                        audioSource.clip = buzz;
                        audioSource.Play();
	                	num_bad++;
	                	ChangeColor(g,false);
	                }
                    g.transform.position = windows[total++].transform.position;
                    g.GetComponent<MovementController>().move = false;

            	}	
            }
            people_outside.Clear();
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    	people_outside.Add(other.gameObject);

    }

    void OnTriggerExit2D(Collider2D other)
    {
    	people_outside.Remove(other.gameObject);
    }

    void ChangeColor(GameObject g, bool isGood)
    {
    	if(isGood)
    		g.GetComponent<SpriteRenderer>().color = Color.green;
    	else
    		g.GetComponent<SpriteRenderer>().color = Color.red;
    }


}
