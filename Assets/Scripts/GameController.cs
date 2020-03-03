using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Prefabs para los depredadores y presas
    public Depredador prefabDepredador;
    public Presa prefabPresa;
    // Instancia de la clase Mundo
    public Mundo mundo;

    // Número que se introduce manualmente en la escena
    // de los depredadores, presas y comida de presa
    public byte numeroPresas;
    public byte numeroDepredadores;
    public byte numeroComida;

    // Lista que guarda las presas actuales
    List<Presa> presas;
    // Lista que guarda las presas que sobreviven más los hijos
    List<Presa> siguientesPresas;
    // Lista que guarda los depredadores actuales
    List<Depredador> depredadores;
    // Lista que guarda los depredadores que sobreviven más los hijos
    List<Depredador> siguientesDepredadores;

    // Ratio de mutuación
    public float ratioMutacion = 0.02f;
    // Generación actual
    int numeroGeneracion = 0;
    // Numero de hijos añadidos 
    int hijosAñadidosPresas = 0;
    int hijosAñadidosDepredadores = 0;

    // Duración de un día
    const float duracionDia = 100;
    // Tiempo restante del día
    float diaActual;
    // Tiempo que se resta a diaActual por frame
    public float tiempo;

    // User Interface
    public Image barraDuracion;
    public Text textoNumeroGeneracion;
    public Text textoVelocidadMediaPresas;
    public Text textoRadioVisionMediaPresas;
    public Text textoVelocidadMediaDepredadores;
    public Text textoRadioVisionMediaDepredadores;
    public Text textoNumeroDepredadores;
    public Text textoNumeroPresas;
    public Text textoNumeroHijosPresas;
    public Text textoNumeroHijosDepredadores;

    // Variables booleanas que indican el comienzo y el final de un día
    bool empezarDia;
    bool acabarDia;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(empezarDia)
        {
            // Se muestran los datos por pantalla
            textoNumeroGeneracion.text = "Generación Nº" + numeroGeneracion;

            textoVelocidadMediaPresas.text    = "Velocidad media: "    + GetVelocidadMedia();
            textoRadioVisionMediaPresas.text  = "Radio visión media: " + GetRadioVisionMedia();
            textoNumeroPresas.text      = "Número de presas: "   + presas.Count;
            textoNumeroHijosPresas.text = "Número de hijos: "    + hijosAñadidosPresas;

            textoVelocidadMediaDepredadores.text = "Velocidad media: " + GetVelocidadMediaDepredadores();
            textoRadioVisionMediaDepredadores.text = "Radio visión media: " + GetRadioVisionMediaDepredadores();
            textoNumeroDepredadores.text = "Número de presas: " + depredadores.Count;
            textoNumeroHijosDepredadores.text = "Número de hijos: " + hijosAñadidosDepredadores;

            // Actualizamos el tiempo restante 
            diaActual -= tiempo;
            barraDuracion.transform.localScale = new Vector3((float)diaActual * 3.5f / duracionDia, barraDuracion.transform.localScale.y, barraDuracion.transform.localScale.z);

            // Comprobamos si el día ha acabado 
            if(diaActual <= 0)
            {
                empezarDia = false;

                // Llamamos a la función FinalDelDia de cada presa y depredador
                for (int i = 0; i < presas.Count; i++)
                {
                    presas[i].FinalDelDia();
                }
                for (int i = 0; i < depredadores.Count; i++)
                {
                    depredadores[i].FinalDelDia();
                }

                acabarDia = true;
            }
        }
       
        if(acabarDia)
        {
            acabarDia = false;
        }
    }
    
    /// <summary>
    /// Función encargada de generar las primeras generaciones de las presas y los depredadores.
    /// </summary>
    public void PrimeraGeneracion()
    {   
        // Creamos el mundo
        mundo.Initialize(numeroComida);

        PrimeraGeneracionPresas();
        PrimeraGeneracionDepredadores();

        // Se setea el tiempo actual y la variable booleana para empezar
        diaActual = duracionDia;
        empezarDia = true;
    }

    /// <summary>
    /// Función que crea las primeras presas.
    /// </summary>
    public void PrimeraGeneracionPresas()
    {
        presas = new List<Presa>();

        for (int i = 0; i < numeroPresas; i++)
        {
            Presa presa = new Presa();
            presa = Instantiate(prefabPresa);
            presa.Initialize();
            presas.Add(presa);
        }
    }

    /// <summary>
    /// Función que crea los primeros depredadores.
    /// </summary>
    public void PrimeraGeneracionDepredadores()
    {
        depredadores = new List<Depredador>();

        for (int i = 0; i < numeroDepredadores; i++)
        {
            Depredador depredador = new Depredador();
            depredador = Instantiate(prefabDepredador);
            depredador.Initialize();
            depredadores.Add(depredador);
        }
    }

    /// <summary>
    /// Función encargada de generar la siguiente generación de las presas y los depredadores.
    /// </summary>
    public void SiguienteGeneracion()
    {
        // Se añada la nueva generación
        numeroGeneracion++;

        // Reiniciamos el mundo y comenzamos las siguientes generaciones
        mundo.Reiniciar();
        mundo.Initialize(numeroComida);
        SiguienteGeneracionPresas();
        SiguienteGeneracionDepredadores();

        // Resetamos las variable del tiempo y el día
        diaActual = duracionDia;
        empezarDia = true;
    }

    /// <summary>
    /// Función que crea la siguiente generación de presas, añadiendo los hijos correspondientes.
    /// </summary>
    public void SiguienteGeneracionPresas()
    {
        // Creamos una nuevas lista para guardar las siguiente generación
        siguientesPresas = new List<Presa>();


        // Se comprueba cual de las presas no han muerto
        for (int i = 0, j = 0; i < presas.Count; i++)
        {
            if (presas[i].accionActual != AccionesAnimal.Morir)
            {
                siguientesPresas.Add(presas[i]);
                j++;
            }
        }

        // Se destruyen las presas anteriores
        while (presas.Count > 0)
        {
            Destroy(presas[0].gameObject);
            presas.RemoveAt(0);
        }

        // Se copia siguientesPresas en la lista de presas
        presas = new List<Presa>(siguientesPresas);

        // Se vacia la lista de siguientesPresas
        for (int i = 0; i < siguientesPresas.Count; i++)
        {
            Destroy(siguientesPresas[i].gameObject);
        }

        siguientesPresas = null;
        
        hijosAñadidosPresas = 0;

        for (int i = 0; i < presas.Count - hijosAñadidosPresas; i++)
        {
            // Creo un cromosoma nuevo
            float[] nuevoCromosoma = new float[Mundo.instance.statsNamesPresa.Length];
            
            for (int j = 0; j < nuevoCromosoma.Length; j++)
            {
                nuevoCromosoma[j] = presas[i].cromosomas[j];
            }

            // Compruebo si ha tenido hijos
            if(presas[i].GetPlantasComidas() > 1)
            {
                Presa presaHijo = new Presa();
                
                // Creo un cromosoma nuevo
                float[] nuevoCromosomaHijo = new float[Mundo.instance.statsNamesPresa.Length];

                for (int j = 0; j < nuevoCromosomaHijo.Length; j++)
                {
                    // RECOMBINACION NORMAL DE UNO DE LOS PADRES
                    if (UnityEngine.Random.value > ratioMutacion)
                        nuevoCromosomaHijo[j] = presas[i].cromosomas[j];
                    else // MUTACION
                        nuevoCromosomaHijo[j] = UnityEngine.Random.Range(0, Mundo.instance.maxRange);
                }

                presas.Add(presaHijo);
                presas[presas.Count - 1] = Instantiate(prefabPresa);
                presas[presas.Count - 1].Initialize(nuevoCromosomaHijo);
                hijosAñadidosPresas++;
            }

            // Creo una nueva presa y la inicializo con el nuevo cromosoma.
            presas[i] = Instantiate(prefabPresa);
            presas[i].Initialize(nuevoCromosoma);
        }

    }

    /// <summary>
    /// Función que crea la siguiente generación de depredadores, añadiendo los hijos correspondientes.
    /// </summary>
    public void SiguienteGeneracionDepredadores()
    {
        // Creamos una nuevas lista para guardar la siguiente generación
        siguientesDepredadores = new List<Depredador>();

        // Se comprueba cual de los depredadores no ha muerto
        for (int i = 0, j = 0; i < depredadores.Count; i++)
        {
            if (depredadores[i].accionActual != AccionesAnimal.Morir)
            {
                siguientesDepredadores.Add(depredadores[i]);
                j++;
            }
        }

        // Se destruyen los depredadores anteriores
        while (depredadores.Count > 0)
        {
            Destroy(depredadores[0].gameObject);
            depredadores.RemoveAt(0);
        }

        // Se copia siguientesDepredadores en depredadores
        depredadores = new List<Depredador>(siguientesDepredadores);

        // Se vacia la lista de siguientesDepredadores
        for (int i = 0; i < siguientesDepredadores.Count; i++)
        {
            Destroy(siguientesDepredadores[i].gameObject);
        }

        siguientesDepredadores = null;

        hijosAñadidosDepredadores = 0;

        for (int i = 0; i < depredadores.Count - hijosAñadidosDepredadores; i++)
        {
            // Creo un cromosoma nuevo
            float[] nuevoCromosoma = new float[Mundo.instance.statsNamesDepredador.Length];

            for (int j = 0; j < nuevoCromosoma.Length; j++)
            {
                nuevoCromosoma[j] = depredadores[i].cromosomas[j];
            }

            // Compruebo si ha tenido hijos
            if (depredadores[i].GetPresasComidas() > 1)
            {
                Depredador depredadorHijo = new Depredador();

                // Creo un cromosoma nuevo
                float[] nuevoCromosomaHijo = new float[Mundo.instance.statsNamesDepredador.Length];

                for (int j = 0; j < nuevoCromosomaHijo.Length; j++)
                {
                    // RECOMBINACION NORMAL DE UNO DE LOS PADRES
                    if (UnityEngine.Random.value > ratioMutacion)
                        nuevoCromosomaHijo[j] = depredadores[i].cromosomas[j];
                    else // MUTACION
                        nuevoCromosomaHijo[j] = UnityEngine.Random.Range(0, Mundo.instance.maxRange);
                }

                depredadores.Add(depredadorHijo);
                depredadores[depredadores.Count - 1] = Instantiate(prefabDepredador);
                depredadores[depredadores.Count - 1].Initialize(nuevoCromosomaHijo);
                hijosAñadidosDepredadores++;
            }

            // Creo una nueva presa y la inicializo con el nuevo cromosoma.
            depredadores[i] = Instantiate(prefabDepredador);
            depredadores[i].Initialize(nuevoCromosoma);
        }

    }

    /// <summary>
    /// Devuelve la velocidad media de las presas.
    /// </summary>
    /// <returns></returns>
    public float GetVelocidadMedia()
    {
        float velMedia = 0;

        for(int i = 0; i < presas.Count; i++)
        {
            velMedia += presas[i].GetVelocidad();
        }

        return velMedia / presas.Count;
    }

    /// <summary>
    /// Devuelve el radio de visión medio de las presas.
    /// </summary>
    /// <returns></returns>
    public float GetRadioVisionMedia()
    {
        float media = 0;

        for (int i = 0; i < presas.Count; i++)
        {
            media += presas[i].GetRadioVision();
        }

        return media / presas.Count;
    }

    /// <summary>
    /// Devuelve la velocidad media de los depredadores.
    /// </summary>
    /// <returns></returns>
    public float GetVelocidadMediaDepredadores()
    {
        float velMedia = 0;

        for (int i = 0; i < depredadores.Count; i++)
        {
            velMedia += depredadores[i].GetVelocidad();
        }

        return velMedia / depredadores.Count;
    }

    /// <summary>
    /// Devuelve el radio de visión medio de los depredadores.
    /// </summary>
    /// <returns></returns>
    public float GetRadioVisionMediaDepredadores()
    {
        float media = 0;

        for (int i = 0; i < depredadores.Count; i++)
        {
            media += depredadores[i].GetRadioVision();
        }

        return media / depredadores.Count;
    }
}
