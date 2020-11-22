var propertyInspectorFactory = null;


/*
 * Call once while loading the JavaScript. The value must be a function which receives a client object
 * and returns an object containing the following functions:
 *
 *  sendToPropertyInspector(payload)
 *      Called when the plugin calls sendToPropertyInspector. The payload is passed as the
 *      first parameter as an object (JSON is already parsed).
 *
 *  didReceiveSettings(payload)
 *      Called when the plugin calls setSettings. The payload is passed as the
 *      first parameter as an object (JSON is already parsed).
 *
 * The factory function is called after the websocket is connected and the registration process has
 * completed. The factory function is passed a client object which has the following functions:
 *
 *  sendToPlugin(payload)
 *      Call to send the payload to the plugin. The plugin will receive the 'sendToPlugin' event.
 *
 *  getWebsocket()
 *      Provides access to the underlying websocket connection.
 *
 *  getUUID()
 *      Returns the property inspector UUID as passed by Stream Deck in connectElgatoStreamDeckSocket.
 *
 *  getInfo()
 *      Returns the info object (already parsed from JSON) as passed by Stream Deck in connectElgatoStreamDeckSocket.
 *
 *  getActionInfo()
 *      Returns the action info object (already parsed from JSON) as passed by Stream Deck in connectElgatoStreamDeckSocket.
 *
 *  getInitialSettings()
 *      Convenience function for retrieving the settings payload from the action info.
 *
 *
 *  setSettings(payload)
 *      Save data persistently for the action's instance.
 *
 *  TODO: getSettings, getGlobalSettings, setGlobalSettings, openUrl, logMessage
 *
 */
function setPropertyInspectorFactory(factory)
{
    if (typeof factory !== "function")
    {
        console.error("Factory parameter for setPropertyInspectorFactory must be a valid function");
        return;
    }

    propertyInspectorFactory = factory;
}


function connectElgatoStreamDeckSocket(inPort, inPropertyInspectorUUID, inRegisterEvent, inInfo, inActionInfo)
{
    if (propertyInspectorFactory == null)
    {
        console.error("setPropertyInspectorFactory not called");
        return;
    }

    const info = JSON.parse(inInfo);
    const actionInfo = JSON.parse(inActionInfo);

    const websocket = new WebSocket(`ws://127.0.0.1:${inPort}`);
    const settings = actionInfo.payload.settings || null;



    const client = {
        sendToPlugin: function(payload)
        {
            if (websocket.readyState !== 1)
            {
                console.warn("Websocket is not ready, sendToPlugin failed");
                return;
            }

            websocket.send(JSON.stringify({
                action: actionInfo.action,
                event: "sendToPlugin",
                context: inPropertyInspectorUUID,
                payload: payload
            }));
        },

        getWebsocket: function() { return websocket; },
        getUUID: function() { return inPropertyInspectorUUID; },
        getInfo: function () { return info; },
        getActionInfo: function () { return actionInfo; },
        getInitialSettings: function () { return settings; },

        setSettings: function(payload)
        {
            if (websocket.readyState !== 1) {
                console.warn("Websocket is not ready, setSettings failed");
                return;
            }

            websocket.send(JSON.stringify({
                event: "setSettings",
                context: inPropertyInspectorUUID,
                payload: payload
            }));
        }
    };


    let propertyInspector = null;

    websocket.onopen = function()
    {
        websocket.send(JSON.stringify({
            event: inRegisterEvent,
            uuid: inPropertyInspectorUUID
        }));

        propertyInspector = propertyInspectorFactory(client);
    };

    websocket.onmessage = function(message)
    {
        const parsedMessage = JSON.parse(message.data);
        switch (parsedMessage.event)
        {
            case "sendToPropertyInspector":
                if (propertyInspector !== null)
                    propertyInspector.sendToPropertyInspector(parsedMessage.payload);

                break;

            case "didReceiveSettings":
                if (propertyInspector !== null)
                    propertyInspector.didReceiveSettings(parsedMessage.payload);

                break;
        }
    };
}
