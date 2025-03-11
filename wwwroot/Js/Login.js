const url = "/Login"; // כתובת ה-API להתחברות

// הגדרת הדומ של האלמנטים
const dom = {
    password: document.getElementById('password'),
    userName: document.getElementById('userName')
}


// הוספת מאזין לאירוע של שליחת הטופס

document.querySelector("form").addEventListener("submit", async (e) => {
    e.preventDefault();

    const item = { password: dom.password.value, permission: "Admin", userName: dom.userName.value };
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
});

