#### 5.0.7 - 26.10.2018
* Trigger For Build

#### 5.0.6 - 16.10.2018
* Updates Discovery package

#### 5.0.5 - 15.08.2018
* Changes the authorization attribute for the GetTopicCounterController

#### 5.0.4 - 15.08.2018
* Builds MessageId from the request when sending a push notificaiton
* Configures log levels for log4net in a way that only pushnotification logs are DEBUG and everything else is ERROR

#### 5.0.3 - 14.08.2018
* Fixes the TopicSubsciptionTracker endpoint

#### 5.0.2 - 09.08.2018
* Recovers Pushy Unsubscribe controller

#### 5.0.1 - 07.08.2018
* Fixes TopiSubscriptionTracker redis lock connectionstring 

#### 5.0.0 - 03.08.2018
* Removes PushNotifications Aggregate
* Fixes Projections
* Synchronises with the client version

#### 4.1.0 - 02.08.2018
* Adds support for subscription to topics
* Updates Cronus to latest version
* Removes Aggregates for push notifications
* Various improvements and optimisations

#### 4.0.6 - 14.05.2018
* Republish the release

#### 4.0.5 - 14.05.2018
* Fixes a crash when trying to publish commands

#### 4.0.4 - 14.05.2018
* Updates the Discovery.Consul package

#### 4.0.3 - 14.05.2018
* Register IProjectionLoader

#### 4.0.2 - 11.05.2018
* Updated Cronus, Newtonsoft.Json and RabbitMQ transport

#### 4.0.1 - 16.04.2018
* Updates discovery packages

#### 4.0.0 - 16.04.2018
* Updates packages which introduced breaking changes

#### 3.2.0 - 05.04.2018
* Adds a check for the number of tokens which will receive pushnotification. FireBase has a limit of 1000 per request
* Removes ContentAvailable which makes the pushnotification to be a silent pushnotification. Apple/Google throttle or drop some of those when there are many sent to a device because it may drain battery on the device. Instead we use the `priority` set to `high`.
* Sets default badge to be `1`

#### 3.1.0 - 27.03.2018
* Fixes nuget package IDs

#### 3.0.0 - 11.12.2017
* Fixes possible null reference
* Adds notification data support
* Updates packages
* Removes requirement for title and body when sending push notification
* Fixes wrong response format
* Fixes building of subscriber id
* Fixes issue in the api with bulding subscriber id
* Fixes redis setting
* Adds normalized discovery
* Fixes missing documentation
* Updates deployment
* Changes app contexts
* Removes dependencies from api host
* Initial release for version 3 of pn
* Initial Release
