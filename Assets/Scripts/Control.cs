using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour, IEntity
{
    public LayerMask rayCastMask;
    Vector3 m_inputDir;
    private float m_height;
    private float m_radius;
    [SerializeField] private bool m_isGround;
    public float speedModifier;
    public float jumpModifier;
    private Vector3 m_velocity;
    private Vector3 m_groundNormal;
    public float stepOffset;
    public float skin;
    // Start is called before the first frame update
    void Start()
    {
        m_height = GetComponent<CapsuleCollider>().height;
        m_radius = GetComponent<CapsuleCollider>().radius;
        speedModifier = speedModifier > 0 ? speedModifier : 5;
        jumpModifier = jumpModifier > 0 ? jumpModifier : 5;

        var sm = GetComponent<StateMachine>();
        if (sm != null)
            sm.Initialize(this);
    }

void Update()
{
    Debug.DrawRay(transform.position,m_velocity,Color.red);
    Debug.DrawRay(transform.position,m_groundNormal * 5,Color.green);    
}
    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = Input.GetButton("Jump") ? 1 : 0;
        m_inputDir.x = x;
        m_inputDir.z = z;
        m_inputDir.y = 0;

        if(m_isGround)
        {
            m_velocity = ProjectOnPlane(m_inputDir, m_groundNormal);
            m_velocity = m_velocity.normalized * speedModifier;

            if (y > 0)
            {
                m_velocity.y = y * jumpModifier;
                m_isGround = false;
            }
        }
        else
        {
            m_velocity += Physics.gravity * Time.fixedDeltaTime;
        } 
        

        RaycastHit raycastHit;
        if(Physics.Raycast(transform.position, m_velocity, out raycastHit, m_velocity.magnitude * Time.fixedDeltaTime + m_radius, rayCastMask))
        {
            m_velocity = Vector3.ProjectOnPlane(m_velocity, raycastHit.normal);
        }


        if(Physics.Raycast(transform.position + new Vector3(0, stepOffset - m_height / 2, 0), Vector3.down, out raycastHit, 200, rayCastMask))
        {
            m_groundNormal = raycastHit.normal;
            if (transform.position.y - skin  <= raycastHit.point.y + m_height / 2)
            {
                transform.position = new Vector3(transform.position.x, raycastHit.point.y + m_height / 2, transform.position.z);
                m_isGround = true;
            }
            else
            {
                m_isGround = false;
            }
        }
        
        transform.position += m_velocity* Time.fixedDeltaTime;
    }
    Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
    {
        vector = vector - Vector3.Dot(vector, planeNormal) * planeNormal;
        return vector;
    }
}
