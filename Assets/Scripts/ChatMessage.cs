using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chatMessage;

    public void SetText(string str)
    {
        chatMessage.text = str;
    }
   
}
