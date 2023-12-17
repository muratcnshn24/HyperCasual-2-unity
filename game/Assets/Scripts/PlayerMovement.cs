using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    private float velocity;
    private Camera mainCam;
    public float roadEndPoint;

    private Transform player;
    private Vector3 firtsMousPos, firstPlayerPos;
    private bool moveTheBall;

    private float camVelocity;
    public float camSpeed = 0.4f;
    private Vector3 offset;


    public float playerzSpeed = 15f;


    public GameObject bodyPrefab;
    public int gap = 2;
    public float bodySpeed = 15f;

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<int> bodyPartsIndex = new List<int>();
    private List<Vector3> positionHistory =new List<Vector3>();






    // Start is called before the first frame update



    void Start()
        {
            mainCam = Camera.main;
            player = this.transform;
            offset = mainCam.transform.position - player.position;

        }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            moveTheBall = true;
        
        }

        else if (Input.GetMouseButtonUp(0))
        {
            moveTheBall = false;

        }

        if (moveTheBall) 
        {

            Plane newPlane = new Plane(Vector3.up, 5f);
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (newPlane.Raycast(ray, out var distance))
            {
                Vector3 newMousePos = ray.GetPoint(distance) - firtsMousPos;
                Vector3 newPlayerPos = newMousePos + firstPlayerPos;
                newPlayerPos.x = Mathf.Clamp(newPlayerPos.x, -roadEndPoint, roadEndPoint);
                player.position = new Vector3(Mathf.SmoothDamp(player.position.x, newPlayerPos.x, ref velocity, speed* Time.deltaTime), player.position.y,
                player.position.z);

            }
        
        
        
        
        
        
        }
    }

    private void FixedUpdate()
    {
        player.position += Vector3.forward * playerzSpeed * Time.deltaTime;

        positionHistory.Insert(0, transform.position);
        int index = 0;
        List<GameObject> validBodyParts = new List<GameObject>();

        foreach (var body in bodyParts)
        {
      
            if (body != null)
            {
                Vector3 point = positionHistory[Mathf.Min(index * gap, positionHistory.Count - 1)];
                Vector3 moveDir = point - body.transform.position;
                body.transform.position += moveDir * bodySpeed * Time.fixedDeltaTime;
                body.transform.LookAt(point);
                index++;
                validBodyParts.Add(body);
            }
        }

       
        bodyParts = validBodyParts;
    }



    private void LateUpdate()
    {
        Vector3 newCamPos = mainCam.transform.position;
        mainCam.transform.position = new Vector3(Mathf.SmoothDamp(newCamPos.x, player.position.x, ref camVelocity, camSpeed ), newCamPos.y, player.position.z + offset.z);
    }

    public void GrowBody()
    {
      
        if (bodyPrefab != null)
        {
            GameObject body = Instantiate(bodyPrefab, transform.position, transform.rotation);
            bodyParts.Add(body);
            int index = 0;
            index++;
            bodyPartsIndex.Add(index);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CellObs")
        {
      
           if (bodyPrefab != null)
            {
                Destroy(other.gameObject, 0.005f);
                GrowBody();
            }
        }
    }

}
