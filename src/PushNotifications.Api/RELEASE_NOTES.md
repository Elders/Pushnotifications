#### 2.0.13 - 24.03.2017
* Changes logging fix to FixFlags.All ^ FixFlags.Properties ^ FixFlags.UserName ^ FixFlags.LocationInfo
* Fixes locking issue for logs when logging to file

#### 2.0.12 - 06.02.2017
* Change the log settings.

#### 2.0.11 - 06.02.2017
* Change the log settings.

#### 2.0.10 - 06.02.2017
* Add integration with Pushy push notifications service.

#### 2.0.9 - 21.21.2016
* Fixes throttling config

#### 2.0.8 - 28.11.2016
* Fixes unhandled exception with projections

#### 2.0.7 - 28.11.2016
* Updates packages

#### 2.0.6 - 27.11.2016
* Uses VH of rabbitmq

#### 2.0.5 - 27.11.2016
* Changes the api deployment path

#### 2.0.4 - 24.11.2016
* Update Pandora

#### 2.0.3 - 24.11.2016
* Add config settings for controlling the push notifications throttling.

#### 2.0.1 - 11.11.2016
* Update Cronus to 3.0.2

#### 2.0.0 - 09.11.2016
* Switches to consul pandora as default settings resolver
* Updates packages. There was a nasty bug where Thinktecture.IdentityServer3.AccessTokenValidation was unlisted. This produced non-consistent error while parsing access tokens. The solution was to remove that package and use the new IdentityServer3.AccessTokenValidation.

#### 1.0.0 - 14.10.2016
* We are sending 20 push notifications every 5 seconds if the usage of the CPU is below 50%
* Adds empty constructor for PushNotificationModel
* Adds check if the token is already used by another user and removing it from him if there is one.

#### 0.1.0 - 13.11.2014
* Initial release
