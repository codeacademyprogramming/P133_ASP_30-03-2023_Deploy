let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").disabled = true;

connection.on("MesajQebulEle", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var group = document.getElementById("groupId").value;
    connection.invoke("MesajGonder", user, message, group)
        .catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("joinGroup").addEventListener("click", function (e) {
    e.preventDefault();

    var group = document.getElementById("groupId").value;
    var chat = document.getElementById("chats");
    var groups = document.getElementById("groups");

    connection.invoke("AddGroup", group)
        .catch(function (ee) {
            console.log(ee)
        })

    groups.classList.add("d-none");
    chat.classList.remove("d-none");
})