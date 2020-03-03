using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Depredador : MonoBehaviour
{
   
    // Cromosomas de la presa
    public float[] cromosomas;

    public float aptitud;

    // Velocidad máxima de la presa
    float maxVelocidad = 7;

    // Controlador del movimiento
    NavMeshAgent navMesh;

    // Enum de la actual acción de la presa
    public AccionesAnimal accionActual;

    // Número de plantas comidas
    private int presasComidas = 0;

    /*********        VISION ANIMAL               ********/
    // Radio de la visión que tiene el depredador
    float radioVision;
    float maxRadioVision = 5;

    // Ángulo de visión que tiene el depredador dentro de la circunferencia que hemos creado
    readonly float viewAngle = 120;

    // Layers que usaremos para comprobar con los RayCast
    public LayerMask presaMask;

    // Transform que guarda el target de la comida
    Transform targetPresa;
    // Distancia máxima para cazar la presa
    public float distanciaMaxima = 2;

    // Escala al morir
    Vector3 cambioEscala;

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        // La velocidad se calcula como un porcentaje de la máxima
        navMesh.speed = cromosomas[0] * maxVelocidad / 100;
        radioVision = cromosomas[1] * maxRadioVision/ 100;
        // Primera acción de la presa
        accionActual = AccionesAnimal.BuscarComida;

        cambioEscala = new Vector3(-0.02f, -0.02f, -0.02f);
    }

    public void Initialize()
    {
        // Posición inicial random
        transform.position = new Vector3(Random.Range(-9.5f, 9.5f), 0f, Random.Range(-9.5f, 9.5f));

        // Creación de los cromosomas random
        cromosomas = new float[Mundo.instance.statsNamesPresa.Length];
        for (int i = 0; i < cromosomas.Length; i++)
        {
            cromosomas[i] = Random.Range(0, Mundo.instance.maxRange);
        }

        presasComidas = 0;
    }

    public void Initialize(float[] nuevoCromosoma)
    {
        // Posición inicial random
        transform.position = new Vector3(Random.Range(-9.5f, 9.5f), 0f, Random.Range(-9.5f, 9.5f));

        // Se obtienen los cromosomas por parámetro
        cromosomas = nuevoCromosoma;
    }

    // Update is called once per frame
    void Update()
    {
        // Si se han comido 2 plantas puede descansar
        if (presasComidas == 2)
        {
            accionActual = AccionesAnimal.Descansar;
        }

        switch (accionActual)
        {
            case AccionesAnimal.BuscarComida:
                BuscarComida();
                break;
            case AccionesAnimal.Comiendo:
                float distancia = Vector3.Distance(transform.position, targetPresa.position);
                if (distancia > distanciaMaxima)
                {
                    accionActual = AccionesAnimal.BuscarComida;
                }
                break;
            case AccionesAnimal.Descansar:
                Descansar();
                break;
            case AccionesAnimal.Explorar:
                Explorar();
                break;
            case AccionesAnimal.Morir:
                Morir();
                break;
            case AccionesAnimal.None:
                break;

        }

    }

    /// <summary>
    /// Función que comprueba si la presa puede sobrevivir o debe morir.
    /// </summary>
    public void FinalDelDia()
    {
        if (presasComidas > 0)
            accionActual = AccionesAnimal.Descansar;
        else
            accionActual = AccionesAnimal.Morir;
    }

    /// <summary>
    /// Función que detiene el movimiento de la presa.
    /// </summary>
    public void Descansar()
    {
        navMesh.isStopped = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    /// <summary>
    /// Función que detiene el movimiento y la detección de colisiones. Además se desactiva el gameObject para simular su muerte.
    /// </summary>
    public void Morir()
    {
        navMesh.isStopped = true;

        transform.localScale += cambioEscala;

        if (transform.localScale.y < 0.1f)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Función que se encarga de buscar comida dado un radio, un ángulo de vista y una capa.
    /// Si encuentra destina al depredador al punto de esa comida encontrada, si no setea su próxima acción a Explorar.
    /// </summary>
    public void BuscarComida()
    {
        // Array contenedor de los elementos que chocan con nuestra esfera y estan dentro del layer comidaMask
        Collider[] comidaVisible = Physics.OverlapSphere(transform.position, radioVision, presaMask);

        if (comidaVisible.Length > 0)
        {

            // Bucle que comprueba mediante un RayCast qué enemigos se encuentran visibles para nuestro personaje
            for (int i = 0; i < comidaVisible.Length; i++)
            {
                targetPresa = comidaVisible[i].transform;

                // Dirección a la comida encontrada
                Vector3 dirToTarget = (targetPresa.position - transform.position).normalized;

                // Comprobamos que esté dentro de nuestro ángulo de visión
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
                {
                    accionActual = AccionesAnimal.Comiendo;

                    transform.LookAt(targetPresa);

                    navMesh.SetDestination(targetPresa.position);
                }
            }
        }
        else
        {
            accionActual = AccionesAnimal.Explorar;
        }
    }

    /// <summary>
    /// Función que busca un punto aleatorio dentro de un radio y setea su próxima accion a BuscarDepredador.
    /// </summary>
    public void Explorar()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 4f;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, 3f, -1);

        navMesh.SetDestination(navHit.position);


        accionActual = AccionesAnimal.BuscarComida;

    }

    /// <summary>
    /// Función que suma 1 al número de presas comidas.
    /// </summary>
    public void AddComida()
    {
        presasComidas++;
        accionActual = AccionesAnimal.Explorar;
    }

    /// <summary>
    /// Getter del número de presas comidas.
    /// </summary>
    /// <returns></returns>
    public int GetPresasComidas()
    {
        return presasComidas;
    }

    /// <summary>
    /// Getter de la velocidad del depredador.
    /// </summary>
    /// <returns></returns>
    public float GetVelocidad()
    {
        return cromosomas[0] * maxVelocidad / 100;
    }

    /// <summary>
    /// Getter del radio de visión del depredador.
    /// </summary>
    /// <returns></returns>
    public float GetRadioVision()
    {
        return radioVision;
    }


}
