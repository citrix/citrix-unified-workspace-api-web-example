// Copyright © 2024. Cloud Software Group, Inc. All Rights Reserved.

let launching = false
async function PerformLaunch(card) {
    if (launching) {
        return;
    }

    try {
        launching = true;
        const resourceLinks = card.dataset
        console.log(resourceLinks)
        
        const launchType = document.getElementById('launch-type').value
        switch (launchType) {
            case "Receiver": {
                form = $($(card).children(".card-item-footer")).children("form")
                form.submit()
                break;
            }
            case "HTML5": {
                await launchHTML5(resourceLinks.launchica)
                break;
            }
            case "IFrame": {
                await launchIFrame(resourceLinks.launchica)
                break;
            }
        }
    }
    finally {
        launching = false;
    }
}

async function launchHTML5(launchUrl) {
    citrix.receiver.setPath("https://localhost:7182/receiver"); 
    let icaFile = $.get("/launchproxy/icafile?launchUrl=" + launchUrl)
    icaFile.done(function (data) { 
        const sessionId = "html5"
        const connectionParams = {
            "launchType": "newtab",
            "container": {
                "type": "window"
            }
        };

        function sessionCreated(sessionObject){
            const launchData = {"type": "ini", value: data};
            sessionObject.start(launchData);
        }
        citrix.receiver.createSession(sessionId, connectionParams,sessionCreated);
    });
}
async function launchIFrame(launchUrl) {
    citrix.receiver.setPath("https://localhost:7182/receiver"); 
    let icaFile = $.get("/launchproxy/icafile?launchUrl=" + launchUrl)

    icaFile.done(function (data) {
        const id = "iframe"
        const connectionParams = {
            "launchType": "embed",
            "container": {
                "type": "iframe",
                "id": "sessionIframe"
            }
        };
        
        document.getElementById("sessionIframe").style.display = "block";

        function sessionCreated(sessionObject){
            function connectionClosedHandler(event){
                document.getElementById("sessionIframe").style.display = "none";
            }
            sessionObject.addListener("onConnectionClosed",connectionClosedHandler);


            const launchData = {"type": "ini", value: data};
            sessionObject.start(launchData);
        }
        citrix.receiver.createSession(id, connectionParams,sessionCreated);
    });
}