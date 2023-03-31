namespace PdfGeneration.Prince.Process.Internal;

using System;

public class MessageReceivedEventArgs : EventArgs
{
    public string Type { get; }
    public string Location { get; }
    public string Text { get; }

    public MessageReceivedEventArgs(string type, string location, string text)
    {
        Type = type;
        Location = location;
        Text = text;
    }
}
