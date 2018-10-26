#### 5.0.4 - 26.10.2018
* Triggers the MSI Build

#### 5.0.3 - 11.10.2018
* Adds content_mutable flag for IOS with default to true

#### 5.0.2 - 15.08.2018
* Fixes the TopicSubsciptionTracker endpoint
* * Logs an error when unsubscribe command fails to be published
* Reworks the InMemoryPushNotificationAggregator. The access to the internal buffer is synchronized and sends all notifications when the entire buffer is flushed
* Configures log levels for log4net in a way that only pushnotification logs are DEBUG and everything else is ERROR

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

#### 3.0.6 - 13.12.2017
* Adds logging after successfully sending pn

#### 3.0.5 - 13.12.2017
* Adds logging for the aggregator

#### 3.0.4 - 13.12.2017
* Handles attempts to unsubscribe from not existing aggregate

#### 3.0.3 - 13.12.2017
* Adds additional logs

#### 3.0.2 - 12.12.2017
* Fixes pushy issue with adding custom params

#### 3.0.1 - 12.12.2017
* Fixes pushy issue with adding custom params

#### 3.0.0 - 11.12.2017
* Adds notification data support
* Updates packages
* Removes requirement for title and body when sending push notification
* Fixes redis setting
* Fixes deployment
* Changes app contexts
* Initial release for version 3 of pn
* Initial Release
