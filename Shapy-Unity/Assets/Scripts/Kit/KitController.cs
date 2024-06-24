using Inworld;
using Inworld.Packet;
using Inworld.Sample.Innequin;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(NavMeshAgent))]
public class KitController : MonoBehaviour
{
    public static KitController Instance;
    public Transform player; // Reference to the player
    public float startFollowDistance = 10f; // Distance at which NPC starts following the player
    public float stopFollowDistance = 7f; // Distance at which NPC stops moving closer to the player
    public float followSpeed = 5f; // Speed of following

    private NavMeshAgent navMeshAgent;
    private Animator anim;
    private bool isFollowing = false;
    public bool canOpenChat = true;

    public string m_SendWoodTrigger, m_SendClothTrigger, m_SendGlassTrigger;
    private InworldCharacter3D m_CurrentCharacter;
    public StoryData story;
    public bool sendeventmanually;
    public GameObject fakeKit;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void Start()
    {
        m_CurrentCharacter = GetComponent<InworldCharacter3D>();
        //InworldController.CharacterHandler.Register(this);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = followSpeed;
        anim = transform.GetChild(0).GetComponent<Animator>();
        GameManager.Instance.UpdateKitStatus(true);
        if (m_CurrentCharacter != null)
        {
            m_CurrentCharacter.Event.onPacketReceived.AddListener(GetTrigger);
        }
    }

    void FixedUpdate()
    {
        if (sendeventmanually)
        {
            ActivateStoryEvent(story.storyText);
            sendeventmanually = false;
        }
        FollowPlayer();
        AdjustSpeed();
    }

    public void AdjustSpeed()
    {
        if (!isFollowing) 
        {
            anim.SetFloat("Walk", 0f);
            navMeshAgent.Stop();
            navMeshAgent.ResetPath();
            return; 
        }
        print(navMeshAgent.speed);
        navMeshAgent.speed = map(Vector3.Distance(player.position, transform.position), stopFollowDistance, startFollowDistance * 2, followSpeed, followSpeed * 2);
        print(navMeshAgent.speed);

        if (isFollowing)
        {
            if (navMeshAgent.speed > 7)
            {
                if (anim.GetFloat("Walk") != 1f)
                {
                    anim.SetFloat("Walk", 1f);
                }
                navMeshAgent.SetDestination(player.position);
            }
            else
            {
                if (anim.GetFloat("Walk") != 0.2f)
                {
                    anim.SetFloat("Walk", 0.2f);
                }
                navMeshAgent.SetDestination(player.position);
            }
            
        }
       
    }
    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        float res = (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        return res;
    }
    void FollowPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set in KitController.");
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > 50)
        {
            transform.position = new Vector3(player.position.x, player.position.y, player.position.z - 5);
        }
        if (distance > startFollowDistance)
        {
            isFollowing = true;
        }
        if (distance < stopFollowDistance)
        {
            isFollowing = false;
        }

       

        RotateTowardsPlayer();
    }

    public void GetTrigger(InworldPacket customPacket)
    {
        if (customPacket is CustomPacket myCustomPacket)
        { 
            print(myCustomPacket.Trigger.ToLower());
            if(myCustomPacket.Trigger.ToLower().Contains("activated.woodgoal1")) OnWoodGoalComplete("check_wood_in_unity");
            if (myCustomPacket.Trigger.ToLower().Contains("activated.clothgoal1")) OnClothGoalComplete("check_cloth_in_unity");
            if (myCustomPacket.Trigger.ToLower().Contains("activated.glassgoal1")) OnGlassGoalComplete("check_glass_in_unity");
        }
    }

    public void ActivateStoryEvent(string story)
    {
        Dictionary<string, string> param = new Dictionary<string, string>
        {
            ["story"] = story
        };
        m_CurrentCharacter.SendTrigger("send_story_event_from_unity", false, param);
    }
    public void OnWoodGoalComplete(string trigger)
    {
        print(trigger);
        Dictionary<string, string> param = new Dictionary<string, string>
        {
            ["amount"] = InventoryManager.Instance.ReturnItemQuantity(ItemReferenceManager.Instance.wood).ToString()
        };
        m_CurrentCharacter.SendTrigger(m_SendWoodTrigger.ToLower(), false, param);
    }

    public void OnClothGoalComplete(string trigger)
    {
        print(trigger);
        Dictionary<string, string> param = new Dictionary<string, string>
        {
            ["amount"] = InventoryManager.Instance.ReturnItemQuantity(ItemReferenceManager.Instance.cloth).ToString()
        };
        m_CurrentCharacter.SendTrigger(m_SendClothTrigger.ToLower(), false, param);
    }

    public void OnGlassGoalComplete(string trigger)
    {
        print(trigger);
        Dictionary<string, string> param = new Dictionary<string, string>
        {
            ["amount"] = InventoryManager.Instance.ReturnItemQuantity(ItemReferenceManager.Instance.glass).ToString()
        };
        m_CurrentCharacter.SendTrigger(m_SendGlassTrigger.ToLower(), false, param);
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the NPC upright

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
        }
    }

    public void ReloadKit()
    {
        gameObject.SetActive(true);
        fakeKit.SetActive(false);
        //InworldController.Instance.Reconnect();
    }
}
