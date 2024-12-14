using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NaveControl : MonoBehaviour
{
    public float velocidadRotacion = 80f;
    public float velocidadMovimiento = 6f;
    public float fuerzaDePropulsion = 1f;
    public int vida = 3;  // Vida del cohete
    public Text vidaText; // UI para mostrar la vida
    public AudioSource audioSource;

    private Rigidbody rb;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // Guardar la posici�n y rotaci�n iniciales
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;

        UpdateVidaUI(); // Actualizar la vida en la UI al iniciar el juego
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Llamada a funciones espec�ficas de control de la nave
        ControlarRotacion(deltaTime);
        ControlarMovimiento(deltaTime);
        ControlarPropulsion();
        AutoEstabilizar(deltaTime);
    }

    // Controla la rotaci�n de la nave
    void ControlarRotacion(float deltaTime)
    {
        float direccionRotacion = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) direccionRotacion = 1f;
        else if (Input.GetKey(KeyCode.RightArrow)) direccionRotacion = -1f;

        if (direccionRotacion != 0f)
        {
            transform.Rotate(Vector3.up * direccionRotacion * velocidadRotacion * deltaTime);
        }
    }

    // Controla el movimiento hacia adelante y atr�s
    void ControlarMovimiento(float deltaTime)
    {
        float direccionMovimiento = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) direccionMovimiento = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) direccionMovimiento = -1f;

        if (direccionMovimiento != 0f)
        {
            transform.Translate(Vector3.forward * direccionMovimiento * velocidadMovimiento * deltaTime);
        }
    }

    // Controla la propulsi�n hacia arriba y el sonido de arranque
    void ControlarPropulsion()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * fuerzaDePropulsion, ForceMode.Force);
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    // Autoestabiliza la nave para evitar inclinaciones en el eje Z
    void AutoEstabilizar(float deltaTime)
    {
        if (Mathf.Abs(transform.rotation.z) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, deltaTime * 2);
        }
    }

    // Detecta colisiones con objetos espec�ficos
    void OnCollisionEnter(Collision collision)
    {
        // Matriz de acci�n de colisi�n seg�n el tag del objeto con el que colisiona
        switch (collision.gameObject.tag)
        {
            case "SafePlatform":
                Debug.Log("Plataforma segura: No se pierde vida");
                break;

            case "DangerZone":
                Debug.Log("Zona peligrosa: Reiniciando posici�n");
                ReiniciarPosicion();
                break;

            default:
                Debug.Log("Colisi�n desconocida");
                break;
        }
    }

    // Detecta colisiones con los l�mites invisibles del mapa
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Boundary")
        {
            Debug.Log("L�mite del mapa alcanzado: Reiniciando posici�n");
            ReiniciarPosicion();
        }
    }

    // Reinicia la posici�n del cohete a la posici�n y rotaci�n iniciales
    private void ReiniciarPosicion()
    {
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        rb.velocity = Vector3.zero; // Reiniciar la velocidad para evitar que contin�e movi�ndose
        rb.angularVelocity = Vector3.zero; // Reiniciar la rotaci�n para evitar giros
    }

    // Actualiza el texto de vida en la UI
    private void UpdateVidaUI()
    {
        if (vidaText != null)
        {
            vidaText.text = "Vida: " + vida;
        }
    }
}
