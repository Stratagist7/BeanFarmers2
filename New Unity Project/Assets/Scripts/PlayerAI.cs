using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//Made by Ben Hamilton
public class PlayerAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject RM;
    [SerializeField] private GameObject FUI;
    [SerializeField] private Vector3 SellPoint;
    [SerializeField] private Vector3 FeedPoint;

    [SerializeField] private float WanderRadius = 5.0f;
    [SerializeField] private float WanderTime = 3.0f;
    [SerializeField] private float ActionDistance = 2.0f;
    [SerializeField] private int FeedWhenRicherThan = 100;

    public static bool HasSellableItems = false;

    private bool acting = false;
    private const float destinationDis = 1.5f;

    private void Start()
    {

    }

    private void Update()
    {
        if (!acting)
        {
            if (HasSellableItems)
            {
                acting = true;
                Sell();
            }

            else if (ResourceManager.money >= FeedWhenRicherThan)
            {
                acting = true;
                Feed();
            }

            else
            {
                acting = true;
                StartCoroutine(Wander());
            }
        }


        if (acting && Vector3.Distance(agent.destination, transform.position) <= destinationDis)
        {
            acting = false;
        }
    }

    // Wanders around based on the radius and distance of wanderRadius
    IEnumerator Wander()
    {
        //Debug.Log("Wandering");
        Vector3 randomDirection = Random.insideUnitSphere * WanderRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, WanderRadius, 1);
        Vector3 nextPosition = hit.position;

        agent.SetDestination(nextPosition);

        yield return new WaitForSeconds(WanderTime);
    }

    private void Sell()
    {
        //Debug.Log("Selling");
        agent.destination = SellPoint;
        if (Vector3.Distance(SellPoint, agent.transform.position) <= ActionDistance)
        {
            //Debug.Log("Arrived at Sell Point");
            RM.GetComponent<ResourceManager>().SellAll();
            HasSellableItems = false;
        }
    }

    private void Feed()
    {
        //Debug.Log("Feeding");
        agent.destination = FeedPoint;
        if (Vector3.Distance(FeedPoint, agent.transform.position) <= ActionDistance)
        {
            //Debug.Log("Arrived at Feed Point");
            FUI.GetComponent<FeedUI>().AIFeed();
            ResourceManager.money = 99;
        }
    }
}
