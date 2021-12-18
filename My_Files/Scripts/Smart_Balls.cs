using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Smart_Balls : MonoBehaviour
{
    public float dist;  // raycasting distance
    public float speed_H;   // horizontal force multiplier
    public float speed_V;   // vertical force
    public float speed_B;   // booster force
    private Rigidbody body;
    private Vector3 movement_V;
    private Vector3 movement_H;

    private bool f; // raycast_forward
    private bool r; // raycast_right
    private bool l; // raycast_left
    private bool fr; // raycast_front_right
    private bool fl; // raycast_front_left

    private bool flag;  // check if we have made a decision, so we do not overwrite it.

    private float cur_speed;    // our current vertical speed
    private int cur_place;  // our place on the X axis (horizontal)

    // Use this for initialization
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        movement_V = new Vector3(0.0f, 0.0f, 1.0f);
        movement_H = new Vector3(1.0f, 0.0f, 0.0f);


        f = false;
        r = false;
        l = false;
        fr = false;
        fl = false;

        flag = false;
        cur_speed = speed_V;

        cur_place = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        // Calculate distances for each raycast

        Vector3 forward_right = new Vector3(0.5f, 0, 1);
        Vector3 forward_left = new Vector3(-0.5f, 0, 1);
        Vector3 left = Vector3.left * 0.5f;
        Vector3 right = Vector3.right * 0.5f;
        Vector3 forward = Vector3.forward;
        float forward_dist;
        float side_dist;
        float angle_dist;
        float multiplier;
        float xVel;

        forward_dist = dist * 1.15f;

        multiplier = (float)Math.Sqrt(1 + (36 / (dist * dist)));
        angle_dist = dist * 1.15f;

        side_dist = (dist / 2);

        f = Double_Raycasting(forward, forward_dist);
        r = Raycasting(right, side_dist);
        l = Raycasting(left, side_dist);
        fr = Raycasting(forward_left, angle_dist);
        fl = Raycasting(forward_right, angle_dist);

        // Calculate our place on the X axis

        if (transform.position.x >= 0)
        {
            cur_place = 1;
        }
        else if (transform.position.x < 0)
        {
            cur_place = -1;
        }


        // Make sure we stay within limits of the X axis

        if (transform.position.x > 15)
        {
            transform.Translate((-transform.position.x + 13f), 0f, 0f);
        }

        if (transform.position.x < -15)
        {
            transform.Translate((transform.position.x - 13f), 0f, 0f);
        }

        // Make sure we stay within limits of the Y axis - inactive
        //if(transform.position.y != 0.5)
        //{
        //    transform.Translate(0f, -(0.5f - transform.position.y), 0f);
        //}


        // Calculate velocity on X axis;

        xVel = transform.InverseTransformDirection(body.velocity).x;

        if (f == false)
        {

            // No obstacle in front of us! We can move forward freely.
            // Velocity is applied only on the Z axis.
            // Velocity for X and Y axis is zero.

            flag = false;

            body.velocity = new Vector3(0.0f, 0.0f, speed_V);
        }
        else
        {
            // An obstacle in front of us! We must make a decision.
            // If front_left raycast is true, we go right.
            // If front_right raycast is true, we go left.
            // If they are both true, we go far towards the side that has the most space.
            // If they are both false, we go to the side that has the most space.

            if (fr == false && fl == true && flag == false)
            {
                flag = true;

                StartCoroutine(moveBall(1, speed_V));
            }
            else if (fr == true && fl == false && flag == false)
            {
                flag = true;

                StartCoroutine(moveBall(-1, speed_V));
            }
            else if (fr == false && fl == false && flag == false)
            {
                if (cur_place <= 0)
                {
                    flag = true;
                    StartCoroutine(moveBall(1, speed_V));
                }
                else if (cur_place > 0)
                {
                    flag = true;
                    StartCoroutine(moveBall(-1, speed_V));
                }
            }
            else if (fr == true && fl == true && flag == false)
            {
                if (cur_place <= 0)
                {
                    flag = true;
                    StartCoroutine(moveBall(2, speed_V));
                }
                else if (cur_place > 0)
                {
                    flag = true;
                    StartCoroutine(moveBall(-2, speed_V));
                }
            }
        }
    }


    // For both Raycasting functions, we follow the same logic.

    // We create a ray of distance max_dist and of direction way.
    // If our ray hits an Obstacle, which is lethal aka not a jumper, then return true.
    // Lastly, draw a debug ray to view during edit mode.

    private bool Raycasting(Vector3 way, float max_dist)
    {
        float distance;
        RaycastHit hit;

        way = way * max_dist;
        Ray ray = new Ray(transform.position, way);

        Debug.DrawRay(transform.position, way, Color.green);

        if (Physics.Raycast(ray, out hit, max_dist))
        {
            distance = hit.distance;
            if (hit.collider.gameObject.name.Contains("Wall") == true || hit.collider.gameObject.name.Contains("Obstacle") == true && hit.collider.gameObject.name.Contains("Jumper") == false)
            {
                return true;
            }
        }
        return false;
    }


    // Same logic as above, however we create 2 rays instead of 1.
    // The rays have an X offset of 0.6 (radius+0.1) from the center of the ball.

    private bool Double_Raycasting(Vector3 way, float max_dist)
    {
        float distance;
        RaycastHit hit;
        Vector3 origin1 = transform.position;
        Vector3 origin2 = transform.position;

        origin1.x = origin1.x - 0.6f;
        origin2.x = origin2.x + 0.6f;

        way = way * max_dist;
        Ray ray1 = new Ray(origin1, way);
        Ray ray2 = new Ray(origin2, way);

        Debug.DrawRay(origin1, way, Color.green);
        Debug.DrawRay(origin2, way, Color.green);

        if (Physics.Raycast(ray1, out hit, max_dist) || Physics.Raycast(ray2, out hit, max_dist))
        {
            distance = hit.distance;
            if (hit.collider.gameObject.name.Contains("Obstacle") == true && hit.collider.gameObject.name.Contains("Jumper") == false)
            {
                return true;
            }
        }
        return false;
    }


    // If we collide with a booster, then boost speed.
    // After 2 seconds, kill the booster object we collided with.

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Booster"))
        {
            StartCoroutine(speedBoost(other));
        }
    }

    private IEnumerator speedBoost(Collider other)
    {
        body.AddForce(movement_V * speed_B);
        yield return new WaitForSeconds(2.0f);
        Destroy(other.gameObject);
    }


    // We calculate the velocity needed for a far_left/left/right/far_right projection.
    // The choice depends on the "place" argument. The velocity is calculated as follows:
    // It equals the distance between our current X position and the target, multiplied by 20.
    // From physics, we know that F = m*a, U = a*t, with t=0.02 (deltaTime).
    // After testing, we modified the equations to have a more desirable result.
    // That means we have a big velocity that will decrease greatly as we reach the target.
    // We apply that velocity to the X axis, as well as our current speed to the Z axis.
    // As a result, we make tight turns and avoid obstacles.

    private IEnumerator moveBall(int place, float cur_speed)
    {
        float goal;
        float proportionate_boost;

        goal = place * 6;

        proportionate_boost = (goal - transform.position.x) * 20;
        body.velocity = new Vector3(proportionate_boost, 0, cur_speed);
        yield return new WaitForSeconds(1);
    }


    // Inactive function. Only if we need to get the ball to the ground instantly.

    private IEnumerator bigGravity()
    {
        yield return new WaitForSeconds(0.2f);
        transform.Translate(0f, (-transform.position.y + 0.6f), 0f);
    }


    // Function called from Obstacles when the ball dies.

    public void gameOver()
    {
        body.velocity = new Vector3(0, 0, 0);

        body.isKinematic = true;
        Destroy(gameObject);
    }


    // The ball's position on the Z axis. Called from Ranking and needed for position rank.

    public float zPosition()
    {
        return gameObject.transform.position.z;
    }

    public void setMaxSpeed(int difficulty)
    {
        speed_V = (speed_V - 10) + (10 * difficulty);
    }
}

