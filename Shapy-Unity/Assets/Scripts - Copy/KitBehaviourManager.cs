//using Inworld.Sample.Innequin;
using Inworld.Sample.Innequin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class KitBehaviourManager : MonoBehaviour
{

    public InworldCharacter3D character;
    private string instruction = " Pretend you are initiating a conversation and that this wasn't asked by the player! Also keep your responses short maximum of 1-2 sentences! Don't ask player if he is ready, reduce unnecessary confirmations!";

    void Start()
    {
        StartCoroutine(ManualPromt("What is Stefans name?"));
    }

    IEnumerator ManualPromt(string msg)
    {
        yield return new WaitForSeconds(2);
        print(msg);
        character.SendTrigger("testTrigger", true, new Dictionary<string, string>()
        {
            {"parametar", "Novska"}
        });
        //character.SendText(msg + instruction);
    }

    public void TestKit()
    {
        character.SendTrigger("testTrigger", true, new Dictionary<string, string>()
        {
            {"parametar", "Novska"}
        });
    }

}
