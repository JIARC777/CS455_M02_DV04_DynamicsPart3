using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour
{

    public Rigidbody playerRB;
    public float speed;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("d")) {
            playerRB.AddForce(speed, 0, 0, ForceMode.VelocityChange);
        }
        if (Input.GetKey("w"))
        {
            playerRB.AddForce(0, 0, speed, ForceMode.VelocityChange);
        }
        if (Input.GetKey("s"))
        {
            playerRB.AddForce(0, 0, -speed, ForceMode.VelocityChange);
        }
        if (Input.GetKey("a"))
        {
            playerRB.AddForce(-speed, 0, 0, ForceMode.VelocityChange);
        }
        if (transform.position.y < -1)
        {
            restart();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            restart();
        }
    }

    void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
