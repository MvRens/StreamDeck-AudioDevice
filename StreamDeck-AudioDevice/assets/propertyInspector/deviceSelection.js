function createPropertyInspector(client)
{
    const deviceSelect = document.getElementById("device");
    const settings = client.getInitialSettings();


    deviceSelect.addEventListener("change", function()
    {
        const newSettings = {
            deviceId: null
        };

        if (deviceSelect.selectedIndex > -1)
            newSettings.deviceId = deviceSelect.options[deviceSelect.selectedIndex].value;

        client.setSettings(newSettings);
    });


    client.sendToPlugin({
        event: "getDevices"
    });


    const handleGetDevices = function(payload)
    {
        let selectedIndex = 0;
        const options = [{
            value: "",
            text: "<Default playback device>"
        }];

        for (let i = 0; i < payload.devices.length; i++) {
            const device = payload.devices[i];

            options.push({
                value: device.id,
                text: device.displayName
            });

            if (device.id === settings.deviceId)
                selectedIndex = i;
        }

        replaceOptions(deviceSelect, options, selectedIndex);
    }


    return {
        sendToPropertyInspector: function(payload)
        {
            switch (payload.event)
            {
                case "getDevices":
                    handleGetDevices(payload);
                    break;
            }
        }
    };
}



function replaceOptions(select, newOptions, selectedIndex)
{
    for (let i = 0; i < newOptions.length; i++)
    {
        const newOption = newOptions[i];

        if (i >= select.length)
        {
            const option = new Option(newOption.text, newOption.value);
            select.add(option);
        }
        else
        {
            const option = select.options[i];
            option.text = newOption.text;
            option.value = newOption.value;
        }
    }

    while (select.length > newOptions.length)
        select.remove(select.length - 1);

    select.selectedIndex = selectedIndex;
}



setPropertyInspectorFactory(createPropertyInspector);