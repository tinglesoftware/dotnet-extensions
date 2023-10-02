# Tingle.Extensions.PushNotifications

This library contains light weight clients for sending push notifications to devices via APNS and FCM. It exists either because there is no comprehensive library or the official library cannot be tested by stubbing HTTP requests

## Apple Push Notification Service (APNs)

This library is a wrapper around the service, you still need to understand how the service works in order to make correct requests. Make sure to read the [official docs](https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server).

In your Program.cs:

```cs
builder.Services.AddApnsNotifier(options =>
{
   options.TeamId = builder.Configuration["Apns:TeamId"];
   options.KeyId = builder.Configuration["Apns:KeyId"];
   options.BundleId = builder.Configuration["Apns:BundleId"];

   options.UsePrivateKey(keyId => environment.ContentRootFileProvider.GetFileInfo($"AuthKey_{keyId}.p8"));
});
```

In your appsettings.json (or any other configuration store/source you use):

```json
{
   "Apns:TeamId": "AA0A0AAAA0",
   "Apns:BundleId": "com.apple.iBooks",
   "Apns:KeyId": "AA00AA000A",
   // ....
}
```

You can also choose to provide the private key directly:

```cs
builder.Services.AddApnsNotifier(options =>
{
   options.TeamId = builder.Configuration["Apns:TeamId"];
   options.KeyId = builder.Configuration["Apns:KeyId"];
   options.BundleId = builder.Configuration["Apns:BundleId"];

   options.PrivateKeyBytes = (keyId) => File.ReadAllBytes($"apns-key.p8");
});
```

In a sample service (named `NotificationManager.cs` below):

```cs
private readonly IHostEnvironment environment;
private readonly ApnsNotifier apnsNotifier;

public NotificationManager(IHostEnvironment environment, ApnsNotifier apnsNotifier)
{
   this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
   this.apnsNotifier = apnsNotifier ?? throw new ArgumentNullException(nameof(apnsNotifier));
}

async Task SendApnsAsync(CancellationToken cancellationToken = default)
{
   var header = new ApnsMessageHeader
   {
      CollapseId = request.Collapse,
      Environment = environment.IsProduction() ? ApnsEnvironment.Production : ApnsEnvironment.Development,
      PushType = ApnsPushType.Background,
      DeviceToken = "<device-token-here>",
   };

   // According to Listing 7-1 Configuring a background update notification,
   // we should set content-available = 1 if the other properties of aps are not set.
   // https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html
   var aps = new ApnsMessagePayload { ContentAvailable = 1, };
   var data = new ApnsMessageData(aps);

   await apnsNotifier.SendAsync(header, data, cancellationToken);
}

```

## Firebase (FCM)

This library is a wrapper around the service, you still need to understand how the service works in order to make correct requests. Make sure to read the [official docs](https://firebase.google.com/docs/cloud-messaging).

### v1 HTTP API

In your appsettings.json (or any other configuration store/source you use):

```json
{
   "Firebase:ProjectId": "dummy-id",
   // ....
}
```

In `Program.cs` add the following code snippet:

```cs
builder.Services.AddFirebaseNotifier(options =>
{
   var projectId = configuration.GetRequiredValue<string>("Firebase:ProjectId");
   options.UseConfigurationFromFile(environment.ContentRootFileProvider.GetFileInfo($"{projectId}.json"));
});

// ...
```

You can also choose to provide the configuration details directly:

```cs
builder.Services.AddFirebaseNotifier(options =>
{
   options.ProjectId = builder.Configuration["Firebase:ProjectId"];
   options.ClientEmail = builder.Configuration["Firebase:ClientEmail"];
   options.TokenUri = builder.Configuration["Firebase:TokenUri"];
   options.PrivateKey = builder.Configuration["Firebase:PrivateKey"];
});
```

In a sample service (named `NotificationManager.cs` below):

```cs
private readonly IHostEnvironment environment;
private readonly FirebaseNotifier firebaseNotifier;

public NotificationManager(IHostEnvironment environment, FirebaseNotifier firebaseNotifier)
{
   this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
   this.firebaseNotifier = firebaseNotifier ?? throw new ArgumentNullException(nameof(firebaseNotifier));
}

async Task SendFirebaseAsync(CancellationToken cancellationToken = default)
{
   var message = new FirebaseRequestMessage
   {
      Token = "<token-here>",
      Android = new FirebaseMessageAndroid
      {
         // add title, body, etc here
         CollapseKey = null,
      },
      Data = new Dictionary<string, string>
      {
         ["key1"] = "something here",
      },
   };

   // send the push notification
   await firebaseNotifier.SendAsync(message, cancellationToken);
}
```

### Legacy HTTP API

In your Program.cs:

```cs
builder.Services.AddFcmLegacyNotifier(options =>
{
   options.Key = builder.Configuration["Firebase:Key"];
});
```

In your appsettings.json (or any other configuration store/source you use):

```json
{
   "Firebase:Key": "<your-legacy-key-here>",
   // ....
}
```

In a sample service (named `NotificationManager.cs` below):

```cs
private readonly IHostEnvironment environment;
private readonly FcmLegacyNotifier firebaseNotifier;

public NotificationManager(IHostEnvironment environment, FcmLegacyNotifier firebaseNotifier)
{
   this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
   this.firebaseNotifier = firebaseNotifier ?? throw new ArgumentNullException(nameof(firebaseNotifier));
}

async Task SendFirebaseAsync(CancellationToken cancellationToken = default)
{
   var message = new FcmLegacyRequest
   {
      RegistrationIds = ["<token1-here>","<token2-here>"],
      Data = new Dictionary<string, string>
      {
         ["key1"] = "something here",
      },
   };

   // send the push notification
   await firebaseNotifier.SendAsync(message, cancellationToken);
}
```
