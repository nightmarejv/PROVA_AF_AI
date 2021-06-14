using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    
    public Transform bulletSpawn;   //Detecta o spawn do projetil
    public Slider healthBar;        //É o slider dinamico de vida
    public GameObject bulletPrefab; //Detecta o objeto da bala
    public Transform player;        //Detecta o Player.

    NavMeshAgent agent;              //Detecta o Nacmesh 
    public Vector3 destination;      //Detecta a direção do movimento
    public Vector3 target;           //Detecta a direção para o disparo da bala
    
    
    float speed1 = 10000f;           //Velocidade para o estado de fuga
    float speed2 = 20f;              //Velicidade para o estado após a fuga
    float rotSpeed = 5.0f;           //Velocidade da rotação 
    float health = 100.0f;           //Pontos de vida 
   
    

    float visibleRange = 80.0f;      //Distancia de visao
    float shotRange = 40.0f;         //Distancia de proguesso da bala




    

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();                                //Puxa o componente Navmesh
        
        agent.stoppingDistance = shotRange - 5;                                   //Para buferização
        InvokeRepeating("UpdateHealth",5,0.5f);                                   //Para chamar a ação de update dos pontos vitais
    }





    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);    //Faz com que o eixo da camera fique seguindo o jogador
        healthBar.value = (int)health;                                                     //Conectar a barra de vida com a quantidade de vida do player
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);                 //Registra a posição da barra de vida
        



    }




    void UpdateHealth()
    {
       if(health < 100)              //Se a vida for menor que 100, vc ganha pontos de vida
        health ++;                   //Fortifica a vida
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "bullet")              //Caso colidir com o objeto com a tag "bullet" 
        {
            health -= 10;                               //Perde-se 10 pontos vitais
        }
    }




    [Task]                                                                                      
    public void PickRandomDestination()                                                       //Tem a função de pegar um destino aleatorio 
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));      //È um ponto aleatorio
        agent.SetDestination(dest);                                                           //Destino
        agent.speed = speed2;                                                                 //A velocidade volta para 20 
        Task.current.Succeed();                                                               
    }




    [Task]                                                                            
    public void MoveToDestination()                                                   //Tem a função de mover para um destino
    {
        if(Task.isInspected)                                                          
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);          //Faz aparecer o tempo
        if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)   //Detecta a distancia percorrida pelo agente, se caso for menor que a distancia pra ele parar, conclui-se a task
        {
            Task.current.Succeed();                                                   
        }

    }
    



    [Task]                                                                            
    public void PickDestination(int x, int z)                                         //Tem a funçao de pegar o local
    {
        Vector3 dest = new Vector3(x, 0, z);                                          //Pega o local como parametro
        agent.SetDestination(dest);                                                   

        Task.current.Succeed();                                                       
    }
   



    [Task]                                                                            
    public void TargetPlayer()                                                        //Detecta o player alvo
    {
        target = player.transform.position;                                           //Tem a função de falar que o alvo é o player
        Task.current.Succeed();                                                       
    } 
    
   




    [Task]
    bool Turn(float angle)                                                                                      
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;     //Detecta que o p é a posição para realizar o turn.
        target = p;                                                                                             //Detecta que o target é igual ao p
        return true;                                                                                            
    }





    [Task]                                                                                                                                      
    public void LookAtTarget()                                                                                                                  //Detecta o target
    {
        Vector3 direction = target - this.transform.position;                                                                                   //Detecta que o vetor é o target
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);     //Pega a rotação para o AI virar para o player
       
        if (Task.isInspected)                                                                                                                   
            Task.current.debugInfo = string.Format("anfle={0}", Vector3.Angle(this.transform.forward, direction));                              //Ativa o task da AI
       
        if(Vector3.Angle(this.transform.forward, direction) < 5.0f)                                                                             //Detectase caso o angulo for menor que 5.0
        {
            Task.current.Succeed();                                                                                                             
        }
    }




    [Task]                                                                            
    public bool Fire()                                                                //Detecta o metodo para atirar
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);     //Cria uma intance para a bala      
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);                                                   //Cria uma força de impulso para a bala

        return true;                                                                                                                  
    }




    [Task]                                                                         
    bool SeePlayer()                                                               //Detecta o player
    {
        Vector3 distance = player.transform.position - this.transform.position;    //Detecta a distancia do player
        RaycastHit hit;                                                            //Faz a criaçao de um raycast para o hit
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);               //Cria um caminho vermelho 
       
        if (Physics.Raycast(this.transform.position, distance, out hit))           //Detecta a distancia do raycast
        {
            if(hit.collider.gameObject.tag == "wall")                              //Caso colidir com algum objeto com a tag "wall"
            {
                seeWall = true;                                                    //Deixa o seeWall verdadeiro
            }
        }
       
        if (Task.isInspected)                                                      
            Task.current.debugInfo = string.Format("wall={0}", seeWall);           //Ativa uma task na ai

        if (distance.magnitude < visibleRange && !seeWall)                         //Caso o seeWall for true
            return true;                                                           //Retorna um true
        else                                                                       //Caso nao
            return false;                                                          //Retorna um false

    }




    [Task]                                                 
    public bool IsHealthLessThan(float health)             //Cria um parametro para caso a vida estiver menor
    {
        return this.health < health;                       //Detecta que a vida é menor que a vida total 
    }




    [Task]                                 
    public bool Explode()                  //Cria um metodo para destruir o player
    {
        Destroy(healthBar.gameObject);     //Deleta a barra de vida
        Destroy(this.gameObject);          //Deleta o player
        return true;                       //Retorna um true
    }




    [Task]
    public void LookAtTargetFuga()                                                                                                              
    {
        Vector3 direction = target - this.transform.position;                                                                                   //Detecta que a direção do vetor é o target
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);     //Faz uma rotação para a ai virar para o player

     


        if (Task.isInspected)                                                                                                                   
        {
            Task.current.debugInfo = string.Format("anfle={0}", Vector3.Angle(this.transform.forward, direction));                              //Faz a ativação na task do ai
            agent.speed = speed1;                                                                                                               //Muda a velocidade do agente para 10.0000
        }

        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)                                                                             //Caso o angulo for menor que 5
        {
            Task.current.Succeed();                                                                                                              
        }
    }




    [Task]

    public void EmpurrarPraFora()
    {

        if (Vector3.Angle(this.transform.forward, destination) < 5.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed1 * Time.deltaTime);                      //Faz ir até o player e empurrar
        }
           
    }




    [Task]

    public void Morre()
    {    
        Destroy(gameObject);                       //Destroi o droid
    }




    [Task]
    public void Congela()
    {
        Vector3 direction = player.position;                                                                           //Detecta que a direção do vetor é o target.
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);     //Detecta uma rotação para o ai virar
        

        if (Task.isInspected)                                                                                                                   
        {
            Task.current.debugInfo = string.Format("anfle={0}", Vector3.Angle(this.transform.forward, direction));                              //Faz a ativação de uma task para o ai
                                                                                                                         
        }

        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)                                                                             //Caso o angulo for menor que 5
        {
            Task.current.Succeed();                                                                                                             
        }
    }
}



  


