const userUrl = '/Users';

let usersArr = [];

let currentPermission = null;
let currentPassword = null;

const getUserPermission = () => {
    let token = localStorage.getItem("Token");
    const payload = token.split('.')[1];
    const decodedPayload = JSON.parse(atob(payload));
    currentPermission = decodedPayload.type;
    currentPassword = decodedPayload.password;
    console.log(currentPermission)
    if (currentPermission !== "SuperAdmin")
        document.getElementById("AddForm").style.display = "none";
    else
        document.getElementById("AddForm").style.display = "block";


}
document.addEventListener("DOMContentLoaded", function () {
    getUserPermission();
});

const displayUsers = (usersJson) => {
    const tBody = document.getElementById("Users");
    tBody.innerHTML = '';
    const button = document.createElement('button');

    updateCounter(usersJson.length);

    usersJson.forEach(element => {

        //calling to displatEditForm func
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm('${element.password}')`)

        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        let password = document.createTextNode(element.password);
        td1.appendChild(password);

        let td2 = tr.insertCell(1);
        let permission = document.createTextNode(element.permission);
        td2.appendChild(permission);

        let td3 = tr.insertCell(2);
        let userName = document.createTextNode(element.userName);
        td3.appendChild(userName);

        //calling to deleteJob func
        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteUser('${element.password}')`)

        let td6 = tr.insertCell(3);
        td6.appendChild(editButton);

        let td7 = tr.insertCell(4);
        td7.appendChild(deleteButton);

    });
    usersArr = usersJson;
}


const getUsers = () => {
    getUserPermission();
    var param = '';
    if (currentPermission === "SuperAdmin")
        param = '/GetAllUsers';

    fetch(`${userUrl}${param}`, {
        method: 'Get',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("Token")}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    })
        .then(Response => {
            return Response.json();  // מחזירים את ה-Promise של ה-json
        })
        .then(
            (data) => {
                console.log(data);
                if (currentPermission !== "SuperAdmin")
                    displayUsers([data]);
                else {
                    displayUsers(data);
                }
            }
        )
        .catch(error => console.error('Unable to get items.', error));
}

const updateCounter = (itemCount) => {
    let counter = document.getElementById("counter");
    counter.innerHTML = itemCount;
}

const addUser = () => {

    const newUser = {
        "password": document.getElementById('Password').value.trim() === "" || document.getElementById('Password').value.trim() === undefined ? null : document.getElementById('Password').value.trim(),
        "permission": document.getElementById('Permission').value.trim() === "" || document.getElementById('Permission').value.trim() === undefined ? null : document.getElementById('Permission').value.trim(),
        "UserName": document.getElementById('UserName').value.trim() === "" || document.getElementById('UserName').value.trim() === undefined ? null : document.getElementById('UserName').value.trim(),
    };

    fetch(`${userUrl}`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("Token")}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newUser)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })

        .then(() => {
            getUsers();
            document.getElementById('Password').value = '',
                document.getElementById('Permission').value = '',
                document.getElementById('UserName').value = ''
            alert("What fun :) The user was successfully added.")
        })
        .catch(error => console.error('Unable to add item.', error));
}

const deleteUser = (password) => {
    let userConfirmed = window.confirm("Are you sure you want to perform this action?");
    if (!userConfirmed) {
        alert("CareerVista is happy with every user.")
        return;
    } else {
        alert("It was fun with you :) We would love to see you again.");
    }

    fetch(`${userUrl}/${password}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("Token")}`,
            'Accept': 'application/json'
        },
    }).then(() => {

        if (password === currentPassword) {
            localStorage.removeItem("Token");
            location.href = "Jobs.html";
        }
    })
        .then(() => getUsers())
        .catch(error => console.error('Unable to delete item.', error));

}
const displayEditForm = (password) => {
    var userToEdit;
    usersArr.forEach((user) => {
        if (user.password == password)
            userToEdit = user;
    });

    document.getElementById('currentPassword').value = userToEdit.password;
    document.getElementById('edit-Password').value = userToEdit.password;
    document.getElementById('edit-Permission').value = userToEdit.permission;
    document.getElementById('edit-UserName').value = userToEdit.userName;

    document.getElementById('editForm').style.display = 'block';

}


const editUser = () => {

    const user = {
        Password: document.getElementById('edit-Password').value.trim(),
        permission: document.getElementById('edit-Permission').value.trim(),
        userName: document.getElementById('edit-UserName').value.trim(),
    };

    const userPassword = document.getElementById('currentPassword').value.trim();
    fetch(`${userUrl}/${userPassword}`, {
        method: 'PUT',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("Token")}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    }).then(() => {
        if (currentPermission !== "SuperAdmin" || (currentPermission == "SuperAdmin" && userPassword === currentPassword)) {
            return fetch("/Login", {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(user)

            }).then(response => response.json())
                .then((res) => {
                    localStorage.setItem("Token", res);
                })
                .catch(error => console.error('Unable to add item.', error));
        }
    }).then(() => {

        getUsers()
    }).catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}



const closeInput = () => {
    document.getElementById('editForm').style.display = 'none';
}

const logOut = () => {
    localStorage.removeItem("Token");
    location.href = "Jobs.html";
}





