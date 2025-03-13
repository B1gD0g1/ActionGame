using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{

    private Animator animator;
    private NPCHeadLookAt npcHeadLookAt;
    [SerializeField] private UIMangaer uiMangaer;


    [Header("̨��")]
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
                uiMangaer.ShowSubtitle(dialogueLine.text, 5f, true, 0.03f);
            }
        }
    }
}
