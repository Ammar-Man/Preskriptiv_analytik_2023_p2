using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DriveScript : Agent
{
    // Start is called before the first frame update
    Vector3 StartPosition;
    Vector3 PointStartPosition;
   

    float speed = 0.2f;
    float turnSpeed = 60.0f;
    public float AddForceSpeed = 20f;
    public float maxSpeed = 10.0f;
    Rigidbody m_Rigidbody;
    public GameObject GoalDistanse;
    public GameObject StartDistance;
    public GameObject tresure;
    public GameObject C_point;

    public Vector3 carPosition;
   public float carSpeed;
   public float updatedDistance;
    public float startDistance = 0;
    public GameObject Waypoint;
    Quaternion StartingRotation;
    GameObject[] point;
    private Text TextInfo;



    //public GameObject Cylinder;
    // GameObject[] AllCylinder; 

    public override void OnActionReceived(ActionBuffers actions)
    {
         Debug.Log("Lyssnar");
        //getGoalDistance(GoalDistanse);
        
        AddReward(-0.0025f);
        
        var discreteActions = actions.DiscreteActions;
        if (discreteActions[0] == 1) { DriveForward();   Score_Closer_To_Goal();
        }
        if (discreteActions[0] == 2) { DriveBackwards();     Score_Closer_To_Goal();
        }
        if (discreteActions[1] == 1) {leftDrive(); 
        }
        if (discreteActions[1] == 2){ RightDrive(); 
        }
        
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("Lyssnar");
        //Observera att vår kurslärare inte testat att alla dessa observationsvärden verkligen behövs :) Experimentera gärna på egen hand :)
        
        sensor.AddObservation(transform.localPosition); //bilens position (3 x float)
        sensor.AddObservation(Waypoint.transform.localPosition); //vägpunktens position (3 x float)
       // sensor.AddObservation(point.transform.localPosition); //vägpunktens position (3 x float)
       
            sensor.AddObservation(C_point.transform.localPosition);
        
        sensor.AddObservation(m_Rigidbody.velocity); //bilens hastighet (3 x float)
        sensor.AddObservation(Vector3.Distance(transform.localPosition, Waypoint.transform.localPosition)); //avståndet mellan bilen och vägpunkten (1 x float)
        sensor.AddObservation(transform.rotation); //bilens rotationsvinkel (4 x float)
        //Som observationsvärden skickar vi totalt 14 flyttalsvärden till neuronnätet. Denna inställning bör göras i Behavour Parameter skriptet för agenten. 

        
    }


    void Start()
    {
       
        StartPosition = transform.localPosition;
        StartingRotation = transform.localRotation;
        Debug.Log("Start");
        m_Rigidbody = GetComponent<Rigidbody>();
        startDistance = getGoalDistance(GoalDistanse);
        StartAllCylinds();
    }

    void StartAllCylinds()
    {
        point = GameObject.FindGameObjectsWithTag("point");
        float[] xCoordinates = new float[] { -3.7f, -25.9f, -36.6f };
        float[] yCoordinates = new float[] { 95f, 90f, 90f };
        float[] zCoordinates = new float[] { 189f, 125f, 83f };
        for (int i = 0; i < point.Length; i++)
        {
            Vector3 newPosition = new Vector3(xCoordinates[i], yCoordinates[i], zCoordinates[i]);
            point[i].transform.localPosition = newPosition;
        }
    }

    // Update is called once per frame
    float trainingTimer = 0;
    private void FixedUpdate()
    {
        updatedDistance = getGoalDistance(GoalDistanse);
        if (updatedDistance < startDistance)
        {
            AddReward(0.1f); // Lägg till poäng om agenten rör sig närmare målet
            Debug.Log("go score: 0.1f");
            //Debug.Log("går mot målet");
            trainingTimer = 0;
            startDistance = updatedDistance;
        }
        else
        {
            Debug.Log("back -score: -0.1f");
            AddReward(-0.1f);
            // Annars om den inte blivit bättre så öka countern.
            trainingTimer++;
        }
        if (trainingTimer > 500)
        {
            Debug.Log("stopat mera än 500 sec. -0.1");
           
            trainingTimer = 0;
            EndEpisode();
        }

        var vel = m_Rigidbody.velocity;
        carPosition = this.transform.position;
        carSpeed = vel.magnitude;
        RequestDecision();
    }
    public override void OnEpisodeBegin()
    {
        Reset();
    }
    void Reset()
    {
        transform.localPosition = StartPosition;
        transform.rotation =StartingRotation;
        m_Rigidbody.velocity= Vector3.zero;
        StartAllCylinds();
    }
    float getGoalDistance(GameObject name)
    {
        float dist = Vector3.Distance(transform.position, name.transform.position);
        return dist;
    }
   
    void Score_Closer_To_Goal()
    {
        updatedDistance = getGoalDistance(GoalDistanse);

        if (updatedDistance < startDistance)
        {
            AddReward(0.1f); // Lägg till poäng om agenten rör sig närmare målet
            Debug.Log("go score: 0.1f");
            startDistance = updatedDistance;
        }
        if (updatedDistance > startDistance)
        {
            Debug.Log("back -score: -0.1f");
            AddReward(-0.1f); // Lägg till poäng om agenten rör sig närmare målet
           // Debug.Log("Distance decreased! Current score: -0.1f");
            startDistance = updatedDistance;
        }

    }

    void TextInfoOutput(String info)
    {
       
        TextInfo.text= info;
        // Modify text position
        RectTransform rectTransform = TextInfo.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(100f, 200f); // Set X and Y position
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Lyssnar");
        
        AddReward(-0.0025f);

        var discreteActions = actionsOut.DiscreteActions;
        var continuousActions = actionsOut.ContinuousActions;

        discreteActions[0] = 0;
        discreteActions[1] = 0;

        if (Input.GetKey(KeyCode.UpArrow))
            discreteActions[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow))
            discreteActions[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow))
            discreteActions[1] = 1;
        if (Input.GetKey(KeyCode.RightArrow))
            discreteActions[1] = 2;
        
    }

    void leftDrive()
    {
        //this.transform.Rotate(0, - turnSpeed , 0);
        transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
        m_Rigidbody.velocity = transform.forward * m_Rigidbody.velocity.magnitude;
    }
    void RightDrive()
    {
        transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
        m_Rigidbody.velocity = transform.forward * m_Rigidbody.velocity.magnitude;
       // this.transform.Rotate(0, turnSpeed , 0);
    }
    void DriveForward()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        float currentSpeed = m_Rigidbody.velocity.magnitude;
        if (currentSpeed < maxSpeed)
        {
            Vector3 force = transform.forward * (maxSpeed - currentSpeed);
            m_Rigidbody.AddForce(force, ForceMode.VelocityChange);
        }

    }
    void DriveBackwards()
    {
      
        float currentSpeed = m_Rigidbody.velocity.magnitude;
        if (currentSpeed < maxSpeed)
        {
            Vector3 force = transform.forward * (maxSpeed - currentSpeed);
            m_Rigidbody.AddForce(force*-1, ForceMode.VelocityChange);
        }
    }
  

    void RandomPosition()
    {
        GameObject[] allCylinders = GameObject.FindGameObjectsWithTag("waypoint");
        foreach (GameObject cylinder in allCylinders)
        {
            float x = UnityEngine.Random.Range(-30, 4);
            float z = UnityEngine.Random.Range(70, 150);
            cylinder.transform.localPosition = new Vector3(x, transform.localPosition.y, z);
        }
    }
    void otherRandomPosition(Collider name)
    {
        float x = UnityEngine.Random.Range(-30, 4);
        float z = UnityEngine.Random.Range(70, 150);
        name.transform.localPosition = new Vector3(x, transform.localPosition.y, z);
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "wall")
        {
            Debug.Log("wall 0.1f)");
            AddReward(-2.0f);
            EndEpisode();
        }
        if (other.gameObject.tag == "point")
        {
            AddReward(0.9f);
            other.transform.localPosition = new Vector3(28, 90, 125);
            Debug.Log("point 0.9f)");
        }

        else if (other.gameObject.tag == "waypoint")
        {
            AddReward(5.0f);
            Debug.Log("goal 5.0f");
            EndEpisode();
        }

    }

    private void OnCollisionExit(Collision other)
 {

     if (other.gameObject.tag == "plane")
     {
            Debug.Log("out of plane");
            m_Rigidbody.velocity.Set(0, 0, 0);
            EndEpisode();
     }
 }

}
