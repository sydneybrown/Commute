using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemnantController : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    	Color c = spriteRenderer.color;
    	if(c.a > Time.deltaTime)
    	{
    		c.a -= Time.deltaTime;
    		spriteRenderer.color = c;

    	}
        
    }
}
