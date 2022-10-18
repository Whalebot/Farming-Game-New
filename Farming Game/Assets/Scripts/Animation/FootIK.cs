/// <summary>
/// 
/// </summary>

using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]

//Name of class must be name of file as well

public class FootIK : MonoBehaviour
{

    //To access the Animator class
    protected Animator avatar;
    Status status;
    //To control whether to use the FootIK
    public bool ikActive = false;
    public float rayLength;
    //To control how much will affect the transform of the IK
    public float transformWeigth = 1.0f;
    public Vector3 footAngleOffset;
    //To make change the value smoothly
    public float smooth = 10;

    //The position of my left foot
    public Transform footL;

    //A Offset to the foot not jam on the floor
    public Vector3 footLoffset;

    //I will use to control when affect the position during animation
    public float weightFootL;

    //The position of my right foot
    public Transform footR;

    //A Offset to the foot not jam on the floor
    public Vector3 footRoffset;

    //I will use to control when affect the position during animation
    public float weightFootR;

    //I'll save the Raycast hit position of the feet
    private Vector3 footPosL;
    private Vector3 footPosR;

    //To access my Collider
    public CapsuleCollider myCollider;

    //Default [center] of my collider
    private Vector3 defCenter;

    //Default [Height] of my collider
    private float defHeight;

    //[LayerMask] to define with layer my foot [RayCast] will collide
    public LayerMask rayLayer;

    // Use this for initialization
    void Start()
    {
        //Definir os component
        //Set the component
        status = transform.parent.GetComponentInParent<Status>();
        avatar = GetComponent<Animator>();
        //	myCollider = GetComponent<CapsuleCollider>();
        //Guardar os valores
        //Save the values
        defCenter = myCollider.center;
        defHeight = myCollider.height;
    }

    void OnAnimatorIK(int layerIndex)
    {
        //Se o valor [avartar] estiver definido
        //If the [avatar] value is set
        if (avatar)
        {
            //Se o [ikActive] for [true]
            //If [ikActive] is [true]
            if (ikActive)
            {
                //Alterar o valor [transformWeigth] para 1 lisamente
                //Change the [transformWeigth] value to 1 smoothly
                if (transformWeigth != 1.0f)
                {
                    transformWeigth = Mathf.Lerp(transformWeigth, 1.0f, Time.deltaTime * smooth);
                    //Se o valor [transformWeigth] ficar maior que 0.99 ele será 1
                    //If the value [transformWeigth] be greater than 0.99 it will be 1
                    if (transformWeigth >= 0.99)
                    {
                        transformWeigth = 1.0f;
                    }
                }
                //Se a situação do player for [Idle]
                //If the situation of the player is [Idle]
                if (!avatar.GetBool("Walking"))
                {
                    //Definir o quanto vai afetar o [transform] do IK
                    //Set how much will affect the IK [transform]
                    avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, transformWeigth);
                    avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, transformWeigth);
                    avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, transformWeigth);
                    avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, transformWeigth);

                    IdleIK();
                }
                //If the situation of the player is [Walk] or [Run]
                //	else if(avatar.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Walk") || avatar.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LocomotionRun")){

                else
                {//Set how much will affect the IK [transform]
                    avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, transformWeigth * weightFootL);
                    avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, transformWeigth * weightFootL);
                    avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, transformWeigth * weightFootR);
                    avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, transformWeigth * weightFootR);
                    WalkRunIK();
                }
            }
            //Se o [ikActive] não for [true]
            //If [ikActive] is not [true]
            else
            {
                //Alterar o valor [transformWeigth] para 0 lisamente
                //Change the [transformWeigth] value to 0 smoothly
                if (transformWeigth != 0.0f)
                {
                    transformWeigth = Mathf.Lerp(transformWeigth, 0.0f, Time.deltaTime * smooth);
                    //Se o valor [transformWeigth] ficar menor que 0.01 ele será 0
                    //If the value [transformWeigth] be less than 0.01 it will be 0
                    if (transformWeigth <= 0.01)
                    {
                        transformWeigth = 0.0f;
                    }
                }
                //Definir o quanto vai afetar o [transform] do IK
                //Set how much will affect the IK [transform]
                avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, transformWeigth);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, transformWeigth);
                avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, transformWeigth);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, transformWeigth);
            }
        }
    }
    void IdleIK()
    {
        //Criar esse valor para usar o [RaycastHit]
        //Create this value to use the [RaycastHit]
        RaycastHit hit;
        //Receber a posição atual do pé esquerdo
        //Get the current position of the left foot
        footPosL = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
        //[RayCast] para o chão, pra saber a distancia
        //[RayCast] to the ground, to know the distance
        if (Physics.Raycast(footPosL + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
        {
            //Mostrar o [Ray]
            //Show [Ray]
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
            //Definir a nova posição do IK
            //Set the new position of IK
            avatar.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + footLoffset);
            //Definir a nova rotação do IK
            //Set the new rotation of IK
            avatar.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(Quaternion.Euler(footAngleOffset) * footL.forward, hit.normal), hit.normal));
            //Guardar a posição da colisão
            //Save the collision position
            footPosL = hit.point;
        }
        //Receber a posição atual do pé direido
        //Get the current position of the right foot
        footPosR = avatar.GetIKPosition(AvatarIKGoal.RightFoot);
        //[RayCast] para o chão, pra saber a distancia
        //[RayCast] to the ground, to know the distance
        if (Physics.Raycast(footPosR + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
        {
            //Mostrar o [Ray]
            //Show [Ray]
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green);
            //Definir a nova posição do IK
            //Set the new position of IK
            avatar.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + footRoffset);
            //Definir a nova rotação do IK
            //Set the new rotation of IK
            avatar.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(Quaternion.Euler(footAngleOffset) * footR.forward, hit.normal), hit.normal));
            //Guardar a posição da colisão
            //Save the collision position
            footPosR = hit.point;
        }
    }
    void WalkRunIK()
    {
        //Create this value to use the [RaycastHit]
        RaycastHit hit;        //Get the current position of the left foot
        footPosL = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
        //[RayCast] to the ground, to know the distance
        if (Physics.Raycast(footPosL + Vector3.up, Vector3.down, out hit,rayLength, rayLayer))
        {
            //Mostrar o [Ray]
            //Show [Ray]
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow);
            //Definir a nova posição do IK
            //Set the new position of IK
            avatar.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + footLoffset);
            //Definir a nova rotação do IK
            //Set the new rotation of IK
            avatar.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(Quaternion.Euler(footAngleOffset) * footL.forward, hit.normal), hit.normal));
            //Guardar a posição da colisão
            //Save the collision position
            footPosL = hit.point;


        }
        //Receber a posição atual do pé direido
        //Get the current position of the right foot
        footPosR = avatar.GetIKPosition(AvatarIKGoal.RightFoot);
        //[RayCast] para o chão, pra saber a distancia
        //[RayCast] to the ground, to know the distance
        if (Physics.Raycast(footPosR + Vector3.up, Vector3.down, out hit, rayLength, rayLayer))
        {
            //Mostrar o [Ray]
            //Show [Ray]
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green);
            //Definir a nova posição do IK
            //Set the new position of IK
            avatar.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + footRoffset);
            //Definir a nova rotação do IK
            //Set the new rotation of IK
            avatar.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(Quaternion.Euler(footAngleOffset) * footR.forward, hit.normal), hit.normal));
            //Guardar a posição da colisão
            //Save the collision position
            footPosR = hit.point;
        }
    }
    void Update()
    {
        //Se o [ikActive] for [true]
        //If [ikActive] is [true]
        if (ikActive)
        {
            weightFootL = avatar.GetFloat("FootLWeight");
            weightFootR = avatar.GetFloat("FootRWeight");
            //Se a situação do player for [Idle] e o [ikActive] for [true]
            //If the situation of the player is [Idle] and [ikActive] is [true]
            if (!avatar.GetBool("Walking"))
            {
             // IdleUpdateCollider();
            }
            //Se a situação do player for [Walk] ou [Run]
            //If the situation of the player is [Walk] or [Run]
          //  else if (avatar.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Walk") || avatar.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Run"))
          else 
            {
             //  WalkRunUpdateCollider();
            }
            //Se o [ikActive] não for [true]
            //If [ikActive] is not [true]
        }
        else
        {
            //Resetar os valores do meu Collider
            //Reset the values of my Collider
            myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y, Time.deltaTime * smooth), 0);
            myCollider.height = Mathf.Lerp(myCollider.height, defHeight, Time.deltaTime * smooth);
        }
    }
    void IdleUpdateCollider()
    {
        //Criar esse valor para calcular a diferença de altura dos os pés
        //Create this value to calculate the height difference of the feet
        float dif;
        //Calcular a diferença de altura dos os pés
        //Calculate the height difference of the feet
        dif = footPosL.y - footPosR.y;
        //Não deixar o valor ser menor que 0
        //Do not let the value be less than 0
        if (dif < 0) { dif *= -1; }
        //Mudar os valores do Collider dependendo do valor [dif]
        //Change the Collider values depending on the value [dif]
        myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y + dif, Time.deltaTime), 0);
        myCollider.height = Mathf.Lerp(myCollider.height, defHeight - (dif / 2), Time.deltaTime);
    }
    void WalkRunUpdateCollider()
    {
        //Criar esse valor para usar o [RaycastHit]
        //Create this value to use the [RaycastHit]
        RaycastHit hit;
        //Criar esse valor para guardar altura do chão da posição que estou 
        //Creating this value to save the height of the floor of the position I am
        Vector3 myGround = Vector3.zero;
        //Criar esse valor para calcular a diferença de altura
        //Create this value to calculate the height difference
        Vector3 dif = Vector3.zero;
        //Verificar a altura do chão da posição que estou 
        //Check the height of the floor of the position I am
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 3.0f, rayLayer))
        {
            //Guardar o valor
            //Save the value
            myGround = hit.point;
        }
        //RayCast para verificar a altura da posição de onde estou indo
        //RayCast to check the height of the position where I'm going
        if (Physics.Raycast(transform.position + (((transform.forward * (myCollider.radius))) + (myCollider.attachedRigidbody.velocity * Time.deltaTime)) + Vector3.up, Vector3.down, out hit, 2.0f, rayLayer))
        {
            //Mostrar o [Ray]
            //Show [Ray]
            Debug.DrawLine(transform.position + (((transform.forward * (myCollider.radius))) + (myCollider.attachedRigidbody.velocity * Time.deltaTime)) + Vector3.up, hit.point, Color.red);
            //Calcular a diferença de altura da posição que estou com a altura da posição de onde estou indo 
            //Calculate the height difference between the height of the position I'm with the height of the position where I'm going
            dif = hit.point - myGround;
            //Não deixar o valor ser menor que 0
            //Do not let the value be less than 0
            if (dif.y < 0) { dif *= -1; }
        }
        //Se o [dif] for menor que 0.5
        //If the [dif] is less than 0.5
        if (dif.y < 0.5f)
        {
            //Mudar os valores do Collider dependendo do valor [dif]
            //Change the Collider values depending on the value [dif]
            myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y + dif.y, Time.deltaTime * smooth), 0);
            myCollider.height = Mathf.Lerp(myCollider.height, defHeight - (dif.y / 2), Time.deltaTime * smooth);
            //Se o [dif] não for menor que 0.5
            //If the [dif] is not less than 0.5
        }
        else
        {
            //Resetar os valores do meu Collider
            //Reset the values of my Collider
            myCollider.center = new Vector3(0, Mathf.Lerp(myCollider.center.y, defCenter.y, Time.deltaTime * smooth), 0);
            myCollider.height = Mathf.Lerp(myCollider.height, defHeight, Time.deltaTime * smooth);
        }

    }
}
