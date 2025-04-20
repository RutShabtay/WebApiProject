<p align="center" style="margin-bottom: -10;">
  <img src="https://github.com/user-attachments/assets/a8feadcb-9026-4103-8df8-7bf77853abdc" width="200">
</p>

# CareerVista – Bridging Students and Employers

This project is an RL (Role-based Authorization) system designed to easily and efficiently connect employers and job seekers. This platform provides a set of endpoints for interacting with  job opportunities and job seekers, based on the authenticated user role.
Security is enforced via JWT-based authentication, with optional Google login for added convenience.
The system supports full CRUD functionality for both jobs and users, with strict access controls ensuring that each operation aligns with the user’s permissions.

Endpoints
---

### Jobs

- GET /api/jobs

   - Role: User/Admin/SuperAdmin
   - Description: Receiving all available jobs in the database for all types of users.
   - Response Body: List of Jobs.
     
- GET /api/Jobs/{id}

    - Role: User/Admin/SuperAdmin
    - Description: Retrieve a specific Job by its ID.
    - Response Body: Details of the requested Job.
      
- POST /api/Jobs

    - Role: Admin
    - Description: Adding a new job to the job database.
    - Request Body: Details of the new Job.
    - Response Body: Location of the newly created Job.
    
- PUT /api/Jobs/{id}

    - Role: Admin
    - Description: Update details of a specific Job.
    - Request Body: Updated details of the Job.
      
- DELETE /api/Jobs/{id}

    - Role: Admin(only his job) /SuperAdmin
    - Description: Delete a specific Job.

---

### Users

- GET /api/user

    - Role: User/Admin/SuperAdmin
    - Description: Retrieve details of the current user.
    - Response Body: Details of the current user.
  
- GET /api/user

    - Role: SuperAdmin
    - Description: Retrieve a list of all users.
    - Response Body: List of users.
    - 
- POST /api/user

    - Role: SuperAdmin
    - Description: Add a new user to the system.
    - Request Body: Details of the new user.
    - Response Body: Location of the newly created user.

- DELETE /api/user/{id}

  - Role: SuperAdmin
  - Description: Delete a specific user from the system.
 
    
 


  

## Tech Stack

### 🔧 Backend – ASP.NET Core Web API
- JWT-based authentication and Google OAuth login
- RESTful API endpoints (with Swagger support)
- Role-based access control (Student / Employer / Admin)
- Request logging to file
- JSON-based data storage via injected service interface
- Docker support (with deployment to Google Cloud Run)
- Local server run via `dotnet run`

### 💻 Frontend – HTML, CSS, JavaScript
- Responsive and elegant user interface
- LocalStorage-based JWT handling
- Seamless UX for managing jobs and users
- Conditional navigation based on role (admin views)

---

## 🏁 How to Run the Project Locally

### 1. Clone the repository:

git clone https://github.com/RutShabtay/WebApiProject
cd CareerVista

### 2. Run the backend:

dotnet run

After running, open the browser using the URL provided in the terminal output (usually https://localhost:xxxx).

---

## 🔐 Admin Access for Full Permissions
To experience the full functionality of the system (including user management), log in with the following admin credentials:

*Username: RUTHI

*Password: RSS


## Challenges
  - Implement functionality to allow users to update their own details, such as name and password.
  - Implement user authentication using Google accounts.



📮 Feedback
Feel free to open issues or submit pull requests for improvements!
This project was built with ❤️ and a lot of attention to clean architecture and UX.









