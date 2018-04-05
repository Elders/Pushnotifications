#### 3.2.0 - 05.04.2018
* Adds a check for the number of tokens which will receive pushnotification. FireBase has a limit of 1000 per request
* Removes ContentAvailable which makes the pushnotification to be a silent pushnotification. Apple/Google throttle or drop some of those when there are many sent to a device because it may drain battery on the device. Instead we use the `priority` set to `high`.
* Sets default badge to be `1`

#### 3.1.0 - 27.03.2018
* Fixes nuget package IDs

#### 3.0.0 - 11.12.2017
* Adds notification data support
* Removes requirement for title and body when sending push notification
* Fixes issue where is success property was false all the time
* Adds normalized discovery
* Accepts datetime instead of timestamp when sending pn
* Initial release for version 3 of pn
