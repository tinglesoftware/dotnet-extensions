using Tingle.Extensions.PhoneValidators;
using Tingle.Extensions.PhoneValidators.Abstractions;
using Tingle.Extensions.PhoneValidators.Airtel;
using Tingle.Extensions.PhoneValidators.Safaricom;
using Tingle.Extensions.PhoneValidators.Telkom;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> related to <see cref="IPhoneNumberValidator"/>
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add an implementation of <see cref="IPhoneNumberValidator"/> and <typeparamref name="TImplementation"/> 
        /// into the service collection
        /// </summary>
        /// <typeparam name="TImplementation">the type of phone number validator</typeparam>
        /// <param name="services">the services collection in which to register the services</param>
        /// <returns></returns>
        public static IServiceCollection AddPhoneNumberValidator<TImplementation>(this IServiceCollection services)

            where TImplementation : AbstractPhoneNumberValidator
        {
            return services.AddScoped<IPhoneNumberValidator, TImplementation>()
                           .AddScoped<TImplementation>();
        }

        /// <summary>
        /// Add phone validator services for Safaricom
        /// </summary>
        /// <param name="services">the services collection in which to register the services</param>
        /// <returns></returns>
        public static IServiceCollection AddSafaricomPhoneNumberValidator(this IServiceCollection services)
        {
            return AddPhoneNumberValidator<SafaricomPhoneNumberValidator>(services);
        }

        /// <summary>
        /// Add phone validator services for Airtel
        /// </summary>
        /// <param name="services">the services collection in which to register the services</param>
        /// <returns></returns>
        public static IServiceCollection AddAirtelPhoneNumberValidator(this IServiceCollection services)
        {
            return AddPhoneNumberValidator<AirtelPhoneNumberValidator>(services);
        }

        /// <summary>
        /// Add phone validator services for Telkom
        /// </summary>
        /// <param name="services">the services collection in which to register the services</param>
        /// <returns></returns>
        public static IServiceCollection AddTelkomPhoneNumberValidator(this IServiceCollection services)
        {
            return AddPhoneNumberValidator<TelkomPhoneNumberValidator>(services);
        }
    }
}
