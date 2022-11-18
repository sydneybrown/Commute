using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementController : MonoBehaviour
{
	public bool move = true;
	public bool direction = true;
	public float speed = 4f;
    private bool slow = false;
    public bool freeze = false;
    private SceneController sceneController;
	
    // Start is called before the first frame update
    void Start()
    {
    	if (transform.position.z > 1 || transform.position.z < -1)
    		speed--;

    	if(transform.position.z > 0)
    	{
    		direction = true;
    		transform.position = new Vector3(UnityEngine.Random.Range(-11,-8),-4.5f,0);
    	}
    	else if(transform.position.z < 0)
    	{
    		direction = false;
    		transform.position = new Vector3(UnityEngine.Random.Range(9,12),-4.5f,0);
    	}
    	else if(UnityEngine.Random.Range(0,2) == 1)
    	{
    		direction = true;
    		transform.position = new Vector3(-10,-4.5f,0);
    	}
    	else
    	{
    		direction = false;
    		transform.position = new Vector3(10,-4.5f,0);
    	}

        if(Input.GetKey(KeyCode.S))
            slow = true;

        sceneController = GameObject.Find("Main Camera").GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
    	if(move && !sceneController.isFrozen(Time.deltaTime))
    	{
            if(Input.GetKeyDown(KeyCode.S))
            {
                slow = true;
            }
            if(Input.GetKeyUp(KeyCode.S))
            {
                slow = false;
            }
    		if(Input.GetKeyUp("space"))
			{
				speed += 1f;
			}
	    	if(direction)
	    	{
	    		if(transform.position.x < 8)
                {
                    if(slow)
                        transform.position += new Vector3(Math.Max(2f,speed-(speed/2)) * Time.deltaTime,0,0);
                    else
                        transform.position += new Vector3(speed * Time.deltaTime,0,0);
                }               
	    		else
	    			direction = false;
	    	}
	    	else
	    	{
	    		if(transform.position.x > -8)
	    		{
                    if(slow)
                        transform.position += new Vector3(-Math.Max(2f,speed-(speed/2)) * Time.deltaTime,0,0);
                    else
                        transform.position += new Vector3(-speed * Time.deltaTime,0,0);
                } 
	    		else
	    			direction = true;
	    	}
    	}
    }
}
