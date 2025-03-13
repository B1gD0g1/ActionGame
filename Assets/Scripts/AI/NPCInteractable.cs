using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{

    private Animator animator;
    private NPCHeadLookAt npcHeadLookAt;
    [SerializeField] private UIMangaer uiMangaer;


    [Header("台词")]
    [SerializeField] private NPCDialogue npcDialogue;
    private int dialogueIndex = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        npcHeadLookAt = GetComponent<NPCHeadLookAt>();
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
                uiMangaer.ShowSubtitle(dialogueLine.text, 5f, true, 0.03f);
            }
        }
    }
}
