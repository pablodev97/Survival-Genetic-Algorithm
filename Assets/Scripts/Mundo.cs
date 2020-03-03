using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mundo : MonoBehaviour
{
    // Variables que se tendrán en cuenta para la creación de los cromosomas
    public string[] statsNamesPresa = { "Velocidad", "RadioVista" };
    public string[] statsNamesDepredador = { "Velocidad", "RadioVista" };
    // Máximo valor posible de un cromosoma
    public float maxRange = 100f;

    // Lista de las plantas que se generarán en el mundo
    public List<Planta> comida;
    public Planta prefabComida;
    
    public static Mundo instance;

    
    /// <summary>
    /// Función que crea las plantas y las instancia en el mundo.
    /// </summary>
    /// <param name="numeroComida"> Número de plantas que se tienen que crear </param> 
    public void Initialize(byte numeroComida)
    {
        instance = this;        
        
        comida = new List<Planta>();

        for (int i = 0; i < numeroComida; i++)
        {
            Planta planta = new Planta();
            comida.Add(planta);

            comida[i] = Instantiate(prefabComida);
            comida[i].Initialize();
        }
    }

    /// <summary>
    /// Función que borra las plantas que queden en el mundo
    /// </summary>
    public void Reiniciar()
    {
        while(comida.Count > 0)
        {
            Destroy(comida[0].gameObject);
            comida.RemoveAt(0);
        }
           
    }

    // Update is called once per frame
    void Update()
    {
    }
}
