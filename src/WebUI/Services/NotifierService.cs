﻿namespace WebUI.Services;

public class NotifierService
{
    //https://learn.microsoft.com/en-us/aspnet/core/blazor/components/?view=aspnetcore-7.0#invoke-component-methods-externally-to-update-state
    public async Task Update()
    {
        if (Notify != null)
        {
            await Notify.Invoke();
        }
    }

    public async Task CancelMessageStream()
    {
        if (CancelMessageStreamEvent != null)
        {
            await CancelMessageStreamEvent.Invoke();
        }
    }

    public event Func<Task>? Notify;
    public event Func<Task>? CancelMessageStreamEvent;
}
