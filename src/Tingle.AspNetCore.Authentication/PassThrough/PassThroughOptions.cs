using Microsoft.AspNetCore.Authentication;

namespace Tingle.AspNetCore.Authentication.PassThrough;

/// <summary>
/// Authentication options for <see cref="PassThroughHandler"/>
/// </summary>
public class PassThroughOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// The object provided by the application to process events raised by the pass through authentication handler.
    /// The application may implement the interface fully, or it may create an instance of <see cref="PassThroughEvents"/>
    /// and assign delegates only to the events it wants to process.
    /// </summary>
    public new PassThroughEvents Events
    {
        get { return (PassThroughEvents)base.Events!; }
        set { base.Events = value; }
    }
}
