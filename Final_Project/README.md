# Unity ML-Agents Agent Script

## Initialization and Parameters

- `Vector3 StartPosition;`: Stores the initial position of the agent.
- `Quaternion StartingRotation;`: Stores the initial rotation of the agent.
- `float startDistance = 0;`: Represents the initial distance to the goal.
- `float speed = 0.2f;`: Speed of the agent.
- `float turnSpeed = 60.0f;`: Turning speed of the agent.
- `float maxSpeed = 10.0f;`: Maximum speed of the agent.
- `Rigidbody m_Rigidbody;`: Rigidbody component of the agent for physics interactions.
- Various game objects (`GoalDistanse`, `Waypoint`, `C_point`, `tresure`, `StartDistance`) are public fields for referencing in the Unity Editor.

## Actions and Observations

- `OnActionReceived`: This method is called whenever the agent receives an action. It interprets discrete actions (e.g., move forward, move backward, turn left, turn right) and performs corresponding actions.
- `CollectObservations`: This method is used to collect observations from the environment for the agent. It includes information like the agent's position, waypoint position, checkpoint position, velocity, distance to the waypoint, and rotation.

## Start Method

- `Start`: Initializes the starting position, rotation, and other parameters. It also sets up the initial positions of the cylinders (points) using `StartAllCylinders`.

## StartAllCylinders Method

- `StartAllCylinders`: Positions the cylinders (points) at predefined coordinates. The coordinates are specified in three arrays (`xCoordinates`, `yCoordinates`, `zCoordinates`).

## FixedUpdate Method

- `FixedUpdate`: This method is called at fixed intervals and handles the main logic of the agent. It calculates the distance to the goal, rewards or penalizes the agent based on its movement, and checks for training time. It also updates the car's position and speed.

## OnEpisodeBegin Method

- `OnEpisodeBegin`: Called at the beginning of a new episode. It resets the agent's position and rotation.

## Reset Method

- `Reset`: Resets the agent's position and rotation to the initial state and stops its velocity.

## GetGoalDistance Method

- `GetGoalDistance`: Calculates the distance between the agent and a specified goal object.

## ScoreCloserToGoal Method

- `ScoreCloserToGoal`: Rewards or penalizes the agent based on its proximity to the goal.

## TextInfoOutput Method

- `TextInfoOutput`: Outputs text information to a UI element.

## Heuristic Method

- `Heuristic`: Provides a manual (heuristic) way of controlling the agent using keyboard inputs.

## Movement Methods

- `LeftDrive`, `RightDrive`, `DriveForward`, `DriveBackwards`: Methods that control the movement of the agent in different directions.

## RandomPosition Method

- `RandomPosition`: Randomly positions the cylinders (waypoints).

## otherRandomPosition Method

- `otherRandomPosition`: Randomly positions a specified collider.

## Collision and Trigger Methods

- `OnTriggerEnter`: Handles events when the agent enters a trigger collider. Rewards or penalizes the agent based on the collider's tag.
- `OnCollisionExit`: Handles events when the agent exits a collision. It stops the agent and ends the episode if it collides with a "plane" tag.
