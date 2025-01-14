﻿using Blazored.LocalStorage;
using WebUI.Classes;

namespace WebUI.Models;

public class DataState
{
    public string? ApiKeyString { get; set; }
    public ChatLinkedList ChatMessages { get; } = new();
    public List<ChatLinkedListItem> CurrentMessageThread { get; set; } = new();
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public AvailableGptModels SelectedGptModel { get; set; } =
        (AvailableGptModels)OpenAI.GPT3.ObjectModels.Models.Model.ChatGpt3_5Turbo;
    public bool UsingByoApiKey { get; set; }
    public bool SavingByoApiKey { get; set; }
    public bool IsAwaitingResponse { get; set; }
    public bool IsAwaitingResponseStream { get; set; }
    public string NewMessageString { get; set; } = "";
}
