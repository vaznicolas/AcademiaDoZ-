
// Nicolas Vaz

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.Message;

public sealed class TemaPreferencesUpdatedMessage : ValueChangedMessage<string>
{
    public TemaPreferencesUpdatedMessage(string value) : base(value)
    {
    }
}
