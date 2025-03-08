const url = '/Jobs';

let jobsArr = [];
let currentPermission = null;
let currentPassword = null;

let token = localStorage.getItem("Token");
if (token == null) {
    document.getElementById('logOut').style.display = "none";
    document.getElementById('userLink').style.display = "none";
}

else {
    const payload = token.split('.')[1];
    const decodedPayload = JSON.parse(atob(payload));
    currentPermission = decodedPayload.type;
    currentPassword = decodedPayload.password;
}
if (currentPermission === "user" || currentPermission == null)
    document.getElementById("AddForm").style.display = "none";
else
    document.getElementById("AddForm").style.display = "block";
// (function () {


// })();

// document.addEventListener("DOMContentLoaded", function () {

//     let token = localStorage.getItem("Token");
//     if (token == null) {
//         document.getElementById('logOut').style.display = "none";
//         document.getElementById('userLink').style.display = "none";
//     }

//     else {
//         const payload = token.split('.')[1];
//         const decodedPayload = JSON.parse(atob(payload));
//         currentPermission = decodedPayload.type;
//         currentPassword = decodedPayload.password;
//     }

//     if (currentPermission !== "SuperAdmin")
//         document.getElementById("AddForm").style.display = "none";
//     else
//         document.getElementById("AddForm").style.display = "block";
// })


const getJobs = () => {

    fetch(url).then(Response => Response.json())
        .then(data => displayItems(data))
        .catch(error => console.error('Unable to get items.', error))

}

const updateCounter = (itemCount) => {
    let counter = document.getElementById("counter");
    counter.innerHTML = itemCount;
}

const addItem = () => {

    const newJob = {
        "location": document.getElementById('Location').value.trim() === "" || document.getElementById('Location').value.trim() === undefined ? null : document.getElementById('Location').value.trim(),
        "jobFieldCategory": document.getElementById('JobFieldCategory').value.trim() === "" || document.getElementById('JobFieldCategory').value.trim() === undefined ? null : document.getElementById('JobFieldCategory').value.trim(),
        "sallery": document.getElementById('Sallery').value.trim() === "" || document.getElementById('Sallery').value.trim() === undefined ? null : document.getElementById('Sallery').value.trim(),
        "jobDescription": document.getElementById('JobDescription').value.trim() === "" || document.getElementById('JobDescription').value.trim() === undefined ? null : document.getElementById('JobDescription').value.trim(),
        "postedDate": document.getElementById('PostedDate').value.trim() === "" || document.getElementById('PostedDate').value.trim() === undefined ? null : document.getElementById('PostedDate').value.trim(),
        "CreatedBy": "Microsoft.AspNetCore.Mvc.ObjectResult"

    };
    fetch(url, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("Token")}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newJob)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })

        .then(() => {
            getJobs();
            document.getElementById('Location').value = '',
                document.getElementById('JobFieldCategory').value = '',
                document.getElementById('Sallery').value = '',
                document.getElementById('JobDescription').value = '',
                document.getElementById('PostedDate').value = ''
        })
        .catch(error => console.error('Unable to add item.', error));
}

const deleteItem = (id) => {
    fetch(`${url}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("Token")}`,
            'Accept': 'application/json'
        },
    })
        .then(() => getJobs())
        .catch(error => console.error('Unable to delete item.', error));
}

const displayItems = (jobsJson) => {

    const tBody = document.getElementById("Jobs");
    tBody.innerHTML = '';
    const button = document.createElement('button');
    updateCounter(jobsJson.length);

    jobsJson.forEach(element => {

        //calling to displatEditForm func
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${element.jobId})`)

        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        let location = document.createTextNode(element.location);
        td1.appendChild(location);

        let td2 = tr.insertCell(1);
        let jobFieldCategory = document.createTextNode(element.jobFieldCategory);
        td2.appendChild(jobFieldCategory);

        let td3 = tr.insertCell(2);
        let sallery = document.createTextNode(element.sallery);
        td3.appendChild(sallery);

        let td4 = tr.insertCell(3);
        let jobDescription = document.createTextNode(element.jobDescription);
        td4.appendChild(jobDescription);

        let td5 = tr.insertCell(4);
        let postedDate = document.createTextNode(element.postedDate);
        td5.appendChild(postedDate);

        //calling to deleteItem func
        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${element.jobId})`)
        if (currentPermission === "SuperAdmin" || (currentPermission === "Admin" && element.createdBy === currentPassword)) {

            let td6 = tr.insertCell(5);
            td6.appendChild(editButton);

            let td7 = tr.insertCell(6);
            td7.appendChild(deleteButton);
        }


    });
    jobsArr = jobsJson;
}


const displayEditForm = (id) => {
    var jobToEdit;
    jobsArr.forEach((item) => {
        if (item.jobId == id)
            jobToEdit = item;
    });

    document.getElementById('edit-Id').value = jobToEdit.jobId;
    document.getElementById('edit-Location').value = jobToEdit.location;
    document.getElementById('edit-JobFieldCategory').value = jobToEdit.jobFieldCategory;
    document.getElementById('edit-Sallery').value = jobToEdit.sallery;
    document.getElementById('edit-JobDescription').value = jobToEdit.jobDescription
    document.getElementById('edit-PostedDate').value = jobToEdit.postedDate;

    document.getElementById('editForm').style.display = 'block';

}

const editItems = () => {

    var itemId = document.getElementById('edit-Id').value;
    const item = {
        JobId: parseInt(itemId, 10),
        Location: document.getElementById('edit-Location').value.trim(),
        JobFieldCategory: document.getElementById('edit-JobFieldCategory').value.trim(),
        Sallery: document.getElementById('edit-Sallery').value.trim(),
        JobDescription: document.getElementById('edit-JobDescription').value.trim(),
        PostedDate: document.getElementById('edit-PostedDate').value.trim(),

        CreatedBy: jobsArr.find((element) => element.jobId == itemId).createdBy
    };
    fetch(`${url}/${itemId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("Token")}`
        },
        body: JSON.stringify(item)
    })
        .then(() => getJobs())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}


const closeInput = () => {
    document.getElementById('editForm').style.display = 'none';
}

const logOut = () => {
    localStorage.removeItem("Token");
    location.href = "index.html";
}





