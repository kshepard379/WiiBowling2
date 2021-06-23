using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// Move initial ball position/rotation left/right
// using A/D keys on the keyboard & shift to swap modes.
// Space to launch ball.

public class LRMove : MonoBehaviour
{
    private float range = 2.8f;
    private float angleRange = 30.0f;
    private bool launched = false;
    public float speed = -10.0f;
    public int mode = 0;
    Rigidbody rBody;
    private Vector3 direction = new Vector3(0.0f, 0.0f, -1.0f);

    // Start is called before the first frame update
    void Start()
    {
        // Obtain rigidbody so we don't have to do it every frame
        rBody = gameObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
    }

    // Update is called once per frame
    void Update()
    {
        float lr = Input.GetAxisRaw("Horizontal");
        float moveSpeed = 0.01f;
        float rotateSpeed = 0.07f;
        float modifier = 1.0f;

        if(Input.GetKey(KeyCode.LeftControl)){
            modifier = 0.5f;
        }

        // If we are in left/right movement mode
        if(mode == 0){
            // And ball is not launched yet
            if(!launched){
                // Transform ball left/right
                transform.position = new Vector3(transform.position.x - (lr * moveSpeed * modifier), 1.23f, 29.6f);

                // Maintain ball within bounds of the lane (initially)
                if (transform.position.x > range) {
                    transform.position = new Vector3(range, 1.23f, 29.6f);
                } else if (transform.position.x < -range) {
                    transform.position = new Vector3(-range, 1.23f, 29.6f);
                }
            }
            
            // Switch to rotation mode
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                mode = 1;
            }

        }
        // Rotation mode!
        else{
            // if ball isn't launched
            if(!launched){
                // Rotate around current position
                transform.RotateAround(transform.position, Vector3.up, (lr * rotateSpeed * modifier));

                // Stay within rotation bounds (initially)
                if (Vector3.SignedAngle(transform.forward, Vector3.forward, Vector3.up) < -angleRange) {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, angleRange, transform.eulerAngles.z);
                } else if (Vector3.SignedAngle(transform.forward, Vector3.forward, Vector3.up) > angleRange) {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, -angleRange, transform.eulerAngles.z);
                }
            }

            // Switch to position mode
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                mode = 0;
            }
        }
        
        // Launch ball down the lane
        if(Input.GetKeyUp(KeyCode.Space) && !launched){
            mode = 0;
            
            GameObject temp = GameObject.Find("Canvas");
            Slider[] s = temp.GetComponentsInChildren<Slider>(false);

            rBody.useGravity = true;
            rBody.velocity = (speed-s[0].value)*transform.forward;

            ConstantForce forceComp = gameObject.GetComponent<ConstantForce>();
            forceComp.enabled = true;
            forceComp.torque = new Vector3(0.0f, 0.0f, s[1].value);

            launched = true;

            GameObject arrow = GameObject.Find("arrow");
            MeshRenderer mesh = arrow.GetComponent<MeshRenderer>();
            mesh.enabled = false;
        }

    }
}
