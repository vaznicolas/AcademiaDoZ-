
// Nicolas Vaz

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.Message;

public sealed class BancoPreferencesUpdatedMessage : ValueChangedMessage<string>
{
    public BancoPreferencesUpdatedMessage(string value) : base(value)
    {
    }
}
