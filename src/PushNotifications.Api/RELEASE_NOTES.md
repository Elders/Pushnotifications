#### 2.0.0 - 09.11.2016
* Switches to consul pandora as default settings resolver
* Updates packages. There was a nasty bug where Thinktecture.IdentityServer3.AccessTokenValidation was unlisted. This produced non-consistent error while parsing access tokens. The solution was to remove that package and use the new IdentityServer3.AccessTokenValidation.

#### 1.0.0 - 14.10.2016
* We are sending 20 push notifications every 5 seconds if the usage of the CPU is below 50%
* Adds empty constructor for PushNotificationModel
* Adds check if the token is already used by another user and removing it from him if there is one.

#### 0.1.0 - 13.11.2014
* Initial release
