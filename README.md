# Mut.Log4net.EventHubAppender
This project contains the source for the Mut.Log4net.EventHubAppender 

## Installing
The Mut.Log4net.EventHubAppender package can be installed using NuGet

[Add NuGet install command]

## Configuration
Before you can configure the Mut.Log4net.EventHubAppender jou must setup a Azure EventHub for the logevents (see Azure EventHub Setup for details).

<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
  </configSections>
  <log4net>
    <root>
      <level value="ALL" />
      <appender-ref ref="EventHubAppender" />
    </root>
    <appender name="EventHubAppender" type="Mut.Log4net.EventHubAppender.EventHubAppender, Mut.Log4net.EventHubAppender">
      <eventHubName value="logging" />
      <eventHubConnectionString value="Endpoint=sb://[your-servicebusnamespace].servicebus.windows.net/;SharedAccessKeyName=SendEventsRule;SharedAccessKey=[YourSharedAccessKeyForSendEventsRule]" />
      <environment value="[Environment eg. Dev, Test, or Prod]" />
      <application value="[NameOfYourApp]" />
      <subsystem value="[SubSystem eg. WebSite]" />
    </appender>
  </log4net>
</configuration>

## Azure EventHub Setup
1. Log on to the Azure portal, and click "+New" select "Hybrid Integration" and then click "Service Bus" in the "Hybrid Integration" blade (this will take you to the old " Azure classic portal").
2. Click "Event Hub", then "Quick Create".
3. Type a name for your new "Event Hub", select the region, and then click "CREATE A NEW Event HUB".
4. If you didn't explicitly select an existing service bus namespace in a given region, the portal creates a namespace for you your new event hub (usually "[event hub name]-ns"). Select the namespace that was just created.
5. Click the "Event Hubs" tab at the top of the page, and then click the Event Hub you just created.
6. Click the "Configure" tab at the top, add a rule named "SendEventsRule" with "Send" rights in the permisions drop down, add another rule called ReceiveEventsRule with "Manage", "Send", and "Listen" rights, and then click "Save".
7. Click the Dashboard tab at the top of the page, and then click Connection Information. Copy the Connection String for the "SendEventsRule" this will be needin in the configuration of the Mut.Log4net.EventHubAppender.
