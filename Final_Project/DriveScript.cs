using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UI;

public class DriveScript : Agent
{
    // Initial positions and parameters
    Vector3 StartPosition;
    Quaternion StartingRotation;
    float startDistance = 0;

    // Movement parameters
    float speed = 0.2f;
    float turnSpeed = 60.0f;
    float maxSpeed = 10.0f;

    // Components and game objects
    Rigidbody m_Rigidbody;
    public GameObject GoalDistanse;
    public GameObject Waypoint;
    public GameObject C_point;
    public GameObject tresure;
    public GameObject StartDistance;

    // Variables for observations
    Vector3 carPosition;
    float carSpeed;
    float updatedDistance;

    // Timer for training episodes
    float trainingTimer = 0;

    // UI element for text information
    private Text TextInfo;

    // Unity ML-Agents callback for receiving actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("Listening");

        // Penalty for time elapsed
        AddReward(-0.0025f);

        var discreteActions = actions.DiscreteActions;

        // Act based on discrete actions
        if (discreteActions[0] == 1) { DriveForward(); ScoreCloserToGoal(); }
        if (discreteActions[0] == 2) { DriveBackwards(); ScoreCloserToGoal(); }
        if (discreteActions[1] == 1) { LeftDrive(); }
        if (discreteActions[1] == 2) { RightDrive(); }
    }

    // Unity ML-Agents callback for collecting observations
    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("Listening");

        // Collect relevant observations
        sensor.AddObservation(transform.localPosition); // Car position (3 x float)
        sensor.AddObservation(Waypoint.transform.localPosition); // Waypoint position (3 x float)
        sensor.AddObservation(C_point.transform.localPosition); // Checkpoint position (3 x float)
        sensor.AddObservation(m_Rigidbody.velocity); // Car velocity (3 x float)
        sensor.AddObservation(Vector3.Distance(transform.localPosition, Waypoint.transform.localPosition)); // Distance to waypoint (1 x float)
        sensor.AddObservation(transform.rotation); // Car rotation (4 x float)
    }

    // Unity callback for initialization
    void Start()
    {
        // Initialize starting positions and rotation
        StartPosition = transform.localPosition;
        StartingRotation = transform.localRotation;

        // Initialize rigidbody and start distance
        m_Rigidbody = GetComponent<Rigidbody>();
        startDistance = GetGoalDistance(GoalDistanse);

        // Initialize cylinders (points)
        StartAllCylinders();
    }

    // Initialize all cylinders (points) to predefined positions
    void StartAllCylinders()
    {
        // Find all point game objects with the "point" tag
        GameObject[] points = GameObject.FindGameObjectsWithTag("point");

        // Define preset coordinates for cylinders
        float[] xCoordinates = new float[] { -3.7f, -25.9f, -36.6f };
        float[] yCoordinates = new float[] { 95f, 90f, 90f };
        float[] zCoordinates = new float[] { 189f, 125f, 83f };

        // Set positions for each point
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 newPosition = new Vector3(xCoordinates[i], yCoordinates[i], zCoordinates[i]);
            points[i].transform.localPosition = newPosition;
        }
    }

    // Unity callback for fixed update
    private void FixedUpdate()
    {
        // Update distance to goal
        updatedDistance = GetGoalDistance(GoalDistanse);

        // Check if agent is getting closer or farther from the goal
        if (updatedDistance < startDistance)
        {
            AddReward(0.1f); // Add reward if getting closer to the goal
            trainingTimer = 0;
            startDistance = updatedDistance;
        }
        else
        {
            AddReward(-0.1f); // Penalize if moving away from the goal
            trainingTimer++;
        }

        // End episode if training time exceeds a threshold
        if (trainingTimer > 500)
        {
            trainingTimer = 0;
            EndEpisode();
        }

        // Update car position and speed
        var vel = m_Rigidbody.velocity;
        carPosition = transform.position;
        carSpeed = vel.magnitude;

        // Request decision for the next action
        RequestDecision();
    }

    // Unity callback for the beginning of a new episode
    public override void OnEpisodeBegin()
    {
        // Reset the agent's position and rotation
        Reset();
    }

    // Reset the agent's position and rotation
    void Reset()
    {
        transform.localPosition = StartPosition;
        transform.rotation = StartingRotation;
        m_Rigidbody.velocity = Vector3.zero;
        StartAllCylinders();
    }

    // Calculate the distance to the goal
    float GetGoalDistance(GameObject name)
    {
        return Vector3.Distance(transform.position, name.transform.position);
    }

    // Score based on proximity to the goal
    void ScoreCloserToGoal()
    {
        updatedDistance = GetGoalDistance(GoalDistanse);

        if (updatedDistance < startDistance)
        {
            AddReward(0.1f); // Add reward if getting closer to the goal
            startDistance = updatedDistance;
        }
        if (updatedDistance > startDistance)
        {
            AddReward(-0.1f); // Penalize if moving away from the goal
            startDistance = updatedDistance;
        }
    }

    // Output text information to UI
    void TextInfoOutput(string info)
    {
        TextInfo.text = info;

        // Modify text position
        RectTransform rectTransform = TextInfo.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(100f, 200f); // Set X and Y position
    }

    // Unity ML-Agents callback for heuristic (manual) control
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Listening");

        // Penalty for time elapsed
        AddReward(-0.0025f);

        var discreteActions = actionsOut.DiscreteActions;

        // Initialize actions to zero
        discreteActions[0] = 0;
        discreteActions[1] = 0;

        // Check input keys and set corresponding actions
        if (Input.GetKey(KeyCode.UpArrow))
            discreteActions[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow))
            discreteActions[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow))
            discreteActions[1] = 1;
        if (Input.GetKey(KeyCode.RightArrow))
            discreteActions[1] = 2;
    }

    // Move the car to the left
    void LeftDrive()
    {
        transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
        m_Rigidbody.velocity = transform.forward * m_R
