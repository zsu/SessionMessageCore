# What is SessionMessage

SessionMessage is an asp.net core library for Modal dialog/StatusBar notifications.

Some of the features of SessionMessage are:

  * Support modal dialog blocking notification and StatusBar non-blocking notification
  * Support cross page notification
  * Support Ajax request notification
  * Options of display position, display timeout, animation effect,etc.

# NuGet
```xml
Install-Package SessionMessage.UI
```
# Getting started with SessionMessage

  * Call the followings in Startup:  
  ```xml
  * services.AddMvc(options=> { options.Filters.Add(typeof(AjaxMessagesActionFilter)); });
  * services.AddSessionMessage();
  ```
  * Inject ISessionMessageManager and call ISessionMessageManager.SetMessage(MessageType.Info, MessageBehaviors.StatusBar, "your notification message") when you want to display message;
  * Use it on your page;
```xml
Razor:
  * Add @addTagHelper *, SessionMessage.UI to _ViewImports.cshtml
  * Add reference to jquery/jqury UI/toastr;
  * Insert <sessionmessage /> after reference to jquery/jqury UI/toastr;
```

# License
All source code is licensed under MIT license - http://www.opensource.org/licenses/mit-license.php
