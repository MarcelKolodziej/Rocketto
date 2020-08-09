using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // ship control
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float rocketThrust = 140f;

    // Sounds 
    [SerializeField] AudioClip ThrustSound;
    [SerializeField] AudioClip DeadSound;
    [SerializeField] AudioClip LoadSound;

    [SerializeField] ParticleSystem ParticleEngine;
    [SerializeField] ParticleSystem ParticleDead;
    [SerializeField] ParticleSystem ParticleWin;

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
                CrashState();
                break;
            case "Finish_Pad":
                FinishLevelState();
                break;
            case "Fuel":
                    Debug.Log("Fuel");
                    break;
                default:
                    Debug.Log("Default");
                    break;
            }
    }

    private void FinishLevelState()
    {
        state = State.Win;
        Invoke("LoadNextScene", 1.0f);
        audioSource.Stop();
        audioSource.PlayOneShot(LoadSound);
        ParticleWin.Play();
    }

    private void CrashState()
    {
        state = State.Crash;
        Invoke("LoadFirstLevel", 1.0f);
        audioSource.Stop();
        audioSource.PlayOneShot(DeadSound);
        ParticleDead.Play();
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
                ParticleEngine.Stop();
            }
    }

    private void AddThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * rocketThrust * Time.deltaTime);

        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(ThrustSound);
        }
        ParticleEngine.Play();
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
