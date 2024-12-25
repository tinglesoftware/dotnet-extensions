using System.Globalization;
using System.Reflection;
using Xunit.v3;

#nullable disable

namespace Xunit;

/// <summary>Apply this attribute to your test method to replace the
/// <see cref="Thread.CurrentThread" /> <see cref="CultureInfo.CurrentCulture" /> and
/// <see cref="CultureInfo.CurrentUICulture" /> with another culture.
/// </summary>
/// <param name="culture">The name of the culture.</param>
/// <param name="uiCulture">The name of the UI culture.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class UseCultureAttribute(string culture, string uiCulture) : BeforeAfterTestAttribute
{
    private CultureInfo _originalCulture;
    private CultureInfo _originalUiCulture;

    /// <summary>Replaces the culture and UI culture of the current thread with <paramref name="culture" /></summary>
    /// <param name="culture">The name of the culture.</param>
    /// <remarks>
    /// <para>This constructor overload uses <paramref name="culture" /> for both <see cref="Culture" /> and <see cref="UICulture" />.</para>
    /// </remarks>
    public UseCultureAttribute(string culture)
        : this(culture, culture)
    {
    }

    /// <summary>Gets the culture.</summary>
    public CultureInfo Culture { get; } = culture is null ? CultureInfo.InvariantCulture : new CultureInfo(culture, false);

    /// <summary>Gets the UI culture.</summary>
    public CultureInfo UICulture { get; } = uiCulture is null ? CultureInfo.InvariantCulture : new CultureInfo(uiCulture, false);

    /// <summary>Stores the current <see cref="Thread.CurrentPrincipal" />
    /// <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
    /// and replaces them with the new cultures defined in the constructor.
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        _originalCulture = Thread.CurrentThread.CurrentCulture;
        _originalUiCulture = Thread.CurrentThread.CurrentUICulture;

        Thread.CurrentThread.CurrentCulture = Culture;
        Thread.CurrentThread.CurrentUICulture = UICulture;

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();

        // Change the default culture of any new threads created by the application domain.
        // These properties are only available as of .NET 4.5.
        // CultureInfo.DefaultThreadCurrentCulture = Culture;
        // CultureInfo.DefaultThreadCurrentUICulture = UICulture;
    }

    /// <summary>Restores the original <see cref="CultureInfo.CurrentCulture" /> and
    /// <see cref="CultureInfo.CurrentUICulture" /> to <see cref="Thread.CurrentPrincipal" />
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        Thread.CurrentThread.CurrentCulture = _originalCulture;
        Thread.CurrentThread.CurrentUICulture = _originalUiCulture;

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
    }
}