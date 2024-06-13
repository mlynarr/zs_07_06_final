using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI; // Import potrzebny do używania NavMeshAgent

public class zombieMove : MonoBehaviour
{
    public Transform target; // Cel, za którym AI będzie podążać (gracz)
    public float detectionRange = 10.0f; // Zasięg wykrycia
    public float patrolSpeed = 2.0f; // Szybkość patrolowania
    public float chaseSpeed = 4.0f; // Szybkość ścigania
    public float patrolRadius = 20.0f; // Promień, w którym zombie może się poruszać podczas patrolu

    private NavMeshAgent agent;
    private Vector3 startingPosition;
    private float patrolTimer; // Timer do zmiany punktu patrolu

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position; // Zapamiętaj początkową pozycję do celów patrolowania
        patrolTimer = Random.Range(5, 10); // Losuj czas, po którym zmieni się cel patrolu
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget < detectionRange)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.speed = patrolSpeed;
            Patrol();
        }
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime; // Odejmuj czas od timera

        if (patrolTimer <= 0)
        {
            Vector3 newPatrolPoint = RandomNavSphere(startingPosition, patrolRadius, -1);
            agent.SetDestination(newPatrolPoint);
            patrolTimer = Random.Range(5, 10); // Resetuj timer
        }
    }

    // Metoda do generowania losowego punktu do patrolu w obrębie określonego promienia
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startingPosition, patrolRadius);
    }
}
