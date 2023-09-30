<!-- TODO: update this file to reflect changes in usage -->
# Tingle.Extensions.PushNotifications

Push notifications are messages that pop up in a mobile device. Mainly users see a notification as a banner or a pop-up alert as they are using their phone. They can be scheduled by app publishers to be sent at a specific time. Users do not have to be in the app or to be using their devices to receive them. The push notifications can be targeted to segments of the user base, and even personalized for specific app users which is a major advantage compared to SMS text messaging. However, they also require the management of user identification data.

The actors in sending push notifications include:
**Operating System Push Notification Service(OSPNS)**: Each mobile platform has support for push notifications. Google uses Firebase Cloud Messaging to send push notifications while for iOS Apple Notification service is used.

**App publisher**: The app publisher enables their app with an Operating System Push Notification Service. Then the publisher uploads the app to the app store.

**Client app**:This is an OS specific app installed on a user's device. It receives incoming notifications.

`Tingle.Extensions.Notifications` is a customized library developed by Tingle Software which comprises of push notification providers for both Apple and Google's Firebase. A provider is a server that is deployed and managed to work with the notification services.

## Apple Push Notification Service (APNS)

Apple Push Notification Service(APNs) is a service that allows your device to be constantly connected to Apple's push notification server.

When you want to send a push notification to an application installed on the users' devices, you(the provider) can contact the APNs so that it can deliver a push message to the particular application installed on the intended device.

iOS lets users customize push notifications at an individual app level. Users can turn sounds on or off, and pick the style that iOS uses to show a notification. Users can also control the red “badge” showing the number of unread notifications on an app’s homescreen icon.

To securely connect the APNs, you can use provider authentication tokens or provider certificates. This library has been customized to use tokens for secure connection. The provider API supports the JWT specification letting you pass statements and metadata, referred to as claims, to APNs along with each push notification.

The provider authentication token is a JSON object that is constructed whose header must include:

- The encryption algorithm(alg) used to encrypt the token.
- A 10-character key identifier(kid) key obtained from the developer account.

The payload of the token in this library includes:

- The issuer(iss) registered claim key, whose value is your 10-character team ID obtained from your developer account.
- The issued at(iat) registered claim key, whose value indicates the time at which the token was generated, in terms of number of seconds, since Epoch, in UTC.

After the token is created it must be signed with a private key. The token is encrypted using the Elliptic Curve Digital Signature Algorithm(ECDSA) with the SHA 256 hash algorithm.

The library contains the configuration options for establishing a connection with the appropriate APNs server,that is, the development server whose URL is api.development.push.apple.com:443 and the production server whose URL is api.push.apple.com:443.

### Configuration

```json
{
   "Apns:AppBundleIdentifier": "my_app_bundle_identifier",
   "Apns:PrivateKey": "gw3XzpoY9kWsKyca74ReCg==",
   "Apns:PrivateKeyId": "gw3XzpoY9kWsKyca74ReCg==",
   "Apns:TeamId": "gw3XzpoY9kWsKyca74ReCg==",
   // ....
}
```

### Adding to Services Collection

In `Program.cs` add the following code snippet:

```cs
// Add Apple Notification services
builder.Services.AddApnsNotifier(Configuration.GetSection("Apns"));

// The sample service we'll use to demonstrate usage
builder.Services.AddScoped<NotificationManager>();
// ....
```

The APNs message format in the library has the following parameters:

- unique identifier for the message
- token for the device to send message to
- absolute expiry time in seconds
- priority of message
- actual data sent to the device

### Usage In A Sample Service
In `NotificationManager.cs` add the following code snippet:

```cs
private readonly ApnsNotifier apnsNotifier;
public NotificationManager(ApnsNotifier apnsNotifier)
{
   this.apnsNotifier = apnsNotifier;
}

async Task SendApnsAsync(CancellationToken cancellationToken = default)
{
    // prepare the aps section/node
    // According to Listing 7-1 Configuring a background update notification,
    // we should set content-available = 1 if the other properties of aps are not set.
    // https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html
    var aps = new ApnsMessagePayload { ContentAvailable = 1 };

    await apnsNotifier.SendAsync(token: "provide_token_here",
                                 payload: aps,
                                 action: "provide_action_here",
                                 extras: new {}, // provide any extras here
                                 collapseId: "provide_key_here", // provide this if you'd like multiple notifications with same identifier to appear as one to the user
                                 cancellationToken: cancellationToken);
}

```

## Firebase (FCM)

Google uses Firebase Cloud Messaging(FCM) to send notifications to apps. FCM is a free cross platform messaging solution that lets you send push notifications to your audience, without having to worry about the server code. Since FCM is one of Firebase services, it is required for the app to be registered with Firebase. An API key is then provided after registration.

Whenever a user downloads the application, Firebase issues a unique ID to the app-device combination so as to enable push notifications from Firebase. After the app-device combination has been registered, an app server identification is also required. It enables the app server to send notifications to the user's device on behalf of the app. A server ID is created using API keys provided by Firebase. After both of these registrations have been done it is possible to send push notifications.

Unlike the Apple Notification Service where the notification message can be configured, for Firebase the notification messages are handled by the Firebase SDK.

When using the legacy HTTP protocol, the app server should direct all requests to the endpoint: https://fcm.googleapis.com/fcm/send.

The push request as modelled in the library is a JSON formatted string with the following properties:
`registration_ids`: An array of registration tokens to which to send a multicast message (a message sent to more than one registration token). It can be ignored if to is used.
`collapse_key`: Used to avoid sending too many of the same messages when the device comes back online or becomes active.
`to`: The particular registration token to be targeted.  It can be ognored if registration_ids is used.
`data`: The data to be sent to the device.

### Configuration

```json
{
    "Firebase:ProjectId": "my_project_id_here",
    "Firebase:Key": "my_key_here",
....
}
```

### Adding to Services Collection

In `Program.cs` add the following code snippet:

```cs
// Add Fcm notification services
builder.Services.AddFirebaseNotifier(Configuration.GetSection("Firebase"));

// The sample service we'll use to demonstrate usage
builder.Services.AddScoped<NotificationManager>();

// ...
```

### Usage In A Sample Service

In `NotificationManager.cs` add the following code snippet:

```cs
private readonly FirebasePushNotifier firebaseNotifier;
public NotificationManager(FirebasePushNotifier firebaseNotifier)
{
   this.firebaseNotifier = firebaseNotifier;
}

async Task SendFirebaseAsync(CancellationToken cancellationToken = default)
{
   var message = new FirebasePushRequestModel
   {
      RegistrationIds = ["","",...], // provide tokens here
      Data = new StandardNotificationData { Action = "my_action_here", Extras = new {} // my extras here if any },
      CollapseKey = collapseKey, // provide this if you'd like multiple notifications with same identifier to appear as one to the user
   };

   // send the push notification
   await firebaseNotifier.SendAsync(message, cancellationToken);
}

```
