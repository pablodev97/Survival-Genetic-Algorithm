using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta : MonoBehaviour
{
    // Variable para indicar cuando es comida
    private bool consumirse = false;

    // Escala para simular su desaparición
    public Vector3 cambioEscala;

    // Collider de la planta
    new BoxCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        cambioEscala = new Vector3(-1.2f, -1.2f, -1.2f);
        collider = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Función que situa la planta en un punto aleatorio del mundo.
    /// </summary>
    public void Initialize()
    {
        transform.position = new Vector3(Random.Range(-9.5f, 9.5f), 0f, Random.Range(-9.5f, 9.5f));
    }

    // Update is called once per frame
    void Update()
    {
        // Una vez comida se desactiva su collider y se escala para hacerla desaparecer.
        if(consumirse)
        {
            collider.enabled = false;
            transform.localScale += cambioEscala;

            if (transform.localScale.y < 0.1f)
            {
                consumirse = false;
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Función que detecta la colisión con una presa para de esa manera ser consumida la planta. Además se añade 1 al número 
    /// de plantas comidas por la presa en cuestión.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Presa")
        {
            consumirse = true;

            Presa presa = other.gameObject.GetComponent<Presa>();
            presa.AddComida();
        }
    }


}
