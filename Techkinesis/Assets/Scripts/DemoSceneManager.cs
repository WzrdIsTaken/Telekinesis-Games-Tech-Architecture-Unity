using UnityEngine;
using System.Collections;

// Manages the NPCs in the demo scene. Like the DemoNPCController, very rough / hacked together but only needed for the visual showcase

public class DemoSceneManager : MonoBehaviour
{
    [Header("NPC")]
    [Tooltip("How long it takes for an NPC to respawn")]
    [SerializeField] float npcRespawnTime;

    [Header("Scene")]
    [SerializeField] KeyCode resetSceneKey;

    NPCInfomation[] npcInfomation;

    void Start()
    {
        DemoNPCController[] npcs = FindObjectsOfType<DemoNPCController>();
        npcInfomation = new NPCInfomation[npcs.Length];

        for (int i = 0; i < npcs.Length; i++)
        {
            npcInfomation[i] = new NPCInfomation
            (
                npcs[i],
                npcs[i].transform.position,
                npcs[i].transform.rotation
            );
        }
    }

    public void ResetNPC(DemoNPCController npc)
    {
        StartCoroutine(FindAndReplaceNPC(npc));
    }

    IEnumerator FindAndReplaceNPC(DemoNPCController npc)
    {
        GameObject npcClone = Instantiate(npc.gameObject);  // Cloning rather than instantiating a new object because then we get to keep all of the script values for free
        npcClone.SetActive(false);

        yield return new WaitForSeconds(npcRespawnTime);

        for (int i = 0; i < npcInfomation.Length; i++)
        {
            if (npc == npcInfomation[i].npc)
            {
                npcClone.transform.position = npcInfomation[i].startPosition;
                npcClone.transform.rotation = npcInfomation[i].startRotation;
 
                npcInfomation[i].npc = npcClone.GetComponent<DemoNPCController>();
                npcClone.SetActive(true);

                break;
            }
        }
    }

    struct NPCInfomation
    {
        public DemoNPCController npc;
        public readonly Vector3 startPosition;
        public readonly Quaternion startRotation;
        
        public NPCInfomation(DemoNPCController _npc, Vector3 _startPosition, Quaternion _startRotation)
        {
            npc = _npc;
            startPosition = _startPosition;
            startRotation = _startRotation; 
        }
    }
}