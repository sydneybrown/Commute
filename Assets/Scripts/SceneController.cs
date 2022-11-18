using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
	private List<GameObject> triangles;
	private List<GameObject> circles;
	private GameObject triangleBuilding;
	private GameObject circleBuilding;
    private BuildingController triangleBuildingController;
    private BuildingController circleBuildingController;
	public GameObject prefabTrianglePerson;
	public GameObject prefabCirclePerson;
    public GameObject prefabStartRemnant;
    private MovementController triangleMovementController;
    private MovementController circleMovementController;
    private int pressCount = 0;
    private Camera cam;
    private float time_person = 0;
    private float time_color = 0;
    private const int MAX_PEOPLE = 6;
    private bool preStart = true;
    private bool gameStart = false;
    private bool gameEnd = false;
    private bool moveComplete = false;
    private List<GameObject> scoreSquares;
    public float speed = 4f;
    public float cameraSpeed = 12f;
    private bool frozen = false;
    private float freezeDuration;
    private AudioSource endAudio;
    private AudioSource startAudio;
    public AudioClip goodEnd;
    public AudioClip badEnd;
    public AudioClip startSound;
    int min = 2;
    int max = 5;


    // Start is called before the first frame update
    void Start()
    {
    	triangles = new List<GameObject>();
    	circles = new List<GameObject>();
        cam = GetComponent<Camera>();

    	triangleBuilding = GameObject.Find("TriangleBuilding");
    	circleBuilding = GameObject.Find("CircleBuilding");

        triangleBuildingController = triangleBuilding.GetComponent<BuildingController>();
        circleBuildingController = circleBuilding.GetComponent<BuildingController>();

        triangleMovementController = prefabTrianglePerson.GetComponent<MovementController>();
        circleMovementController = prefabCirclePerson.GetComponent<MovementController>();
        triangleMovementController.speed = speed;
        circleMovementController.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameEnd)
        {
            if(!moveComplete)
            {
                if(cam.transform.position.y < 8)
                {
                    cam.transform.position += new Vector3(0,cameraSpeed * Time.deltaTime,0);
                }
                else
                    moveComplete = true;
            }
            else
            {
                if(Input.GetKeyUp("space"))
                {
                    triangleMovementController.speed = speed;
                    circleMovementController.speed = speed;
                    Application.Quit();
                }   
            }
        }
        else if(preStart)
        {
            if(Input.GetKeyDown("space"))
            {
                preStart = false;
                gameStart = true;
                PlayStartSound();
                Destroy(GameObject.Find("Start"));
                GameObject startRemnant = (GameObject)Instantiate(prefabStartRemnant);
                Destroy(startRemnant,2f);
            }
        }
        else if(gameStart)
        {
            if(cam.transform.position.x < 0)
            {
                cam.transform.position += new Vector3(cameraSpeed * Time.deltaTime,0,0);
            }
            else
            {
                gameStart = false;
                AddPerson();
            }

        }
        else
        {
             //end game conditions:
            // > certain number presses
            // all windows filled
            // or no windows left for appropriate shape
            int triTotal = triangleBuildingController.total;
            int circTotal = circleBuildingController.total;
            time_color += Time.deltaTime;
            time_person += Time.deltaTime;
            if(triTotal >= MAX_PEOPLE && circTotal >= MAX_PEOPLE ||
               pressCount >= MAX_PEOPLE * 2 ||
               triTotal >= MAX_PEOPLE && triangleBuildingController.num_bad + circleBuildingController.num_good >= MAX_PEOPLE ||
               circTotal >= MAX_PEOPLE && circleBuildingController.num_bad + triangleBuildingController.num_good >= MAX_PEOPLE)
            {
                int score = triangleBuildingController.num_good + circleBuildingController.num_good;
                SetUpScore((score - 6) / 2);
                PlayEndSound((score - 6) / 2);
                gameEnd = true;
            }
            else
            {
                if (time_person > Random.Range(min,max))
                {
                    AddPerson();
                    time_person = 0;
                }

                if(time_color > 0.5f)
                {
                    time_color = 0;
                    Color current = cam.backgroundColor;
                    current.g -= 0.01f;
                    cam.backgroundColor = current;
                }
                
                if(Input.GetKeyUp("space"))
                {
                    pressCount++;
                    UpdateMinMax();
                }
            }   
        }  
    }

    void AddPerson()
    {
        triangleMovementController.speed += 0.75f;
        circleMovementController.speed += 0.75f;
        int rand = Random.Range(0,12);
            if(circles.Count < MAX_PEOPLE - 1 && circles.Count < triangles.Count && rand > 7)
            {
                if(rand < 10)
                    circles.Add((GameObject)Instantiate(prefabCirclePerson, new Vector3(-10,-4.5f,1), Quaternion.identity));
                else
                {
                    circles.Add((GameObject)Instantiate(prefabCirclePerson, new Vector3(10,-4.5f,2), Quaternion.identity));
                    circles.Add((GameObject)Instantiate(prefabCirclePerson, new Vector3(-10,-4.5f,-1), Quaternion.identity));
                }
            }
            else if (triangles.Count < MAX_PEOPLE - 1 && triangles.Count < circles.Count && rand > 7)
            {
                if(Random.Range(0,3) < 2)
                    triangles.Add((GameObject)Instantiate(prefabTrianglePerson, new Vector3(-10,-4.5f,0), Quaternion.identity));
                else
                {
                    triangles.Add((GameObject)Instantiate(prefabTrianglePerson, new Vector3(10,-4.5f,1), Quaternion.identity));
                    triangles.Add((GameObject)Instantiate(prefabTrianglePerson, new Vector3(-10,-4.5f,-2), Quaternion.identity));
                }
            }
            else if(circles.Count < MAX_PEOPLE && triangles.Count < MAX_PEOPLE)
            {
                if(rand == 7)
                {
                    triangles.Add((GameObject)Instantiate(prefabTrianglePerson, new Vector3(10,-4.5f,1), Quaternion.identity));
                    circles.Add((GameObject)Instantiate(prefabCirclePerson, new Vector3(-10,-4.5f,-1), Quaternion.identity));

                }
                else if(rand == 6)
                {
                    triangles.Add((GameObject)Instantiate(prefabTrianglePerson, new Vector3(10,-4.5f,-1), Quaternion.identity));
                    circles.Add((GameObject)Instantiate(prefabCirclePerson, new Vector3(-10,-4.5f,1), Quaternion.identity));
                }
                else if(rand > 2)
                   triangles.Add((GameObject)Instantiate(prefabTrianglePerson, new Vector3(10,-4.5f,0), Quaternion.identity));
                else
                   circles.Add((GameObject)Instantiate(prefabCirclePerson, new Vector3(-10,-4.5f,0), Quaternion.identity));
            }
            else if(circles.Count < MAX_PEOPLE)
            {
                circles.Add((GameObject)Instantiate(prefabCirclePerson, new Vector3(-10,-4.5f,0), Quaternion.identity));
            }
            else if(triangles.Count < MAX_PEOPLE)
            {
                triangles.Add((GameObject)Instantiate(prefabTrianglePerson, new Vector3(10,-4.5f,0), Quaternion.identity));
            }
    }

    void UpdateMinMax()
    {
        if(pressCount > 4)
            max--;
        if(pressCount > 7)
            min--;
        if(pressCount > 10)
            max--;
    }       

    void SetUpScore(int num)
    {
        scoreSquares = new List<GameObject>();
        for(int i = 0; i < 3; i++)
        {
            GameObject square = GameObject.Find("Score" + i);
            scoreSquares.Add(square);
            if(num >= (i+1))
            {
                square.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else if(num <= -i-1)
            {
                square.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }

    void PlayEndSound(int num)
    {
        endAudio = this.gameObject.AddComponent<AudioSource>();
        endAudio.volume = 0.5f;
        if(num > 0)
            endAudio.clip = goodEnd;
        else
            endAudio.clip = badEnd;
        endAudio.Play();
    }

    void PlayStartSound()
    {
        startAudio = this.gameObject.AddComponent<AudioSource>();
        startAudio.clip = startSound;
        startAudio.volume = 0.3f;
        startAudio.Play();
    }

    public void Freeze(float duration)
    {
        frozen = true;
        freezeDuration = duration;
    }

    public bool isFrozen(float dt)
    {
        if(frozen)
        {
            freezeDuration -= dt;
            if(freezeDuration <= 0)
            {
                frozen = false;
            }
        }
        return frozen;
    }

}
