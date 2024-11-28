using UnityEngine;
public class Pin : MonoBehaviour
{
    // Threshold for the pin to be considered fallen
    public float fallThreshold = 45.0f;
    private Vector3 initialPosition;
    private bool isFallen = false;

    private AudioSource audioSource;

    void Start()
    {
        initialPosition = transform.up;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isFallen)
        {
            CheckIfPinFallen();
        }
    }

    private void CheckIfPinFallen()
    {
        float angle = Vector3.Angle(transform.up, initialPosition);

        if (angle > fallThreshold)
        {
            isFallen = true;
            OnPinFall();
        }
    }
    private void OnPinFall()
    {
        Debug.Log($"{gameObject.name} is fallen!");

        ScoreManager.instance.AddPoints(1);
    }
    void OnCollisionEnter(Collision collision)
    {
        // Debug statement to check by what the pin was hit
        Debug.Log($"{gameObject.name} was hit by {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Bowling"))
        {
            audioSource.Play();
        }
    }
}
