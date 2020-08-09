using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // ship control
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float shipThrust = 140f;

    // Sounds 
    [SerializeField] AudioClip ThrustSound;
    [SerializeField] AudioClip DeadSound;
    [SerializeField] AudioClip LoadSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive,Crash, Win, Transcending }
    State state = State.Alive; 

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive) 
        {
            ApplyThrustInput();
            ApplyRotationInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }  // ignore collisions when dead
          
        switch (collision.gameObject.tag)
            {
                case "Friendly":
                    Debug.Log("Friendly");
                    break;
            case "Hazards":
                RocketCrash();
                break;
            case "Finish_Pad":
                Rocket_Win_Level();
                break;
            case "Fuel":
                    Debug.Log("Fuel");
                    break;
                default:
                    Debug.Log("Default");
                    break;
            }
    }

    private void Rocket_Win_Level()
    {
        state = State.Win;
        Invoke("LoadNextScene", 1.0f);
        audioSource.Stop();
        audioSource.PlayOneShot(LoadSound);
        successParticles.Play();
    }

    private void RocketCrash()
    {
        state = State.Crash;
        Invoke("LoadFirstLevel", 1.0f);
        audioSource.Stop();
        audioSource.PlayOneShot(DeadSound);
        deathParticles.Play();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void ApplyThrustInput()
    {   
        if (Input.GetKey(KeyCode.Space))
        {
            AddThrust();
        }
        else
            {
                audioSource.Stop();
                mainEngineParticles.Stop();
        }
    }

    private void AddThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * shipThrust * Time.deltaTime);

        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(ThrustSound);
        }
        mainEngineParticles.Play();
    }

    private void ApplyRotationInput()
    { 
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }
}
