using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball_Script : MonoBehaviour
{
    public float dist;  // raycasting distance
    public float speed_H;   // horizontal force multiplier
    public float speed_V;   // vertical force
    public float max_speed; // vertical speed limit
    public float max_speed_x; // horizontal speed limit
    public float speed_B;   // booster force
    private Rigidbody body;
    private Vector3 movement_V;
    private Vector3 movement_H;

    public Texture[] textures;  // set of different textures for billiard balls

    private bool f; // raycast_forward
    private bool r; // raycast_right
    private bool l; // raycast_left
    private bool fr; // raycast_front_right
    private bool fl; // raycast_front_left

    private bool flag;  // check if we have made a decision, so we do not overwrite it.

    private float cur_speed;  // our current vertical speed
    private int cur_place;  // our place on the X axis (horizontal)
    private int randTexture;

    // Use this for initialization
    private void Start()
    {
        // Pick a random billiard ball texture from the set (16 textures) for this specific ball

        randTexture = UnityEngine.Random.Range(0, textures.Length);

        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", textures[randTexture]);


        // Initialize variables

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
        float zVel;
        float total_Vel;

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

        if(transform.position.x >= 0)
        {
            cur_place = 1;
        }
        else if(transform.position.x < 0)
        {
            cur_place = -1;
        }

        // Calculate velocity on X and Z axis respectively, as well as the total velocity

        xVel = transform.InverseTransformDirection(body.velocity).x;
        zVel = transform.InverseTransformDirection(body.velocity).z;
        total_Vel = body.velocity.magnitude;

        cur_speed = zVel;
        //cur_speed = total_vel;

        if (f==false)
        {
            // No obstacle in front of us! We can move forward freely.
            // However, we need to stall the ball on the X axis first.
            // We also make sure our Z speed is lower than the limit.
            // If it isn't, we do not add more force.

            StartCoroutine(stallBall2(xVel));

            flag = false;
            if (body.velocity.magnitude < max_speed)
            {
                body.AddForce(0.0f, 0.0f, speed_V);
            }
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
                StartCoroutine(moveBall2(1, cur_speed, xVel));
            }
            else if (fr == true && fl == false && flag == false)
            {
                flag = true;
                StartCoroutine(moveBall2(-1, cur_speed, xVel));
            }
            else if(fr == false && fl == false && flag == false)
            {
                if(cur_place<=0)
                {
                    flag = true;
                    StartCoroutine(moveBall2(1, cur_speed, xVel));
                }
                else if(cur_place>0)
                {
                    flag = true;
                    StartCoroutine(moveBall2(-1, cur_speed, xVel));
                }
            }
            else if (fr == true && fl == true && flag == false)
            {
                if (cur_place <= 0)
                {
                    flag = true;
                    StartCoroutine(moveBall2(2, cur_speed, xVel));
                }
                else if (cur_place > 0)
                {
                    flag = true;
                    StartCoroutine(moveBall2(-2, cur_speed, xVel));
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
            if (hit.collider.gameObject.name.Contains("Wall") == true || hit.collider.gameObject.name.Contains("Obstacle") == true && hit.collider.gameObject.name.Contains("Jumper")==false)
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
    // After 5 seconds, kill the booster object we collided with.
    
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
        yield return new WaitForSeconds(5.0f);
        Destroy(other.gameObject);
    }


    // We calculate the velocity needed for a far_left/left/right/far_right projection.
    // The choice depends on the "place" argument. The velocity is calculated as follows:
    // It equals the distance between our current X position and the target, multiplied by 20.
    // From physics, we know that F = m*a, U = a*t, with t=0.02 (deltaTime).
    // After testing, we modified the equations to have a more desirable result.
    // So, we multiply the velocity with the mass of our object, to get the force.
    // Finally, we multiply that force with our Z speed so we can make a tight turn regardless of our vertical speed.
    // That means we add a big force that will decrease greatly as we reach the target.
    // However, it won't reach zero, which means we need another function to stall the ball on the X axis.

    private IEnumerator moveBall2(int place, float cur_speed, float x_speed)
    {
        //print(body.velocity.z);

        float goal;
        float vel;
        float the_force;

        goal = place * 6;

        vel = (goal - transform.position.x) * 20;
        the_force = vel * body.mass;  // Time.fixedDeltaTime

        body.AddForce(the_force* body.velocity.z, 0.0f, 0.0f);
        yield return new WaitForSeconds(0);
    }


    // We add a X force that equals double of our Z speed, multiplied by our X speed.
    // This means that our force on the X axis will increase greatly towards the opposite direction of our X speed, until our X speed becomes zero.
    // This way, we manage to stop the X axis speed very quickly, and therefore balance the ball as we desired.

    private IEnumerator stallBall2(float x_speed)
    {
        //print(body.velocity.x);

        body.AddForce(-body.velocity.x * body.velocity.z  * 2, 0.0f, 0.0f);
        yield return new WaitForSeconds(0);

        //body.drag = Mathf.Lerp(100, 1, t);
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
}
 
 