using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{

    private Animator animator;
    [SerializeField] private NPCHeadLookAt npcHeadLookAt;
    [SerializeField] private UIMangaer uiManager;


    [Header("̨��")]
    [SerializeField] private NPCDialogue npcDialogue;
    private int dialogueIndex = 0;

    public bool IsDead { get; set; }

    public NPCHeadLookAt NPCHeadLookAt
    {
        get => default;
        set
        {
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        npcHeadLookAt = GetComponent<NPCHeadLookAt>();

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIMangaer>();
        }

        if (npcHeadLookAt == null)
        {
            Debug.LogError("NPCHeadLookAt component not found on " + gameObject.name);
        }
    }

    private void OnEnable()
    {
        uiManager.OnDialogueEnd += UiManager_OnEndInteraction;
    }

    private void OnDisable()
    {
        uiManager.OnDialogueEnd -= UiManager_OnEndInteraction;
    }

    private void UiManager_OnEndInteraction()
    {
        if (npcHeadLookAt != null)
        {
            npcHeadLookAt.StopLooking();
        }
        else
        {
            Debug.LogError("npcHeadLookAt is null when trying to StopLooking!");
        }
    }

    public void Interact(Transform interactTransform)
    {
        Debug.Log("Interact!");

        //��ʾ��Ļ
        PlayDialogue();

        animator.SetTrigger("Talk");

        float playerHeight = 1.7f;
        npcHeadLookAt.LookAtPosition(interactTransform.position + Vector3.up * playerHeight);
    }

    private void PlayDialogue()
    {
        if (npcDialogue != null && npcDialogue.dialogueLines.Length > 0)
        {
            //���������ظ�����ͬһ�䣬���ֻ��һ�䣬�򲻻����ѭ��
            int newIndex;
            do
            {
                newIndex = Random.Range(0, npcDialogue.dialogueLines.Length);
            }
            while (newIndex == dialogueIndex && npcDialogue.dialogueLines.Length > 1);

            dialogueIndex = newIndex;

            //��ȡ��ǰ̨��
            DialogueLine dialogueLine = npcDialogue.dialogueLines[dialogueIndex];

            if (dialogueLine != null)
            {
                //��ʾ��Ļ
                uiManager.ShowSubtitle(dialogueLine.text, 5f, true, 0.03f);
            }
        }
    }
}
