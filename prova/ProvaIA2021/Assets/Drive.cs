using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drive : MonoBehaviour {

	
    float rotationSpeed = 120.0F;     //Cria uma velocidade inicial
    float health = 10.0f;             //Cria uma quantidade de pontos vitais para o player
    float speed = 20.0F;              //Cria uma velocidade



    public GameObject bulletPrefab;   //Puxa o obejto prefab da bala


    
    public Transform bulletSpawn;     //Cria uma ponto de spawn para a bala


   
    public Slider healthBar;        //Cria uma slider dinamico para os pontos vitais

    
  


    private void Start()
    {
        






    }




    void Update() {
        float translation = Input.GetAxis("Vertical") * speed;             //Faz a movementação vertical
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;      //Faz a rotação
        translation *= Time.deltaTime;                                     //Faz movimento pelo Deltatime
        rotation *= Time.deltaTime;                                        //Faz a rotação pelo Deltatime
       
        transform.Translate(0, 0, translation);                            //Puxa um vetor que muda de acordo com a movimetanção
        transform.Rotate(0, rotation, 0);                                  //Puxa um vetor que muda de acordo com a rotação
        
        healthBar.value = (int)health;                                    //Faz a ligação do slider com os pontos de vida do player
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);    //Faz a barra de vida seguir o player
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);                 //Detecta a posição da barra de vida

        if (Input.GetKeyDown("space"))          //Caso apertar a barra de espaço
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);  //Puxa o objeto prefab da bala onde tem o Bulletspawn
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);                                                  //Puxa o rigidbody do objeto prefab da bala e aplica uma força
        }

        if(health == 0)                                               //Caso a vida for = a 0.
        {
            transform.position = new Vector3(239.5f, 0, -218f);       
            health = 100;                                             //Volta a vida para 100
        }
        
    }




    
    void OnCollisionEnter(Collision col)                 //Detecta a colisão
    {
        if (col.gameObject.tag == "bullet")              //Caso colidir com o objeto com tag "bullet"
        {
            health -= 10;                               //Remove 10 pontos de vida
        }
    }




    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "out")
        {
            transform.position = new Vector3(239.5f, 0, -218f);       
            health = 100;                                             //Volta a vida para 100
        }
    }



}
