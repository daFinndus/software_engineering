using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinScript : MonoBehaviour
{
    // Schwellenwert, ab wann ein Pin als umgefallen gilt
    [SerializeField] private float fallThreshold = 45.0f; // Winkel in Grad
    private Vector3 initialPosition;
    private bool isFallen = false;

    void Start()
    {
        // Speichere die Ausgangsrotation des Pins
        initialPosition = transform.up; // "Up"-Vektor zeigt nach oben
    }

    void Update()
    {
        if (!isFallen)
        {
            // Prüfe, ob der Pin umgefallen ist
            CheckIfPinFallen();
        }
    }

    private void CheckIfPinFallen()
    {
        // Berechne den Winkel zwischen der aktuellen Ausrichtung und der ursprünglichen Ausrichtung
        float angle = Vector3.Angle(transform.up, initialPosition);

        if (angle > fallThreshold)
        {
            isFallen = true;
            OnPinFall();
        }
    }

    private void OnPinFall()
    {
        Debug.Log($"{gameObject.name} ist umgefallen!");

        // Optional: Markiere den Pin als umgefallen, z. B. für Punktestand
        ScoreManager.Instance.AddPoints(1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug-Ausgabe für Kollisionen
        Debug.Log($"{gameObject.name} wurde getroffen von {collision.gameObject.name}");
    }
}
