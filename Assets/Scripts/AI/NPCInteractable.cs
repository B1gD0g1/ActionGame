using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{

    private Animator animator;
    [SerializeField] private NPCHeadLookAt npcHeadLookAt;
    [SerializeField] private UIMangaer uiManager;


    [Header("台词")]
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

        //显示字幕
        PlayDialogue();

        animator.SetTrigger("Talk");

        float playerHeight = 1.7f;
        npcHeadLookAt.LookAtPosition(interactTransform.position + Vector3.up * playerHeight);
    }

    private void PlayDialogue()
    {
        if (npcDialogue != null && npcDialogue.dialogueLines.Length > 0)
        {
            //避免连续重复播放同一句，如果只有一句，则不会进入循环
            int newIndex;
            do
            {
                newIndex = Random.Range(0, npcDialogue.dialogueLines.Length);
            }
            while (newIndex == dialogueIndex && npcDialogue.dialogueLines.Length > 1);

            dialogueIndex = newIndex;

            //获取当前台词
            DialogueLine dialogueLine = npcDialogue.dialogueLines[dialogueIndex];

            if (dialogueLine != null)
            {
                //显示字幕
                uiManager.ShowSubtitle(dialogueLine.text, 5f, true, 0.03f);
            }
        }
    }
}
