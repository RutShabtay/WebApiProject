const url = "/Login";

const dom = {
    password: document.getElementById('password'),
    userName: document.getElementById('userName')
}

document.querySelector("form").addEventListener("submit", async (e) => {
    e.preventDefault();

    const item = { password: dom.password.value, permission: null, userName: dom.userName.value };
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)

        });

        if (!response.ok) {
            let errorData;
            try {
                errorData = await response.json(); // ננסה לקבל את המידע מהשרת
            } catch (jsonError) {
                errorData = { message: "Unknown error occurred" }; // אם זה לא JSON
            }

            const error = new Error(errorData.message);
            error.status = response.status;
            throw error;
        }
        const res = await response.json();
        localStorage.setItem("Token", res);
        location.href = "Jobs.html";

    }
    catch (error) {
        if (error.status === 401) {
            alert("👉Invalid login credentials. Please check your username and password and try again.");
        }
        else {
            alert("There was an error in the system, please contact our customer service at: 054-8541650")
        }
    };

})

// קבלי את הטוקן מהשרת אחרי התחברות
fetch("/Google/GoogleResponse") // כתובת ה-API שמחזירה את הטוקן
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.text(); // קרא כטקסט תחילה
    })
    .then(text => {
        if (!text) {
            throw new Error("Empty response received");
        }
        return JSON.parse(text); // הפוך ל-JSON ידנית
    })
    .then((data) => {
        if (data.token) {
            console.log("reached to token in google....");
            saveToken(data.token);
            init();
        } else {
            console.error("No token received", data);
        }
    })
    .catch((err) => {
        // fetch(`${crown}/${id}`, {
        //   method: "GET",
        //   headers: { Accept: "application/json", "Content-Type": "application/json" },
        //   body: JSON.stringify(item),
        // })
        //   .then((data) => {
        let data = { "Token": "kjhgfdxcvbhu765rfvbnji87ytg.8765rvhuygh.6r7ytfgb" };
        saveToken(data.token);
        init();
        // })
        // .catch((error) => console.error("Unable to update item.", error, "\ngoogle error: ", err));
    });

const init = () => {
    const token = localStorage.getItem("Token");

    if (!token) {
        alert("אין הרשאה. נא להתחבר.");
        window.location.href = "/Jobs.html"; // מחזיר לדף ההתחברות
    }
    getJobs();
}



