
using TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.SideChannels;
using Random = UnityEngine.Random;

/// <summary>
/// A predator agent that learns to chase and catch a prey in a 2D arena.
/// Observations include relative position and velocities. Rewards are given for
/// catching the prey quickly and staying within bounds.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PredatorAgent : Agent
{
    public Transform preyTransform;
    public float speed = 5f;
    private Rigidbody rBody;
    private StringLogSideChannel logChannel;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.useGravity = false;
        rBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        logChannel = new StringLogSideChannel();
        SideChannelManager.RegisterSideChannel(logChannel);
        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        // Reset predator position and velocity
        rBody.angularVelocity = Vector3.zero;
        rBody.linearVelocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        // Reset prey via its script
        Prey prey = preyTransform.GetComponent<Prey>();
        if (prey != null)
        {
            prey.Respawn();
        }
        logChannel.Log("Episode started");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe relative position to prey
        sensor.AddObservation(preyTransform.localPosition - transform.localPosition);
        // Observe predator velocity
        sensor.AddObservation(rBody.linearVelocity);
        // Observe prey velocity if the prey has a Rigidbody
        Rigidbody preyRb = preyTransform.GetComponent<Rigidbody>();
        if (preyRb != null)
        {
            sensor.AddObservation(preyRb.linearVelocity);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Apply continuous actions: x and z components
        var continuousActions = actionBuffers.ContinuousActions;
        Vector3 force = new Vector3(continuousActions[0], 0, continuousActions[1]);
        rBody.AddForce(force * speed);
        // Small time penalty to encourage efficiency
        AddReward(-0.001f);
        // Distance to prey
        float distance = Vector3.Distance(transform.localPosition, preyTransform.localPosition);
        if (distance < 1.2f)
        {
            SetReward(1.0f);
            logChannel.Log("Caught the prey!");
            EndEpisode();
        }
        // Out-of-bounds penalty
        if (Mathf.Abs(transform.localPosition.x) > 5f || Mathf.Abs(transform.localPosition.z) > 5f)
        {
            SetReward(-1.0f);
            logChannel.Log("Went out of bounds");
            EndEpisode();
        }
        // Log distance as a statistic
        Academy.Instance.StatsRecorder.Add("Predator/DistanceToPrey", distance);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnDestroy()
    {
        if (logChannel != null)
        {
            SideChannelManager.UnregisterSideChannel(logChannel);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == preyTransform)
        {
            SetReward(1.0f);
            logChannel.Log("Caught the prey via collision");
            EndEpisode();
        }
    }
}
