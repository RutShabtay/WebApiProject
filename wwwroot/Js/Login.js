const url = "/Login";

const dom = {
    password: document.getElementById('password'),
    userName: document.getElementById('userName')
}


document.querySelector("form").addEventListener("submit", (e) => {
    e.preventDefault();
    const item = { password: dom.password.value, permission: null, userName: dom.userName.value };
    fetch(url, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)

    }).then(response => response.json())
        .then((res) => {

            if (res.status == 401) {
                alert("The username or password you entered is incorrect")
            }
            else {
                localStorage.setItem("Token", res)
            }
        }).then(() => {
            location.href = "index.html";
        })
        .catch(error => console.error('Unable to add item.', error));

})


