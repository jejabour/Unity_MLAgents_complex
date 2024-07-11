using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class move_to_target : Agent
{

    [SerializeField] private Transform env;
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;

    [SerializeField] Transform capsule_1;
    [SerializeField] Transform capsule_2;


    public override void OnEpisodeBegin()
    {
        target.localPosition = new Vector3(Random.Range(0f, 2.5f), Random.Range(4f, -4f));
        transform.localPosition = new Vector3(Random.Range(-4f, -1.25f), Random.Range(4f, -4f));

        env.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        transform.rotation = Quaternion.identity;

        if (capsule_1.gameObject.activeSelf != true){
            capsule_1.gameObject.SetActive(true);
        }

        if (capsule_2.gameObject.activeSelf != true){
            capsule_2.gameObject.SetActive(true);
        }
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation((Vector2)target.localPosition);
        sensor.AddObservation((Vector2)transform.localPosition);

        if (capsule_1.gameObject.activeSelf == true){
            sensor.AddObservation((Vector2)capsule_1.localPosition);
        }

        if (capsule_2.gameObject.activeSelf == true){
            sensor.AddObservation((Vector2)capsule_2.localPosition);
        }


    }


    public override void OnActionReceived(ActionBuffers actions){
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];


        float movementSpeed = 5f;

        transform.localPosition += new Vector3(moveX, moveY) * Time.deltaTime * movementSpeed;

        AddReward((-1f / MaxStep) * 2);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        
        if (collision.TryGetComponent(out Target target)){
            AddReward(10f);
            backgroundSpriteRenderer.color = new Color(0.043f, 0.718f, 0.05f, 1f);
            EndEpisode();
        }

        else if(collision.TryGetComponent(out Wall wall)){
            AddReward(-2f);
            backgroundSpriteRenderer.color = new Color(0.86f, 0.16f, 0.05f, 1f);
            EndEpisode();
        }

        if (collision.TryGetComponent(out Capsule capsule)){
            
            AddReward(5f);
            capsule.gameObject.SetActive(false);
        
        }


    }

}
